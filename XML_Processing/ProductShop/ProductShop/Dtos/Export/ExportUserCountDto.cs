﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    
    public class ExportUserCountDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")] 
        public ExportUserDto[] Users { get; set; }
    }

    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }


        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public ExportProdutCountDto SoldProduct { get; set; }
    }


    public class ExportProdutCountDto
    {
        [XmlElement("count")]
        public int Count { get; set; }


        [XmlArray("products")]
        public ExportProductDto[] Products { get; set; }
    }


    [XmlType("Product")]
    public class ExportProductDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
