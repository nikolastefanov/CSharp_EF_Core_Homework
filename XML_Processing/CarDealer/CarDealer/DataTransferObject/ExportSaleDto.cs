using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObject
{
    [XmlType("Sale")]
    public class ExportSaleDto
    {
        [XmlElement("discount")]
        public int Discount { get; set; }

        [XmlElement("customer-name")]
        public string  CustomerName { get; set; }

        [XmlElement("price")]
        public decimal  Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }

        [XmlElement("car")]
        public ExportCarDto Car { get; set; }
    }

    [XmlType("car")]
    public class ExportCarDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string  Model { get; set; }

        [XmlElement("travelled-distance")]
        public long TraveledDistance { get; set; }
    }
}
