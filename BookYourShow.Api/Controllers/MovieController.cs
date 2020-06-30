using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookYourShow.BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;
using BookYourShow.DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BookYourShow.Api.Helper;

namespace BookYourShow.Api.Controllers {
    [ApiController]
    [Authorize]
    [Route ("api/[controller]")]
    public class MovieController : ControllerHelperBase {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;

        public MovieController (IMovieService movieService, ILogger<MovieController> logger) {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpPost ("AddMovies")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddMovies ([FromForm]MovieDto movie) {
            if(ModelState.IsValid){
                var files = HttpContext.Request.Form.Files;
                response =  await _movieService.AddMovie(movie, files);
                if(response.IsSuccess){
                    return Ok (response);
                }
            }else{
                response.Message =  Constant.Messages.ErrorDataInValid;
                response.IsSuccess = false;
                response.Errors = ModelState.Values
                    .SelectMany (v => v.Errors)
                    .Select (e => e.ErrorMessage);

            }
            return (BadRequest(response));
        }

        [HttpPost ("EditMovies")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditMovies ([FromForm]MovieDto movie) {
            if(ModelState.IsValid){
                var files = HttpContext.Request.Form.Files;
                response =  await _movieService.EditMovie(movie, files);
                if(response.IsSuccess){
                    return Ok (response);
                }
            }else{
                response.Message =  Constant.Messages.ErrorDataInValid;
                response.IsSuccess = false;
                response.Errors = ModelState.Values
                    .SelectMany (v => v.Errors)
                    .Select (e => e.ErrorMessage);

            }
            return (BadRequest(response));
        }

        [HttpGet ("GetMovieByPage")]
        public async Task<IActionResult> GetMovieByPage (int pageNumber, int pageSize) {
            response =  await _movieService.GetMovies (pageNumber, pageSize);
            return ReturnResponse(response);
        }

        [HttpGet ("GetMoviePagination")]
        public async Task<IActionResult> GetMoviePagination (int pageNumber, int pageSize) {
            response =  await _movieService.GetMoviePagination (pageNumber, pageSize);
            return ReturnResponse(response);
        }

        [HttpGet ("GetMovie/{movieId}")]
        public async Task<IActionResult> GetMovie (long movieId) {
            response =  await _movieService.GetMovie (movieId);
            return ReturnResponse(response);
        }
    }
}