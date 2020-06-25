namespace BookYourShow.BusinessLogic.Constants
{
    public static class ServiceConstant
    {
        public struct FileStorage{
            public const string Thumnail = "Thumbnail";
        }
        public struct Messages{
            public const string ErrorDataInValid = "Data is not valid";
            
            #region Movies
            public struct Movies{
                public const string SuccessCreateMovie = "Movie added and thumbnail saved successfully"; 
                public const string SuccessCreateMovieNoThumbnail = "Movie details saved successfully";      
                public const string ErrorCreateMovie = "Movie added but thumbnail not saved!!";
                public const string ErrorGetMovie = "No more data found.";

            }   
            #endregion
        }
    }
}