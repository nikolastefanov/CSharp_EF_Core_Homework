using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DataTransferObject;
using CarDealer.Models;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using XmlFacade;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            var result = ImportSuppliers(context, suppliersXml);
            Console.WriteLine(result);


            var partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            var resultParts = ImportParts(context, partsXml);
            Console.WriteLine(resultParts);


            var carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            var resultCars = ImportCars(context, carsXml);
            Console.WriteLine(resultCars);

            //    var customerXml = File.ReadAllText("../../../Datasets/customers.xml");
            //    var resultCustomer = ImportCustomers(context, customerXml);
            //    Console.WriteLine(resultCustomer);
        }

        //ZAD--9
        public static string ImportSuppliers(
                     CarDealerContext context, string inputXml)
        {
            var suppliersDtos = XmlConverter.Deserializer<ImportSupplierDto>(
                inputXml, "Suppliers");

            var result = suppliersDtos
                .Select(s => new Supplier
                {
                    Name = s.Name,
                    IsImporter = s.IsImporter
                })
                .ToArray();

            context.Suppliers.AddRange(result);
            context.SaveChanges();
            return $"Successfully imported {result.Length}";

        }

        //ZAD--10
        public static string ImportParts(
            CarDealerContext context, string inputXml)
        {
            var partsDtos = XmlConverter.Deserializer<ImportPartDto>(
                inputXml, "Parts");

            var parts = partsDtos
                .Where(i => context.Suppliers.Any(s => s.Id == i.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Length}";
        }

        //ZAD--11


        //   public static string ImportCars(
        //       CarDealerContext context, string inputXml)
        //   {
        //       var carsDtos = XmlConverter.Deserializer<
        //           ImportCarDto>(inputXml, "Cars");
        //
        //       List<Car> cars = new List<Car>();
        //
        //       foreach (var carDto in carsDtos)
        //       {
        //           var uniqueParts = carDto.Parts.Select(s => s.Id)
        //               .Distinct().ToArray();
        //
        //           var realParts = uniqueParts.Where(id =>
        //             context.Parts.Any(i => i.Id == id)).ToArray();
        //
        //           var car = new Car
        //           {
        //               Make = carDto.Make,
        //               Model = carDto.Model,
        //               TravelledDistance = carDto.TraveledDistance,
        //
        //
        //               PartCars = realParts.Select(id => new PartCar
        //               {
        //                   PartId = id
        //               }).ToArray()
        //           };
        //
        //           cars.Add(car);
        //       }
        //
        //       context.Cars.AddRange(cars);
        //       context.SaveChanges();
        //
        //       return $"Successfully imported {cars.Count}";
        //   }


        public static string ImportCars(
            CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]),
                new XmlRootAttribute("Cars"));

            ImportCarDto[] carDtos;

            using (var reader = new StringReader(inputXml))
            {
                carDtos = (ImportCarDto[])xmlSerializer
                    .Deserialize(reader);
            }

            List<Car> cars = new List<Car>();
            List<PartCar> partCars = new List<PartCar>();

            foreach (var carDto in carDtos)
            {
                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };
                var parts = carDto.Parts
                    .Where(pdto => context.Parts.Any(p => p.Id == pdto.Id))
                    .Select(p => p.Id)
                    .Distinct();

                foreach (var partId in parts)
                {
                    var partCar = new PartCar
                    {
                        PartId = partId,
                        Car = car
                    };
                    partCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);

            context.PartCars.AddRange(partCars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}";

        }


        //Zad--12
        public static string ImportCustomers(
            CarDealerContext context, string inputXml)
        {

            var customerDtos = XmlConverter.Deserializer<
                ImportCustomerDto>(inputXml, "Customer");

            var customers = customerDtos
                .Select(x => new Customer
                {
                    Name = x.Name,
                    IsYoungDriver = x.IsYoungDriver,
                    BirthDate = DateTime.Parse(x.BirthDate)
                })
                .ToArray();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";

        }

        //ZAd--13
        public static string ImportSales(
            CarDealerContext context, string inputXml)
        {
            var salesDtos = XmlConverter.Deserializer<ImportSaleDto>(
                inputXml, "Sales");

            var sales = salesDtos
                .Where(i => context.Cars.Any(x => x.Id == i.CarId))
                .Select(c => new Sale
                {
                    CarId = c.CarId,
                    CustomerId = c.CustomerId,
                    Discount = c.Discount

                })
                .ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        //ZAD--16
        public static string GetLocalSuppliers(
            CarDealerContext context)
        {

            StringBuilder sb = new StringBuilder();
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<ExportLocalSuppliersDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(
                ExportLocalSuppliersDto[]), new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            using (var writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, suppliers, namespaces);


            }

            return sb.ToString().TrimEnd();
        }

        //ZAD--19
        //   public static string GetSalesWithAppliedDiscount(
        //                               CarDealerContext context)
        //   {
        //       var result = context.Sales
        //           .Select(s => new ExportSaleDto
        //           {
        //               Car = new ExportCarDto
        //               {
        //                   Make = s.Car.Make,
        //                   Model = s.Car.Model,
        //                   TraveledDistance = s.Car.TravelledDistance
        //               },
        //               Discount = s.Discount,
        //               CustomerName = s.Customer.Name,
        //               Price = s.Car.PartCars.Sum(p => p.Part.Price),
        //
        //               PriceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price) - s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100
        //           })
        //           .ToArray();
        //
        //       var xmlResult = XmlConverter.Serialize(result, "sales");
        //
        //       return xmlResult;
        //   }
        //      public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        //      {
        //          var salesWithDiscount = context.Sales
        //              .Select(s => new ExportSalesDiscount()
        //              {
        //                  CustomerName = s.Customer.Name,
        //                  Price = s.Car.PartCars.Sum(pc => pc.Part.Price).ToString(),
        //                  Discount = s.Discount.ToString(),
        //                  PriceWithDiscount = (s.Car.PartCars.Sum(pc => pc.Part.Price) - (s.Car.PartCars.Sum(pc => pc.Part.Price) * (s.Discount / 100))).ToString(),
        //                  CarDto = new ExportCarAtributeDto()
        //                  {
        //                      Make = s.Car.Make,
        //                      Model = s.Car.Model,
        //                      TravelledDistance = s.Car.TravelledDistance
        //                  }
        //              }).ToArray();
        //
        //          var sb = new StringBuilder();
        //
        //          var serializer = new XmlSerializer(typeof(ExportSalesDiscount[]), new XmlRootAttribute("sales"));
        //
        //          var ns = new XmlSerializerNamespaces();
        //          ns.Add("", "");
        //
        //          using (var writer = new StringWriter(sb))
        //          {
        //              serializer.Serialize(writer, salesWithDiscount, ns);
        //          }
        //
        //          return sb.ToString().Trim();
        //      }
        //
        //
        //
        //      [XmlType("sale")]
        //      public class ExportSalesDiscount
        //      {
        //          [XmlElement("car")]
        //          public ExportCarAtributeDto CarDto { get; set; }
        //
        //          [XmlElement("discount")]
        //          public string Discount { get; set; }
        //
        //          [XmlElement("customer-name")]
        //          public string CustomerName { get; set; }
        //
        //          [XmlElement("price")]
        //          public string Price { get; set; }
        //
        //          [XmlElement("price-with-discount")]
        //          public string PriceWithDiscount { get; set; }
        //      }
        //  }
        //
        //
        //
        //
        //  [XmlType("car")]
        //  public class ExportCarAtributeDto
        //  {
        //     [XmlAttribute("make")]
        //     public string Make { get; set; }
        //
        //     [XmlAttribute("model")]
        //     public string Model { get; set; }
        //
        //     [XmlAttribute("travelled-distance")]
        //     public long TravelledDistance { get; set; }
        // }
        //
    }
}