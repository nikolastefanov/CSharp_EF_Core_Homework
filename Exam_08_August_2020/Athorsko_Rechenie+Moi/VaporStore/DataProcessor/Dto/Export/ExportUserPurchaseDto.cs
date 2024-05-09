﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.ExportResults
{


    [XmlType("Purchase")]
    public class ExportUserPurchaseDto
    {

        [XmlElement("Card")]
        public string CardNumber { get; set; }

        [XmlElement("Cvc")]
        public string CardCvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Game")]
        public ExportUserPurchaseGameDto Game { get; set; }
    }
}
