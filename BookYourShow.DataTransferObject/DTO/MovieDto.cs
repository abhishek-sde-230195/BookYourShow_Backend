using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookYourShow.DataTransferObject.DTO
{
    public class MovieDto
    {
        public long id {get;set;}
        [Required]
        public string MovieName{get;set;}
        public DateTime LaunchDate{get;set;}
        public string ThumbnailUrl{get;set;}
        public List<string> MediaUrls {get;set;}
        public string MovieSummary{get;set;}
    }
}