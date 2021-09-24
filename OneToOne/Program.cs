using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OneToOne
{

    //Entity Classes
    //Product(Id,Name,Price) => Product(Id,Name,Price)

    class ShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }   //Db set e entity class larımızı aldık liste olarak tanımladık
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory =
            LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  // hangi database yada provider ile çalışacağımızı belirtiyoruz
        {
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)  // Bu satırda ilgili oluşturduğumuz metodu çağırmış oluruz
                                                    // .UseSqlServer(@"Data Source=TUGCEERASLAN-PC;Initial Catalog=ShopDb;Integrated Security=SSPI"); // Yolu bu şekilde vermezsek tabloyu bulamıyor
               .UseMySql(@"server=localhost;port=3306;database=ShopDb2;user=root;password=mysql1234");
               //.UseSqlServer(@"Data Source=TUGCEERASLAN-PC;Initial Catalog=ShopDb;Integrated Security=SSPI"); // Yolu bu şekilde vermezsek tabloyu bulamıyor
        }
    }

    // One to Many =>
    // mysql de veritabanı oluşturmak için önce migrations oluştur
    // dotnet ef migrations add OneToManyRelation cümleciği ile 
    // sonra oluşan migrationsu veritabanına aktar dotnet ef database update cümleciği ile 

    class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Customer Customer { get; set; }  // navigation proporty (Customer ile ilişkilendirme)
        public List<Address> Addresses { get; set; } //Bir kişinin birden fazla adderesi olabilir
    }

    class Customer
    {
        public int Id { get; set; }
        public string IdentityNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public User User { get; set; }  //navigation proporty (User ile ilişkilendirme)
        public int UserId { get; set; }  // bir kere kullanılacak. Unique olarak işaretlenecek
    }

    class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxNumber { get; set; }
    }


    class Address    // User ile Address tablosu arasında bire çok bir ilişki var
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public User User { get; set; }  //navigation proporty
        public int UserId { get; set; }  // int=> null,1,2,3

    }

    // One to One
    // Many to Many

    class Product
    {

        // Primary key(Id,<type_name>Id)
        public int Id { get; set; }


        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }


    class Category
    {
        public int Id { get; set; }


        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
    }


    class Program
    {
        static void GetAllProducts()
        {
            using (var context = new ShopContext())
            {
                // var products = context.Products;  //Products listesinin bir referansı olan context i alırız
                var products = context
                    .Products
                    .Select(p =>   // Bir kolon seçmek istersem bu satıra ihtiyacım var. Ama onun dışında .Products.ToList
                      new {
                          p.Name,
                          p.Price
                      })
                    .ToList();  // Gelen Collection ı bir listeye çeviridğimizde yani ToList() yaptığımızda sorgu olarak gider

                foreach (var p in products)  // Gelen sorguyu ekrana yazdıralım
                {
                    Console.WriteLine($"name:{p.Name}, price:{p.Price}");
                }

            }
        }

        static void GetProductById(int id)
        {
            using (var context = new ShopContext())
            {
                var result = context.Products
                    .Where(p => p.Id == id)
                      .Select(p =>
                      new {
                          p.Name,
                          p.Price
                      })
                    .FirstOrDefault();
                // Gönderdiğim Id bilgisi ile veritabanında bir tane kayıt var. Sonuç liste değil de product olsun dersem
                // FirstOrDefault(); yaparım . İlgili kaydı bulamazsa null değer gönderir

                Console.WriteLine($"name:{result.Name}, price:{result.Price}");

            }
        }


        static void GetProductByName(string name)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products
                    .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                      .Select(p =>
                      new {
                          p.Name,
                          p.Price
                      })
                    .ToList();
                // Gönderdiğim Id bilgisi ile veritabanında bir tane kayıt var. Sonuç liste değil de product olsun dersem
                // FirstOrDefault(); yaparım . İlgili kaydı bulamazsa null değer gönderir

                foreach (var p in products)
                {
                    Console.WriteLine($"name:{p.Name}, price:{p.Price}");
                }



            }
        }


        static void Main(string[] args)
        {
            //InsertUsers();
            //InsertAddresses();

            using (var db = new ShopContext())
            {

                //var user = db.Users.FirstOrDefault(t => t.Id == 2);
                //try
                //{
                //    if (user != null)
                //    {
                //        user.Addresses = new List<Address>();
                //        user.Addresses.AddRange(
                //            new List<Address>
                //            {
                //            new Address { Fullname = "Fatma Kaya", Title = "İş adresi 1", Body = "Istanbul"},
                //            new Address { Fullname = "Fatma Kaya", Title = "İş adresi 2", Body = "Istanbul"},
                //            new Address { Fullname = "Fatma Kaya", Title = "İş adresi 3", Body = "Istanbul"}
                //            }
                //            );
                //    }
                //    db.SaveChanges();
                //}
                //catch (Exception ex)
                //{
                //    Console.Write("Error info: " + ex.InnerException.Message);
                //}
                //var customer = new Customer()
                //{
                //    IdentityNumber = "123456",
                //    FirstName="Tuğçe",
                //    LastName="Eraslan",
                //    User=db.Users.FirstOrDefault(t=>t.Id==1)
                //};

                //db.Customers.Add(customer);
                //db.SaveChanges();

                var user = new User()
                {
                    Username="Deneme",
                    Email="Deneme@gmail.com",
                    Customer=new Customer()
                    {
                         IdentityNumber="987654",
                         FirstName= "Deneme",
                         LastName="Deneme",      
                         
                    }
                };

                db.Users.Add(user);
                db.SaveChanges();
            }

        }

        static void InsertUsers()
        {
            var users = new List<User>()
            {
                new User { Username = "Tugce Eraslan", Email = "tugceeraslan34@gmail.com" },
                new User { Username = "Ayşe Ay", Email = "xxxxxxxx@gmail.com" },
                new User { Username = "Fatma Kaya", Email = "yyyyyyy@gmail.com" },
                new User { Username = "Ali Demir", Email = "zzzzzzz@gmail.com" },
        };

            using (var db = new ShopContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();
            }
        }

        static void InsertAddresses()
        {
            var addreses = new List<Address>()
            {
                new Address { Fullname = "Tugce Eraslan", Title="Ev adresi", Body="Istanbul",UserId=1 },
                new Address { Fullname = "Tugce Eraslan", Title = "İş adresi", Body ="Istanbul", UserId = 1 },
                new Address { Fullname = "Ayşe Ay", Title = "Ev adresi", Body = "Ankara", UserId = 3 },
                new Address { Fullname = "Ayşe Ay", Title = "İş adresi", Body = "Ankara", UserId = 3 },
                new Address { Fullname = "Fatma Kaya", Title = "İş adresi", Body = "Istanbul", UserId = 2 },
                new Address { Fullname = "Ali Demir", Title = "İş adresi", Body = "Istanbul", UserId = 4 },
        };

            using (var db = new ShopContext())
            {
                db.Addresses.AddRange(addreses);
                db.SaveChanges();
            }
        }
        static void AddProducts()
        {
            try
            {
                using (var db = new ShopContext())  // db nesnesini using içinde kullandım ki işimiz bittikten sonra bellekten silinsin
                {
                    var products = new List<Product>
                {
                    new Product { Name = "Samsung S6", Price = 3000 },
                    new Product { Name = "Samsung S7", Price = 4000},
                    new Product { Name = "Samsung S8", Price = 5000},
                    new Product { Name = "Samsung S9", Price = 6000}

            };
                    //foreach (var product in products)
                    //{
                    //    db.Products.Add(product); // Add tek bir elemanı ekler
                    //}

                    db.Products.AddRange(products);  //AddRange bir listeyi ekler

                    db.SaveChanges();

                    Console.WriteLine("Veriler eklendi.");


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }

        static void AddProduct()
        {
            try
            {
                using (var db = new ShopContext())  // db nesnesini using içinde kullandım ki işimiz bittikten sonra bellekten silinsin
                {
                    var p = new Product { Name = "Samsung S10", Price = 8000 };

                    db.Products.Add(p);  //Add tek bir eleman ekler

                    db.SaveChanges();

                    Console.WriteLine("Veri eklendi.");


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }

        }

        static void UpdateProduct()
        {
            using (var db = new ShopContext())
            {
                var p = db
                    .Products
                    .Where(t => t.Id == 1)
                    .FirstOrDefault();

                if (p != null)
                {
                    p.Price = 2400;
                    db.Products.Update(p);  // Tek bir entity i update edeceksem Update metodu olacak.
                                            // Bir liste olacaksa update edilecek UpdateRange metodu alacak
                    db.SaveChanges();
                }
            }

        }

        static void DeleteProduct(int id)
        {
            using (var db = new ShopContext())
            {
                var eraslan = new Product() { Id = 6 };
                db.Entry(eraslan).State = EntityState.Deleted;      //db.Products.Remove(eraslan);
                db.SaveChanges();

            }

        }

    }
}
