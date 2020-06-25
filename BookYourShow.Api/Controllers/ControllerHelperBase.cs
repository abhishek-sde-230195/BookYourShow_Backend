using Microsoft.AspNetCore.Mvc;
using BookYourShow.DataTransferObject.DTO;
namespace BookYourShow.Api.Controllers
{
    public class ControllerHelperBase : ControllerBase
    {
        public ResponseDto response;
        public ControllerHelperBase(){
            response = new ResponseDto{
                IsSuccess = true
            };
        }

        protected IActionResult ReturnResponse(ResponseDto res) {
            if(res.IsSuccess)
                return Ok(res);
            return BadRequest(res);
        }
    }
}