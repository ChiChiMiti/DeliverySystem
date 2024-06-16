using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliverySystem.Data.Models
{
    public class CourierLocation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
