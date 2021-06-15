using System.Collections.Generic;

namespace SignalRToolkit.Base
{
    public class HubStorageHandler
    {
        public IDictionary<string, string[]> _strictMethods { get; private set; } = new Dictionary<string, string[]>();

        public void SetMethod(string methodName, params string[] roles)
        {
            _strictMethods.Add(methodName, roles);
        }
    }
}