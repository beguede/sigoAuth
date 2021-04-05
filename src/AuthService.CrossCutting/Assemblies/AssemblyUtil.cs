using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AuthService.CrossCutting.Assemblies
{
    [ExcludeFromCodeCoverage]
    public class AssemblyUtil
    {
        public static IEnumerable<Assembly> GetCurrentAssemblies()
        {            
            return new Assembly[]
            {
                Assembly.Load("AuthService.Api"),
                Assembly.Load("AuthService.Application"),
                Assembly.Load("AuthService.Domain"),
                Assembly.Load("AuthService.Domain.Core"),
                Assembly.Load("AuthService.Infrastructure"),
                Assembly.Load("AuthService.CrossCutting")
            };
        }
    }
}
