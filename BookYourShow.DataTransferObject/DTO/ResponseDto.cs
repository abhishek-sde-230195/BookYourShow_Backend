using System.Collections.Generic;

namespace BookYourShow.DataTransferObject.DTO
{
    public class ResponseDto
    {
        public string Message{get;set;}
        public bool IsSuccess {get;set;}
        public IEnumerable<string> Errors{get;set;}
        public object Data {get;set;}
    }

    public class  UserManagerResponseDto : ResponseDto {}
}