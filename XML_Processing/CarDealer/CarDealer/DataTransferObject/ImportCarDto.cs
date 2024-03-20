using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObject
{
    [XmlType("Car")]
    public class ImportCarDto
    {
        [XmlElement("make")]
        public string Make { get; set; }


        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }


        [XmlArray("parts")]
        public ImportCarPartDto[] Parts { get; set; }
    }

    [XmlType("partId")]
    public class ImportCarPartDto
    {
        [XmlElement("id")]
        public int Id { get; set; }
    }
}
