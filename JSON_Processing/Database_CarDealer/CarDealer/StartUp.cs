using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext db = new CarDealerContext();

            //string inputJson = File.ReadAllText("../../../Datasets/sales.json");

            string result = GetOrderedCustomers(db);

            Console.WriteLine(result);

            //  ResetDatabase(db);
        }

        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();

            Console.WriteLine("Database was successfully deleted");

            db.Database.EnsureCreated();

            Console.WriteLine("Database was successfully created");
        }




        //ZAD--10
        public static string ImportSuppliers(
                CarDealerContext context, string inputJson)
        {
            Supplier[] suppliers = JsonConvert
                .DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        //ZAD--11
        public static string ImportParts(
        CarDealerContext context, string inputJson)
        {

            Part[] parts = JsonConvert
                 .DeserializeObject<Part[]>(inputJson)
                 .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                 .ToArray();

                context.Parts.AddRange(parts);
                context.SaveChanges();

                return $"Successfully imported {parts.Length}.";


            

        }


        //ZAD--12
        public static string ImportCars(
            CarDealerContext context, string inputJson)
        {

            var carsDto = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            var cars = new List<Car>();

            var carParts = new List<PartCar>();
            foreach (var carDto in carsDto)
            {
                var car = new Car()
                {
                    Make=carDto.Make,
                    Model=carDto.Model,
                    TravelledDistance=carDto.TravelledDistance
                };

                
                foreach (var part in carDto.PartsId.Distinct())
                {
                    var carPart = new PartCar()
                    {
                        PartId = part,
                        Car = car
                    };
                    carParts.Add(carPart);
                }
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(carParts);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }


        //ZAD--13
        public static string ImportCustomers(
            CarDealerContext context, string inputJson)
        {
            Customer[] customers = JsonConvert
                .DeserializeObject<Customer[]>(inputJson);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";

        }

        //ZAD--14
        public static string ImportSales(
            CarDealerContext context, string inputJson)
        {
            Sale[] sales = JsonConvert
                .DeserializeObject<Sale[]>(inputJson);

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        //ZaD--15

        public static string GetOrderedCustomers(
                              CarDealerContext context)
        {
            var custumers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenByDescending(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
               // .OrderBy(c => c.BirthDate)
                //.ThenByDescending(c => c.IsYoungDriver)
                .ToArray();

            string json = JsonConvert.SerializeObject(
                custumers,Formatting.Indented);

            return json;
        }

        ///ZAD--19
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Include(c => c.Sales)
                .ThenInclude(s => s.Car)
                .ThenInclude(c => c.PartCars)
                .ThenInclude(pc => pc.Part)
                .Where(c => c.Sales.Any(s => s.CarId != null))
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(s => s.CarId != null),
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToArray();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        //ZAD--20

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },

                    customerName = x.Customer.Name,
                    Discount = $"{x.Discount:F2}",
                    price = $"{x.Car.PartCars.Sum(y => y.Part.Price):F2}",
                    priceWithDiscount = $"{x.Car.PartCars.Sum(y => y.Part.Price) - (x.Car.PartCars.Sum(y => y.Part.Price) * (x.Discount / 100)):F2}",
                })
                .ToList();

            string json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }

        //ZAD--20


    }

}