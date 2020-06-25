using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookYourShow.DataTransferObject.DTO
{
    public class TheatreDto
    {
        public long id {get;set;}
        [Required]
        public string TheatreName{get;set;}
        public string OpeningTime{get;set;}
        public string ClosingTime{get;set;}
        public string ThumbnailUrl{get;set;}
        public long CityId { get; set; }
    }
}