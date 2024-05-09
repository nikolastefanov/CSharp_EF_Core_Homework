using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VaporStore.Data.Models
{
    public class GameTag
    {
        [ForeignKey("Game")]
        public int GameId { get; set; }

      
        public Game Game { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }
        [Required]
        public Tag Tag { get; set; }
    }
}
