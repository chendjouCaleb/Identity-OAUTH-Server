namespace Everest.Identity.Core.ExceptionTransformers
{
    public class ErrorResponseModel
    {
        public int StatusCode { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string HelpLink { get; set; }
    }
}
