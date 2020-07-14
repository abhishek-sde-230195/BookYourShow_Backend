using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Service
{
    public class BaseService
    {
        public ResponseDto response;

        public BaseService()
        {
            response = new ResponseDto
            {
                IsSuccess = true
            };
        }

    }
}
