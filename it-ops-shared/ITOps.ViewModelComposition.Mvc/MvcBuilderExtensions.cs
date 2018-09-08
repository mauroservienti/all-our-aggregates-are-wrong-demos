using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ITOps.ViewModelComposition.Mvc
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddViewModelCompositionMvcSupport(this IMvcBuilder builder, string assemblySearchPattern = "*ViewModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);

            var types = new List<Type>();
            foreach (var fileName in fileNames)
            {
                var temp = AssemblyLoader.Load(fileName)
                    .GetTypes()
                    .Where(t =>
                    {
                        var typeInfo = t.GetTypeInfo();
                        return !typeInfo.IsInterface
                            && !typeInfo.IsAbstract
                            && typeof(IHandleResult).IsAssignableFrom(t);
                    });

                types.AddRange(temp);
            }

            foreach (var type in types)
            {
                builder.Services.AddSingleton(typeof(IHandleResult), type);
            }

            builder.Services.Configure<MvcOptions>(options => 
            {
                options.Filters.Add(typeof(CompositionActionFilter));
            });

            return builder;
        }
    }
}
