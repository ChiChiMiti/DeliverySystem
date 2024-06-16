using Microsoft.ML.Data;

namespace DeliverySystem.Data.Models
{
    public class OrderData
    {
        [LoadColumn(0)]
        public string Fruit { get; set; }

        [LoadColumn(1)]
        public float Date { get; set; }

        [LoadColumn(2)]
        public float QuantitySold { get; set; }

        [LoadColumn(3)]
        public float StockLeft { get; set; }
    }
}
