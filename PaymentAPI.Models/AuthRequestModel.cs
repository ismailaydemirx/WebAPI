using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Models
{
    public class AuthRequestModel
    {
        [Required(ErrorMessage = "Bu alan boş geçilemez.")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Kullanıcı adı minimum 3 karakter, maximum 25 karakterden oluşmalıdır.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Bu alan boş geçilemez.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Şifre minimum 6 karakter, maximum 16 karakterden oluşmalıdır.")]
        public string Password { get; set; }
    }
}
