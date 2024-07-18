using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Common.AuthMiddleware
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeClaimAttribute : AuthorizeAttribute
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }
}