namespace Aviant.DDD.Application.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoMapper;

    public abstract class MappingProfileBase : Profile
    {
        public MappingProfileBase()
        {
            ApplyMappingsFromAssembly(Assembly.GetCallingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            List<Type> types = assembly.GetExportedTypes()
                .Where(
                    t => t.GetInterfaces()
                        .Any(
                            i =>
                                i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                                                    || i.GetGenericTypeDefinition() == typeof(IMapTo<>))))
                .ToList();

            foreach (Type type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping", BindingFlags.Instance | BindingFlags.NonPublic)
                                 ?? (type.GetInterfaces()
                                     .Any(
                                         i => i.IsGenericType &&
                                              i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                                     ? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping")
                                     : type.GetInterface("IMapTo`1")?.GetMethod("Mapping"));

                methodInfo?.Invoke(instance, new object?[] {this});
            }
        }
    }
}