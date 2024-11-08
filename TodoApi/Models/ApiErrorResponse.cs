namespace TodoApi.Models
{
    public class ApiErrorResponse
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public int StatusCode { get; set; }
    }
}
