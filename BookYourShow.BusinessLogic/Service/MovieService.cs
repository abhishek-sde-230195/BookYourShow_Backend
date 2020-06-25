using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BookYourShow.BusinessLogic.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BookYourShow.DataTransferObject.DTO;
using BookYourShow.Data.EFDatabase;
using System.Linq;

namespace BookYourShow.BusinessLogic.Service {
    public interface IMovieService {
        List<string> ShowAllMovies ();
        Task<ResponseDto> AddMovie (MovieDto model, IFormFileCollection files);
        Task<ResponseDto>  GetMovies(int pageNumber, int pageSize);
        Task<ResponseDto> GetMovie(long movieId);
        Task<ResponseDto> EditMovie (MovieDto dto, IFormFileCollection files);
    }

    public class MovieService : BaseService,  IMovieService {

        private readonly IConfiguration _configuration;
        private readonly ILogger<MovieService> _logger;

        private readonly BYSDBContext _context;

        public MovieService (IConfiguration configuration, ILogger<MovieService> logger, BYSDBContext context) {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        #region  Private methods
        private bool CheckIfImageFile (IFormFile file) {
            var extension = "." + file.FileName.Split ('.') [file.FileName.Split ('.').Length - 1];
            return (extension.ToLower () == ".jpg" || extension.ToLower () == ".png"); // Change the extension based on your need
        }

        private async Task<string> WriteFile (IFormFile file, string movieName) {
            string newFileName= string.Empty;

            try {
                var extension = "." + file.FileName.Split ('.') [file.FileName.Split ('.').Length - 1];
                string fileName = ServiceConstant.FileStorage.Thumnail;
                string uniqueName = string.Format (@"!__!{0}{1}", Guid.NewGuid (), extension);
                fileName +=  uniqueName;
                newFileName = $"\\{movieName}\\{fileName}";

                var pathBuilt = Path.Combine (_configuration["FilePaths:Thumbnail"], $"{movieName}");

                if (!Directory.Exists (pathBuilt)) {
                    Directory.CreateDirectory (pathBuilt);
                }

                var path = Path.Combine (pathBuilt, fileName);

                using (var stream = new FileStream (path, FileMode.Create)) {
                    await file.CopyToAsync (stream);
                }
            } 
            catch (Exception e) {
                newFileName = string.Empty;
                _logger.LogError(e, e.Message);
            }

            return newFileName;
        }

        #endregion

        #region  Public Methods
        public List<string> ShowAllMovies () {
            List<string> stringList = new List<string> ();
            var senderEmail = _configuration["MailSettings:FromEmail"];
            stringList.Add ("Main Hoon Na");
            stringList.Add ("How are you");
            stringList.Add (senderEmail);

            return stringList;
        }
      

        public async Task<ResponseDto> AddMovie (MovieDto dto, IFormFileCollection files) {

            Movie movie = new Movie{
                MovieName = dto.MovieName,
                LaunchDate = dto.LaunchDate,
                MovieSummary = dto.MovieSummary
            };
            using(_context){
                _context.Add(movie);
                foreach (var file in files) {
                    if (CheckIfImageFile (file)) {
                        string urlPath = await WriteFile (file, dto.MovieName);
                        if(!string.IsNullOrWhiteSpace(urlPath)){
                            movie.ThumbnailUrl = urlPath;
                            await _context.SaveChangesAsync();
                            response.Message = ServiceConstant.Messages.Movies.SuccessCreateMovie;
                        }else{
                            response.Message = ServiceConstant.Messages.Movies.ErrorCreateMovie;
                            response.IsSuccess = false;
                        }
                    }
                }
                return response;
            }
        }

         public async Task<ResponseDto> EditMovie (MovieDto dto, IFormFileCollection files) {

            using(_context){
                var movie = _context.Movie
                    .Where(x => x.id == dto.id).FirstOrDefault();
                movie.MovieName = dto.MovieName??movie.MovieName;
                movie.MovieSummary = dto.MovieSummary??movie.MovieSummary;
                movie.LaunchDate = dto.LaunchDate != null?dto.LaunchDate: movie.LaunchDate;
                if(files.Count <= 0 ){
                    await _context.SaveChangesAsync();
                    response.Message = ServiceConstant.Messages.Movies.SuccessCreateMovieNoThumbnail;
                }
                foreach (var file in files) {
                    if (CheckIfImageFile (file)) {
                        string urlPath = await WriteFile (file, dto.MovieName);
                        if(!string.IsNullOrWhiteSpace(urlPath)){
                            movie.ThumbnailUrl = urlPath;
                            await _context.SaveChangesAsync();
                            response.Message = ServiceConstant.Messages.Movies.SuccessCreateMovie;
                        }else{
                            response.Message = ServiceConstant.Messages.Movies.ErrorCreateMovie;
                            response.IsSuccess = false;
                        }
                    }
                }
                return response;
            }
        }

        public async Task<ResponseDto> GetMovies(int pageNumber, int pageSize)
        {
            using (_context){
                List<MovieDto> moviesDto = new List<MovieDto>();
                var movies = _context.Movie
                    .OrderByDescending(x=>x.LaunchDate)
                    .Skip((pageNumber-1)*pageSize)
                    .Take(pageSize);
                foreach(var movie in movies){
                    moviesDto.Add(new MovieDto{
                        id = movie.id,
                        MovieName = movie.MovieName,
                        ThumbnailUrl = movie.ThumbnailUrl,
                        LaunchDate = movie.LaunchDate,
                        MovieSummary = movie.MovieSummary
                    });
                }
                if(moviesDto.Count > 0){
                    response.Data = moviesDto;
                }else{
                    response.IsSuccess = true;
                    response.Message = Constants.ServiceConstant.Messages.Movies.ErrorGetMovie;
                }  
            }
            return response;
        }

        public async Task<ResponseDto> GetMovie(long movieId)
        {
            using (_context){
                var movie = _context.Movie
                    .Where(x => x.id == movieId).FirstOrDefault();
                
                if(movie == null){
                    response.IsSuccess = true;
                    response.Message = Constants.ServiceConstant.Messages.Movies.ErrorGetMovie;
                }else{
                    response.Data = new MovieDto{
                        id = movie.id,
                        MovieName = movie.MovieName,
                        ThumbnailUrl = movie.ThumbnailUrl,
                        LaunchDate = movie.LaunchDate,
                        MovieSummary = movie.MovieSummary
                    };
                }
            }
            return response;
        }

        #endregion

    }
}