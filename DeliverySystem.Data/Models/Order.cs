using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverySystem.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        [Required]
        public int FruitId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        public decimal FinalPrice { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        [ForeignKey("FruitId")]
        public Fruit? Fruit { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public DateTime OrderDate { get; set; }

        public bool IsDelivered { get; set; }

        public Delivery? Delivery { get; set; }
    }
}
