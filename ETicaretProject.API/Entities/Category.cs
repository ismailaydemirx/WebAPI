using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretProject.API.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; } // başka entity'in class'ı olduğu için virtual olarak belirledik.

        public Category()
        {
            Products = new List<Product>();
        }
    }
}
