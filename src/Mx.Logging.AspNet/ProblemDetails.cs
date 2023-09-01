
namespace Mx.Logging.AspNet
{
    public class ProblemDetails
    {
        public ProblemDetails()
        {
            ErrorId = string.Empty;
            Type = string.Empty;
            Title = string.Empty;
            Status = 500;
            Detail = string.Empty;
            Instance = string.Empty;
        }
        public ProblemDetails(string errorId, string type, string title, int status, string detail, string instance)
        {
            ErrorId = errorId;
            Type = type;
            Title = title;
            Status = status;
            Detail = detail;
            Instance = instance;

        }
        public string ErrorId { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set;  }
        public string Instance { get; set; }

    }
}
