using System.ComponentModel.DataAnnotations;

namespace BookYourShow.Data.EFDatabase
{
    public class City
    {
        public long id{get;set;}
        [Required]
        public string CityName{get;set;}
    }
}