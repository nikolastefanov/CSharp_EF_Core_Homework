﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObject
{
    [XmlType("suplier")]
    public class ExportLocalSuppliersDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }


        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
