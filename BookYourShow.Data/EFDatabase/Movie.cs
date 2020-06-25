using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace BookYourShow.Data.EFDatabase
{
    public class Movie
    {
        public long id {get;set;}
        [Required]
        public string MovieName{get;set;}
        public DateTime LaunchDate{get;set;}
        public string ThumbnailUrl{get;set;}
        public string MediaUrls {get;set;}
        public string MovieSummary{get;set;}

    }
}