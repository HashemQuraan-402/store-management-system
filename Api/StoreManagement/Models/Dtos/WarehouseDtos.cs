using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Dtos
{
    public class WarehouseListDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string? WarehouseDescription { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDateAndTime { get; set; }
        public int ItemsCount { get; set; }
    }


    // could be user for the item in supplydocument page
    public class DropListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }


    public class ItemDto 
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public int Quantity { get; set; }
        public int WarehouseId { get; set; }
    }

    public class ItemSearchResultDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public int Quantity { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
    }

    // write models

    public class CreateItemDto
    {
        [Required]
        [MaxLength(150)]
        public string ItemName { get; set; } = string.Empty;

        
        [MaxLength(500)]
        public string? ItemDescription { get; set; } 

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }



    public class CreateWarehouseDto
    {
        [Required]
        [MaxLength(150)]
        public string WarehouseName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? WarehouseDescription { get; set; }

        public List<CreateItemDto> Items { get; set; } = new();

    }


    
}
