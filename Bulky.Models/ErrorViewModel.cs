namespace Bulky.Models
{
    // Default error model that ships with the MVC template. The Error view shows the
    // RequestId so a specific failed request can be traced in the logs.
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        // Only show the request id in the UI when we actually have one.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
