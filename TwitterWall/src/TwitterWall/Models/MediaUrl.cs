﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterWall.Models
{
    public class MediaUrl
    {
        public MediaUrl()
        {
        }

        public MediaUrl(string url, Tweet tweet)
        {
            this.Url = url;
            this.Tweet = tweet;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        [JsonIgnore]
        [Required]
        public Tweet Tweet { get; set; }
    }
}