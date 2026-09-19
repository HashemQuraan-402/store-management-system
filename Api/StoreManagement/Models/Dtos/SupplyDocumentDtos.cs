using StoreManagement.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Dtos
{
    public class SupplyDocumentListDto
    {
        public int SupplyDocumentId { get; set; }
        public string SupplyDocumentName { get; set; } = string.Empty;
        public string supplyDocumentSubject { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime createdDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
    }

    public class CreateSupplyDocumentDto
    {
        [Required]
        [MaxLength(150)]
        public string SupplyDocumentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string SupplyDocumentSubject { get; set; } = string.Empty;

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ItemId { get; set; }

    }
}
