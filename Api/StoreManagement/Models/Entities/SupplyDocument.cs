using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Entities
{
   
    public class SupplyDocument
    {
        [Key]
        public int SupplyDocumentId { get; set; }

        [Required]
        [MaxLength(150)]
        public string SupplyDocumentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string SupplyDocumentSubject { get; set; } = string.Empty;

        [Required]
        public int CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDateAndTime { get; set; } = DateTime.Now;


        [Required]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [Required]
        public int ItemId { get; set; }
        public Item? Item { get; set; }


        [Required]
        public DocumentStatus documentStatus { get; set; } = DocumentStatus.Pending;

       
    }
}
