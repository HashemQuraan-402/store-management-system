using System.ComponentModel.DataAnnotations;
using System.Data;

namespace StoreManagement.Models.Entities
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(150)]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ItemDescription { get; set; }

        [Required]
        [Range(0,int.MaxValue,ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        // this is a forgin key to the warehouse table
        [Required]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public ICollection<SupplyDocument> supplyDocuments { get; set; } = new List<SupplyDocument>();
    }
}
