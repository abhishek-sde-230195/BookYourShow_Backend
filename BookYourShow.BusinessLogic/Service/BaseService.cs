using BookYourShow.DataTransferObject.DTO;

namespace BookYourShow.BusinessLogic.Service {
    public class BaseService {
        public ResponseDto response;

        public BaseService () {
            response = new ResponseDto {
                IsSuccess = true
            };
        }

    }
}