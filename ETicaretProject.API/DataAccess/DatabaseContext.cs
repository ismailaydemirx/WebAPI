using ETicaretProject.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretProject.API.DataAccess
{
    // DatabaseContext class'ımız veritabanına erişme sınıfımızdır.
    public class DatabaseContext : DbContext
    {
        // Databasecontext'i new'lerken bize options'lar gelir bu option'lar DbContext Option'larıdır. Bu optionları base class'a (yani şu an buradaki base class DbContext' oluyor) gönder ve oluşumunu sağla. DatabaseContext class'ının new'lenmesini sağla.
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        // içerisine yazacağımız property'ler de database de bulunan tablolara erişirken kullandığımız property'ler.
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }
        public DbSet<Payment> Payments { get; set; }



        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (optionsBuilder.IsConfigured == false)
        //    {
        //        optionsBuilder.UseSqlServer("Data Source=BAYDEMIRPC\\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        //        //optionsBuilder.UseLazyLoadingProxies();
        //    }
        //    base.OnConfiguring(optionsBuilder);
        //}

    }
}
