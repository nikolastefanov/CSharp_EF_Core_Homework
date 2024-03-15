using ProductShop.Data;
using ProductShop.Dtos.Import;
using System.IO;
using System.Xml;
using XmlFacade;
using System;
using System.Collections.Generic;
using ProductShop.Models;
using System.Linq;
using ProductShop.Dtos.Export;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //  context.Database.EnsureDeleted();
            //  context.Database.EnsureCreated();
            //
            //  var userXml = File.ReadAllText("../../../Datasets/users.xml");
            //  var result = ImportUsers(context, userXml);
            //  Console.WriteLine(result);
            //
            //
            //  var userXmlProduct = File.ReadAllText("../../../Datasets/products.xml");
            //  var resultProduct = ImportProducts(context, userXmlProduct);
            //  Console.WriteLine(resultProduct);
            //
            //  var userXmlCategory = File.ReadAllText("../../../Datasets/categories.xml");
            //  var resultCategory = ImportCategories(context, userXmlCategory);
            //  Console.WriteLine(resultCategory);
            //
            //  var userXmlCategoryProduct = File.ReadAllText("../../../Datasets/categories-products.xml");
            //  var resultCategoryProduct = ImportCategoryProducts(context, userXmlCategoryProduct);
            //  Console.WriteLine(resultCategoryProduct);

            // var productsInRange = GetProductsInRange(context);
            // File.WriteAllText(
            //     "../../../Results/productsInRange.xml", productsInRange);



               var soldProducts = GetSoldProducts(context);
              File.WriteAllText(
                    "../../../Results/users-sold-products.xml", soldProducts);
            

            // var categoryProductsCount = GetCategoriesByProductsCount(context);
            // File.WriteAllText(
            //       "../../../Results/categories-by-products.xml", categoryProductsCount);

            var userAndProducts = GetUsersWithProducts(context);
            File.WriteAllText(
                  "../../../Results/users-and-products.xml", userAndProducts);
        }

        //Zad--1
        public static string ImportUsers(
            ProductShopContext context, string inputXml)
        {
            const string rootElem = "Users";

            var usersResult = XmlConverter.Deserializer<ImportUserDto>(
                inputXml, rootElem);

            List<User> users = new List<User>();

            foreach (var importUserDto in usersResult)
            {
                var user = new User
                {
                    FirstName = importUserDto.FirstName,
                    LastName = importUserDto.LastName,
                    Age = importUserDto.Age
                };
                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();


            return $"Successfully imported {users.Count}"; 
        }

        //ZAD--2
        public static string ImportProducts(
              ProductShopContext context, string inputXml)
        {
            const string rootElem = "Products";

            var productDtos = XmlConverter.Deserializer<ImportProductDto>(
                                 inputXml, rootElem);

            var products = productDtos.Select(p => new Product
            {
                Name = p.Name,
                Price = p.Price,
                BuyerId = p.BuyerId,
                SellerId = p.SellerId
            })
            .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();


            return $"Successfully imported {products.Length}";
            
        }

        //ZAD--3
        public static string ImportCategories(
            ProductShopContext context, string inputXml)
        {
            const string rootElem = "Categories";

            var categoriesDtos = XmlConverter.Deserializer<ImportCategoryDto>(
                inputXml, rootElem);

            List<Category> categories = new List<Category>();

            foreach (var dto in categoriesDtos)
            {
                if (dto.Name==null)
                {
                    continue;
                }
                var category = new Category
                {
                    Name = dto.Name
                };
            categories.Add(category);
            }
            context.Categories.AddRange(categories);
            context.SaveChanges();
             
            return $"Successfully imported {categories.Count}";

        }

        //ZAD--4
        public static string ImportCategoryProducts(
              ProductShopContext context, string inputXml)
        {
            const string rootElem = "CategoryProducts";

            var categoryProductDtos = XmlConverter.Deserializer<
                      ImportCategoryProductDto>(inputXml, rootElem);

            var categories = categoryProductDtos
                .Where(i => context.Categories.Any(s => s.Id == i.CategoryId) &&
                                   context.Products.Any(s => s.Id == i.ProductId))
                .Select(c => new CategoryProduct
                {
                    CategoryId = c.CategoryId,
                    ProductId = c.ProductId
                })
                .ToArray();

            context.CategoryProducts.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";

        }


        //ZAD--5
        public static string GetProductsInRange(
                         ProductShopContext context)
        {
            const string rootElem = "Products";

            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x => new ExportProductInfoDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToList();

            var result = XmlConverter.Serialize(products, rootElem);

            return result;
        }

        //ZAD--6
        public static string GetSoldProducts(
                      ProductShopContext context)
        {
            const string rootElem = "Users";

            var userWithProducts = context.Users
                .Where(u => u.ProductsSold.Any())
                .Select(x => new ExportUserSoldProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p =>
                      new UserProductDto
                      {
                          Name = p.Name,
                          Price = p.Price
                      }).ToArray()
                })
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToArray();

            var result = XmlConverter.Serialize(userWithProducts, rootElem);

            return result;
        }

        //ZAD--7
        public static string GetCategoriesByProductsCount(
                                 ProductShopContext context)
        {
            const string rootElem = "Categories";

            var categories = context.Categories
                .Select(c => new ExportCategoryDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price),
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price)
                })
                .OrderByDescending(p => p.Count)
                .ThenBy(t => t.TotalRevenue)
                .ToArray();

            var result = XmlConverter.Serialize(categories, rootElem);

            return result;
        }

        //ZAD--8
      public static string GetUsersWithProducts(
                        ProductShopContext context)
        {
            var usersAndProducts = context.Users
                .ToArray()
                .Where(p => p.ProductsSold.Any())
                .Select(u => new ExportUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProduct = new ExportProdutCountDto
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(p =>
                              new ExportProductDto
                              {
                                  Name = p.Name,
                                  Price = p.Price
                              })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.SoldProduct.Count)
                .Take(10)
                .ToArray();


            var resultDto = new ExportUserCountDto
            {
                Count = context.Users.Count(p => p.ProductsSold.Any()),
                Users = usersAndProducts
            };

            var result = XmlConverter.Serialize(resultDto, "Users");

            return result;
        }
    }
}