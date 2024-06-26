﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.ImportResults
{
    public class ImportUserDto
    {
        [Required]
        [MinLength(GlobalConstants.UserUsernameMinLength)]
        [MaxLength(GlobalConstants.UserUsernameMaxLength)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(GlobalConstants.UserFullNameRegex)]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(GlobalConstants.UserAgeMinValue, GlobalConstants.UserAgeMaxValue)]
        public int Age { get; set; }

        public ImportUserCardDto[] Cards { get; set; }
    }
}
