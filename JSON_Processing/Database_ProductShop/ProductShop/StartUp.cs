using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using System.Text;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ProductShop.Export;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();

            Mapper.Initialize(cfg => cfg.AddProfile(new ProductShopProfile()));
            QueryAndExport();

         

            //  string inputJson = File.ReadAllText("../../../Datasets/products.json");

          //  string result = GetCategoriesByProductsCount(db);
           
            //Console.WriteLine(result);

           // ResetDatabase(db);
        }
        public static void QueryAndExport()
        {
            using (ProductShopContext context = new ProductShopContext())
            {
                string result = GetUsersWithProducts(context);
                Console.WriteLine(result);
            }
        }



        private static void ResetDatabase(ProductShopContext db)
        {
            db.Database.EnsureDeleted();

            Console.WriteLine("Database was successfully deleted");

            db.Database.EnsureCreated();

            Console.WriteLine("Database was successfully created");
        }

        //ZAD--1
        public static string ImportUsers(
               ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert
                .DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";

        }


        //ZAD--2
        public static string ImportProducts(
               ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert
                .DeserializeObject<Product[]>(inputJson);
                
                context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }


        //ZAD--3
        public static string ImportCategories(
               ProductShopContext context, string inputJson)
        {
            Category[] categories = JsonConvert
                .DeserializeObject<Category[]>(inputJson);

            int count = 0;
            foreach (Category category in categories)
            {
                if (category.Name!=null)
                {
                    context.Categories.Add(category);
                    count++;
                }
            }

            context.SaveChanges();

            return $"Successfully imported {count}";


        }


        //ZAd--4
        public static string ImportCategoryProducts(
               ProductShopContext context, string inputJson)
        {
            CategoryProduct[] categoryProducts = JsonConvert
                .DeserializeObject<CategoryProduct[]>(inputJson);

            //context.CategoryProducts.AddRange(categoryProducts);

            int count = 0;
            foreach (CategoryProduct cp in categoryProducts)
            {
                
                    context.CategoryProducts.Add(cp);
                    count++;
                
            }


            context.SaveChanges();

            return $"Successfully imported {count}";

        }


        //ZAD--5

        public static string GetProductsInRange(
                          ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .ToArray();

            string json = JsonConvert.SerializeObject(
                            products, Formatting.Indented);

            return json;
                
        }

        //ZAD--6
        public static string GetSoldProducts(
                       ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                                  .Where(p => p.Buyer != null)
                                  .Select(p => new
                                  {
                                      name = p.Name,
                                      price = p.Price,
                                      buyerFirstName = p.Buyer.FirstName,
                                      buyerLastName = p.Buyer.LastName
                                  })
                                  .ToArray()
                }).ToArray();

            string json = JsonConvert.SerializeObject(users
                                       , Formatting.Indented);
            return json;
        }


        //   //ZAD--7
        //   public static string GetCategoriesByProductsCount(
        //                             ProductShopContext context)
        //   {
        //       var categories = context.Categories
        //           .Select(c => new
        //           {
        //               category = c.Name,
        //               productsCount = c.CategoryProducts.Count(),
        //               averagePrice = c.CategoryProducts.Average(cp =>
        //                 cp.Product.Price).ToString("f:2"),
        //               totalRevenue = c.CategoryProducts.Sum(cp =>
        //                 cp.Product.Price).ToString("f:2")
        //           })
        //           .OrderByDescending(c => c.productsCount)
        //           .ToArray();
        //
        //       string json = JsonConvert.SerializeObject(categories,
        //             Formatting.Indented);
        //
        //       return json;
        //   }
        //
        //  //ZAd--8
        //  public static string GetUsersWithProducts(
        //                       ProductShopContext context)
        //  {
        //      var users = context.Users
        //          .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
        //          .Select(u => new
        //          {
        //              firstName=u.FirstName,
        //              lastName = u.LastName,
        //              age = u.Age,
        //              soldProducts = new
        //              {
        //                  count = u.ProductsSold
        //                  .Count(p => p.Buyer != null),
        //                  products = u.ProductsSold
        //                  .Where(p => p.Buyer != null)
        //                  .Select(p => new
        //                  {
        //                      name = p.Name,
        //                      price = p.Price
        //                  })
        //                  .ToArray()
        //
        //              }
        //          })
        //          .OrderByDescending(u => u.soldProducts.count)
        //          .ToArray();
        //
        //      var resultObj = new
        //      {
        //          userCount = users.Length,
        //          users = users
        //      };
        //
        //      JsonSerializerSettings settings = new JsonSerializerSettings
        //      {
        //          Formatting = Formatting.Indented,
        //          NullValueHandling = NullValueHandling.Ignore
        //          
        //      };
        //
        //      string json = JsonConvert.SerializeObject(resultObj, settings);
        //
        //      return json;
        //  }
        //
        //
        //ZAD--8

        //   public static void Main(string[] args)
        //   {
        //       string json = string.Empty;
        //       var context = new ProductShopContext();
        //
        //
        //       Console.WriteLine(GetUsersWithProducts
        //           (context));
        //   }
        //
        //   public static string GetUsersWithProducts(ProductShopContext context)
        //   {
        //       DefaultContractResolver contractResolver = new DefaultContractResolver
        //       {
        //           NamingStrategy = new CamelCaseNamingStrategy()
        //       };
        //
        //       var users = context.Users
        //           .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
        //           .Include(u => u.ProductsSold)
        //           .Select(u => new
        //           {
        //               u.FirstName,
        //               u.LastName,
        //               u.Age,
        //               soldProducts = new
        //               {
        //                   count = u.ProductsSold
        //                   .Where(p => p.BuyerId != null)
        //                   .Count(),
        //                   products = u.ProductsSold
        //                   .Where(p => p.BuyerId != null)
        //                   .Select(p => new
        //                   {
        //                       p.Name,
        //                       p.Price
        //                   })
        //                   .ToList()
        //               }
        //
        //           })
        //           .OrderByDescending(u => u.soldProducts.count)
        //           .ToList();
        //
        //
        //       var withUserCount = new
        //       {
        //           usersCount = users.Count,
        //           users,
        //
        //       };
        //
        //       var json = JsonConvert.SerializeObject(withUserCount, new JsonSerializerSettings
        //       {
        //           ContractResolver = contractResolver,
        //           Formatting = Formatting.Indented,
        //           NullValueHandling = NullValueHandling.Ignore
        //       });
        //       return json;
        //   }
        //

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(x => new
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = $"{x.CategoryProducts.Average(c => c.Product.Price):F2}",
                    TotalRevenue = $"{x.CategoryProducts.Sum(c => c.Product.Price)}"
                })
                .ToList();

            string json = JsonConvert.SerializeObject(categories,
                new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    },

                    Formatting = Formatting.Indented
                }
            );

            return json;
        }


        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                .ProjectTo<UserDto>()
                .ToList();

            var objectToSerialize = Mapper.Map<UsersAndProductsDto>(users);

            string json = JsonConvert.SerializeObject(objectToSerialize, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },

                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }
    }
}