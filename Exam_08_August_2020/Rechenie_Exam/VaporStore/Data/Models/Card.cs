﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
            this.Purchases = new HashSet<Purchase>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression("^(\\d{4}) (\\d{4}) (\\d{4}) (\\d{4})$")]
        public string Number { get; set; }

        [Required]
        [RegularExpression("^(\\d{3})$")]
        public string Cvc { get; set; }

        [Required]
        [Range(0,1)]
        public CardType Type { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; }
        [Required]
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
