﻿using System.ComponentModel.DataAnnotations;
namespace BE.Data.Models
{
    public class Module
    {
        [Required]
        public int id { get; set; }
        public string nameModule { get; set; }
        public string note { get; set; }
        public int isDeleted { get; set; }
        public int idSort { get; set; }

    }
}
