using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebApplication_BasicAuth
{
    // eğer ki bir authentication handler yazıyorsak bunu "AuthenticationHandler" class'ından türetip bunu da AuthenticationSchemeOptions tipinden bir class olarak üretmemeiz gerekyior. Daha sonrasında Abstract class'ı implemente ediyoruz. Ek olarak Constructor'ı da eklemek gerekiyor.
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        // Method 'Task' döndürüyorsa Asenkrondur başına 'async' ibaresini ekledik.
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // öncelikle istek yapılan endpoint'i elde etmemiz gerekiyor.

            var endpoint = Context.GetEndpoint();

            // action anonim e açık ise bypass yani [Allowanonymous] methodu açık ise çalışır değilse çalışmaz.
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.NoResult();
            }

            if (Request.Headers.ContainsKey("Authorization") == false)
            {
                return AuthenticateResult.Fail("Kimlik bilgilerini içeren authorization header bulunmuyor");
            }

            // Reques header da; Authorization "Basic ersdewr=" (karışık bir şeyler var)
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            // Authorization da Basic'den sonra gelen parametreyi aldık.
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':',2); // username:password ü bana dizi olarak döndür dedik
            var username = credentials[0];
            var password = credentials[1];

            bool result = (username=="test"&&password=="123123")? true: false;

            if (result)
            {
                // Eğer ki bilgiler doğru girildiyse, kullanıcının ticket'ini oluşturmam gerekiyor.
                // Öncelikle kullanıcının haklarının bulunduğu bir Claim list oluşturuyoruz.
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "99"),
                    new Claim(ClaimTypes.Name,username),
                    new Claim(ClaimTypes.Role,"user")
                };

                var identity = new ClaimsIdentity(claims,Scheme.Name); // Kimlik oluşturup içerisine claim leri doldurup şema adımızı eklememiz gerekiyor.
                var principal = new ClaimsPrincipal(identity); // bir principal oluştururuz bu principal bizim oluşturduğumuz identity'i giriş değeri olarak ister.

                var ticket = new AuthenticationTicket(principal,Scheme.Name); // Yetkilendirme (authentication) bileti oluşturulur. Bu bilete, önceki adımlarda oluşturulan kullanıcı temsilcisi (principal) ve kullanılan kimlik şemasının adı eklenir.


                return AuthenticateResult.Success(ticket); // Kimlik doğrulama işlemi başarılı olduğunda bir "AuthenticateResult" dönüş değeri oluşturulur ve içerisine oluşturulan yetkilendirme bileti (ticket) eklenir. Bu, kimlik doğrulama sürecinin başarılı olduğunu gösterir.

            }
            else
            {
                return AuthenticateResult.Fail("Kullanıcı adı ya da şifre geçersiz.");
            }
        }
    }
}
