using System.ComponentModel.DataAnnotations;


namespace StoreManagement.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public  string UserFullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }


        //Navigation
        public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

        public ICollection<SupplyDocument> SupplyDocuments { get; set; } = new List<SupplyDocument>();

    }
}
