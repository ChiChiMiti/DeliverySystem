using Microsoft.ML.Data;

namespace DeliverySystem.Data.Models
{
    public class StockPrediction
    {
        [ColumnName("Score")]
        public float StockLeft { get; set; }
    }
}
