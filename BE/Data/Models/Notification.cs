﻿using BE.Data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("Notification")]
    public class Notification : BaseEntity
    {
        public int? requestUser { get; set; }
        public int? receiveUser { get; set; }
        public string message { get; set; }
        public string title { get; set; }
        public bool? isWatched { get; set; } = false;
        public int? categoryModule { get; set; }
        public string? link { get; set; }
    }
}
