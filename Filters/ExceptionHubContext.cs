using System;
using System.Net;

namespace SignalRToolkit.Filters
{
    public class ExceptionHubContext
    {
        public Exception Exception { get; set; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public object ExceptionResult { get; set; }
    }
}
