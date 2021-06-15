using System;

namespace SignalRToolkit.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeHubAttribute : Attribute
    {
        public string[] Roles { get; private set; }
        public AuthorizeHubAttribute(params string[] rolesName)
        {
            Roles = rolesName;
        }
    }
}
