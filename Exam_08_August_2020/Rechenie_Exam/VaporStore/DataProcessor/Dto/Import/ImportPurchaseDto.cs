using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    
    public class ImportPurchaseDto
    {

        [Required]
        [Range(0, 1)]
        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [Required]
        [RegularExpression("^(\\w{4})-(\\w{4})-(\\w{4})$")]
        [XmlElement("Key")]
        public string ProductKey { get; set; }

        [Required]
        [XmlElement("Card")]
        public string Card { get; set; }

        [Required]
        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
