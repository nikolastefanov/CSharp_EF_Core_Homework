using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.ExportResults
{


         [XmlType("Game")]
    public class ExportUserPurchaseGameDto
    {
        [XmlAttribute("title")]
        public string Name { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
