namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			throw new NotImplementedException();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			throw new NotImplementedException();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			StringBuilder sb = new StringBuilder();

			List<Purchase> validPurchases = new List<Purchase>();

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(
				ImportPurchaseDto[]),
				new XmlRootAttribute("Purshases"));

			using (StringReader stringReader = new StringReader(xmlString))
			{
				ImportPurchaseDto[] purchaseDtos =
		  (ImportPurchaseDto[])xmlSerializer.Deserialize(stringReader);

				foreach (ImportPurchaseDto purchaseDto in purchaseDtos)
				{
					if (!IsValid(purchaseDto))
					{
						sb.AppendLine("Invalid Data");
						continue;
					}

					DateTime purchaseDate;
					bool isProjectOpenDateValid =
						DateTime.TryParseExact(
							purchaseDto.Date, "dd/MM/yyyy"
							, CultureInfo.InvariantCulture
							, DateTimeStyles.None
							, out purchaseDate);

					if (!isProjectOpenDateValid)
					{
						sb.AppendLine("Invalid Data");
						continue;
					}

					Purchase pur = new Purchase
					{
						//Type=null  ,
						ProductKey=purchaseDto.ProductKey,
						Card= null ,
						Date = purchaseDate
					};

					validPurchases.Add(pur);
					sb.AppendLine($"Imported {null} for {null}!");
				}

				context.Purchases.AddRange(validPurchases);
				context.SaveChanges();
			}
			return sb.ToString().TrimEnd();
		}
		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}