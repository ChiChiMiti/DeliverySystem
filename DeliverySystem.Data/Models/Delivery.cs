using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverySystem.Data.Models
{
    public class Delivery
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? Courier { get; set; }

        public bool IsDelivered { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}
