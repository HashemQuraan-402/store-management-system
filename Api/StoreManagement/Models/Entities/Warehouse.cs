using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Entities
{
    
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }

        [Required]
        [MaxLength(150)]
        public string WarehouseName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? WarehouseDescription { get; set; }

        [Required]
        public int CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDateAndTime { get; set; } = DateTime.Now;

        
        // Navigation
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<SupplyDocument> SupplyDocuments { get; set; } = new List<SupplyDocument>();
    }
}
