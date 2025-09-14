using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProudctManagementDashboard.Api.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
