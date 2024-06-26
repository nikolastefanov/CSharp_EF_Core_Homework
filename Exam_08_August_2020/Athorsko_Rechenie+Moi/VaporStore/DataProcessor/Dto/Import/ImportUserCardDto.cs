﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.ImportResults
{
    public class ImportUserCardDto
    {
        [Required]
        [RegularExpression(GlobalConstants.CardNumberRegex)]
        public string Number { get; set; }

        [Required]
        [MaxLength(3)]
        [RegularExpression(GlobalConstants.CardCvcRegex)]
        [JsonProperty("CVC")]
        public string Cvc { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
