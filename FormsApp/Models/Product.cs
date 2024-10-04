using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace FormsApp.Models
{
    public class Product
    {
        [DisplayName("Ürün ID")]
        public int ProductId { get; set; }
        [Required]
        [DisplayName("Ürün Adı")]
        public string? Name { get; set; }
        [DisplayName("Fiyat")]
        [Required]
        public decimal? Price { get; set; }
        [DisplayName("Resim")]
        public string? Image { get; set; }
        [DisplayName("Aktif")]
        [Required]
        public bool IsActive { get; set; }
        [DisplayName("Kategori ID")]
        [Required]
        public int? CategoryId { get; set; }
    }
}
