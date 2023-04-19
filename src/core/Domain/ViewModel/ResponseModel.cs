namespace Model
{
    public class ResponseModel<T>
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class MediatRResponseModel<T>
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public T Data { get; set; }
    }
    public class ParametersPageModel
    {
        const int maxPageSize = 500;
        public int PageNumber { get; set; } = 1;
        public int Comid ;
        private int _pageSize = 10;
        public string sortOn = "";
        public string sortDirection = "";
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
