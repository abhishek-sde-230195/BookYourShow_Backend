using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObject.DTO
{
    public class ResponseDto
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public object Data { get; set; }
    }

}
