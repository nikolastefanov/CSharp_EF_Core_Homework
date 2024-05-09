using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.ImportResults;

namespace VaporStore.ExportResults
{
 

        [XmlType("User")]
        public class ExportUserDto
        {
            [XmlAttribute("username")]
            public string Username { get; set; }

            [XmlArray("Purchases")]
            public ExportUserPurchaseDto[] Purchases { get; set; }

            [XmlElement("TotalSpent")]
            public decimal TotalSpent { get; set; }
        }
   
}
