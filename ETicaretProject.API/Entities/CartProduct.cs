using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretProject.API.Entities
{
    [Table("CartProducts")]
    public class CartProduct
    {
        [Key]
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
        public int? CartId { get; set; } // bilerek nullable yapıyoruz çünkü bunlar birbirlerine bağlı olacak ileride bir şey sildiğimizde hepsi zincirleme silinmesin diye bu önlemi en başta alıyoruz.

        public virtual Product Product { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
