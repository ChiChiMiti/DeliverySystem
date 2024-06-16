using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ML;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace DeliverySystem.Pages.Prediction
{
    [Authorize(Roles = "Administrator")]
    public class PredictionModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly MLContext _mlContext;

        public PredictionModel(ApplicationDbContext context)
        {
            _context = context;
            _mlContext = new MLContext();
        }

        [BindProperty]
        public string SelectedFruitId { get; set; }
        public List<Fruit> Fruits { get; set; }
        public DateTime? PredictedDate { get; set; }

        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Fruits = await _context.Fruits.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Fruits = await _context.Fruits.ToListAsync();
            var selectedFruit = await _context.Fruits.FindAsync(int.Parse(SelectedFruitId));

            if (selectedFruit == null)
            {
                return Page();
            }

            var orders = await _context.Orders
                .Include(o => o.Fruit)
                .Where(o => o.FruitId == selectedFruit.Id)
                .ToListAsync();

            var totalOrdered = orders.Sum(o => o.Amount);
            var initialStock = selectedFruit.Stock + totalOrdered;

            var orderDataList = new List<OrderData>();
            float currentStock = (float)initialStock;

            if(!orders.Any())
            {
                ErrorMessage = "No orders found for the selected product";

                return Page();
            }

            foreach (var order in orders)
            {
                currentStock -= (float)order.Amount;

                var daysSinceStart = (float)(order.OrderDate - new DateTime(2024, 6, 1)).TotalDays;

                var orderData = new OrderData
                {
                    Fruit = selectedFruit.Name,
                    Date = (float)daysSinceStart,
                    QuantitySold = (float)order.Amount,
                    StockLeft = currentStock
                };

                orderDataList.Add(orderData);
            }

            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "orders_data.csv");
            SaveToCsv(orderDataList, filePath);

            PredictedDate = PredictStockDepletionDate(orderDataList, filePath, initialStock);

            return Page();
        }

        private void SaveToCsv(List<OrderData> orderDataList, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Fruit,Date,QuantitySold,StockLeft");

                foreach (var orderData in orderDataList)
                {
                    var dateStr = orderData.Date.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    var quantitySoldStr = orderData.QuantitySold.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    var stockLeftStr = orderData.StockLeft.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    writer.WriteLine($"{orderData.Fruit},{dateStr},{quantitySoldStr},{stockLeftStr}");
                }
            }
        }

        private DateTime PredictStockDepletionDate(List<OrderData> orderDataList, string filePath, decimal initialStock)
        {
            IDataView dataView = _mlContext.Data.LoadFromTextFile<OrderData>(filePath, separatorChar: ',', hasHeader: true);

            var dataProcessPipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "StockLeft")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "FruitEncoded", inputColumnName: "Fruit"))
                .Append(_mlContext.Transforms.Concatenate("Features", "Date", "QuantitySold", "FruitEncoded"));

            var trainer = _mlContext.Regression.Trainers.LbfgsPoissonRegression();
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            var trainedModel = trainingPipeline.Fit(dataView);

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<OrderData, StockPrediction>(trainedModel);

            var lastOrder = orderDataList.OrderByDescending(o => o.Date).First();
            var daysToDepletion = ((float)initialStock - lastOrder.StockLeft) / lastOrder.QuantitySold;
            var predictedDepletionDate = new DateTime(2024, 6, 1).AddDays(lastOrder.Date + daysToDepletion);

            return predictedDepletionDate;
        }
    }
}
