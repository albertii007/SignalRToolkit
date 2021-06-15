using System;

namespace SignalRToolkit.Base.Attributes
{
    public class RouteHubAttribute : Attribute
    {
        public string _path { get; private set; }
        public RouteHubAttribute(string path)
        {
            _path = path;
        }
    }
}
