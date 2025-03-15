using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace product_manager.Models
{
    [Table("Pro")]
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2")]
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty ;
        public int StockQuantity { get; set;}
        [StringLength(100)]
        public string? Category { get; set; }
    }
}
