using System.Collections.Generic;

namespace BookYourShow.DataTransferObject.DTO
{
    public class PageResultDto
    {
        public object Data{get;set;}
        public int TotalPages {get;set;}
        public int CurrentPage{get;set;}
        public int TotalItemCount {get;set;}
    }
}