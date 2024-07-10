using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Common.AuthMiddleware
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ClaimAttribute : Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}