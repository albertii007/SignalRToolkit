using System.Net;

namespace SignalRToolkit.ResponseResult
{
    public sealed class HubResponseResult<TResult>
    {
        public TResult Result { get; set; }
        public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
        public object ExceptionMessage { get; set; }
        public bool HasException { get; set; } = false;
    }
}
