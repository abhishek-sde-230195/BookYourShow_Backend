using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Constants;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class BaseHelperController : ControllerBase
    {
        public ResponseDto response;
        public BaseHelperController()
        {
            response = new ResponseDto
            {
                IsSuccess = true
            };
        }

        protected IActionResult ReturnResponse(ResponseDto res)
        {
            if (res.IsSuccess)
                return Ok(res);
            return BadRequest(res);
        }

        protected void SetModelStateErrorMessage()
        {
            response.Message = MessageConstant.Global.InValidData;
            response.IsSuccess = false;
            response.Errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
        }
        protected void SetErrorMessage()
        {
            response.Message = MessageConstant.Global.InValidData;
            response.IsSuccess = false;
        }
    }
}
