using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretProject.API.Entities
{
    [Table("Carts")]
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsClosed { get; set; }
        public int AccountId { get; set; }

        public virtual Account Accounts { get; set; }
        public virtual List<CartProduct> CartProducts { get; set; }

        // Liste olan tablolarda Contructor ile bu listelerin new'lenmesini sağlayalım.

        public Cart()
        {
            CartProducts = new List<CartProduct>();
        }
    }
}
