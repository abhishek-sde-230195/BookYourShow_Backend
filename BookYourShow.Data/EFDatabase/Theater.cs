using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookYourShow.Data.EFDatabase {
    public class Theater {
        public long id { get; set; }
        public string TheaterName { get; set; }
        public long CityId { get; set; }
        public string OpeningTime{get;set;}
        public string ClosingTime{get;set;}
        public string ThumbnailUrl{get;set;}
        public City City { get; set; }
    }
}