using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Models
{
    public class PaymentRequestModel
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }
        [Required]
        [StringLength(40)]
        public string CardName { get; set; }
        //04/23
        [Required]
        [StringLength(5)]
        [RegularExpression(@"^\d{2}/\d{2}$")] // regex yani regularexpression ifadesi. sayı aldığımızı belirttik
        public string ExpireDate { get; set; }
        [Required]
        [StringLength(3)]
        [RegularExpression(@"^\d{3}$")]
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }
        public string Token { get; set; }
    }
}
