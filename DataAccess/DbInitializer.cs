using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;

        }


        public void Initialize()
        {
            _db.Database.EnsureCreated();

            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (_db.Categories.Any())
            {
                return; //DB has been seeded
            }

            //create roles if they are not created
            //SD is a “Static Details” class we will create in Utility to hold constant strings for Roles

            _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.ShipperRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();

            //Create at least one "Super Admin" or “Admin”.  Repeat the process for other users you want to seed

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "dritsanaja@gmail.com",
                Email = "dritsanaja@gmail.com",
                FirstName = "Driti",
                LastName = "Sanaja",
                PhoneNumber = "12345678",
                StreetAddress = "Street",
                State = "UT",
                PostalCode = "84408",
                City = "Ogden"
            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "dritsanaja@gmail.com");

            _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();

            var Categories = new List<Category>
 {

 new Category { Name = "Non-Alcoholic Beverages", DisplayOrder = 1 },
 new Category { Name = "Wine", DisplayOrder = 2 },
 new Category { Name = "Snacks", DisplayOrder = 3 }
 };

            foreach (var c in Categories)
            {
                _db.Categories.Add(c);
            }
            _db.SaveChanges();

            var Manufacturers = new List<Manufacturer>
 {
 new Manufacturer { Name = "Coca Cola" },
 new Manufacturer { Name = "Yellow Tail"},
 new Manufacturer { Name = "Trinchero Family Estates" },
 new Manufacturer { Name = "Frito Lay" }
 };

            foreach (var m in Manufacturers)
            {
                _db.Manufacturers.Add(m);
            }
            _db.SaveChanges();

            var Products = new List<Product>
 {
 new Product {
         Name = "Coca Cola Classic",
         Description = "The primary taste of Coca-Cola is thought to come from vanilla and cinnamon, with trace amounts of essential oils, and spices such as nutmeg.",
         ListPrice = 1.99,
         UnitPrice = 1.49,
         HalfDozenPrice = 1.24,
         DozenPrice = .99,
         Size = "12 oz",
         UPC = "4894034",
         ImageUrl = "/images/products/Coke.jpg",
         CategoryId = 1,
         ManufacturerId =1
     },
     new Product
     {

         Name = "Yellow Tail Shiraz",
         Description = "<p>The Yellow Tail Shiraz has a deep red color with bright purple hues, characteristic of fine young wines. It displays impressive <strong>spice, licorice, and black currant aromas</strong>. The palate is perfectly balanced with soft tannins and fine French Oak, further complemented by ripe fruit flavors.</p>",
         ListPrice = 9.99,
         UnitPrice = 8.99,
         HalfDozenPrice = 7.99,
         DozenPrice = 6.99,
         Size = "750 ml",
         UPC = "031259008943",
         ImageUrl = "/images/products/YellowTail.png",
         CategoryId = 2,
         ManufacturerId = 2
     },
     new Product
     {

         Name = "Menage a Trois Merlot",
         Description = "Menage a Trois California Red Blend exposes the fresh, ripe, jam-like fruit that is the calling card of California wine. Forward, spicy and soft, this delicious dalliance makes the perfect accompaniment for grilled meats or chicken.",
         ListPrice = 12.99,
         UnitPrice = 11.49,
         HalfDozenPrice = 10.75,
         DozenPrice = 9.99,
         Size = "750 ml",
         UPC = "099988071096",
         ImageUrl = "/images/products/menage.jpg",
         CategoryId = 2,
         ManufacturerId = 3
     },
     new Product
     {

         Name = "Doritos",
         Description = "The chip that packs a flavorful punch with the classic crunch. Boldly seasoned with three cheeses, tomatoes, onions, and a savory blend of spices. Indulge yourself or share with large gatherings",
         ListPrice = 1.99,
         UnitPrice = 1.49,
         HalfDozenPrice = 1.05,
         DozenPrice = .69,
         Size = "1.75 oz",
         UPC = "028400443753",
         ImageUrl = "/images/products/doritos.jpg",
         CategoryId = 3,
         ManufacturerId = 4
     },
     new Product
     {

         Name = "Cheetos",
         Description = "The fun, crunchy snack that is made with real cheese. Packed with flavor that satisfies. Always a crowd favorite.",
         ListPrice = 1.99,
         UnitPrice = 1.49,
         HalfDozenPrice = 1.05,
         DozenPrice = .69,
         Size = "2 oz",
         UPC = "028400443661",
         ImageUrl = "/images/products/cheetos.jpg",
         CategoryId = 3,
         ManufacturerId = 4
     }
 };

            foreach (var p in Products)
            {
                _db.Products.Add(p);
            }
            _db.SaveChanges();



            _db.SaveChanges();
        }
    }
}


