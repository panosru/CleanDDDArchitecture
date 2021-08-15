namespace CleanDDDArchitecture.Hosts.RestApi.Application.Swagger
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.TypeInspectors;

    /// <summary>
    ///     To use YAML serializer to generate YAML
    /// </summary>
    internal sealed class YamlDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// </summary>
        private readonly IWebHostEnvironment _hostingEnvironment;

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlDocumentFilter" /> class.
        /// </summary>
        /// <param name="hostingEnvironment">IHostingEnvironment</param>
        public YamlDocumentFilter(IWebHostEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;

        #region IDocumentFilter Members

        /// <summary>
        ///     Apply YAML Serializer
        /// </summary>
        /// <param name="swaggerDoc">SwaggerDocument</param>
        /// <param name="context">DocumentFilterContext</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            try
            {
                var builder = new SerializerBuilder();
                builder.WithNamingConvention(CamelCaseNamingConvention.Instance);
                builder.WithTypeInspector(innerInspector => new PropertiesIgnoreTypeInspector(innerInspector));

                var serializer = builder.Build();

                using var writer = new StringWriter();

                serializer.Serialize(writer, swaggerDoc);

                var file = Path.Combine(_hostingEnvironment.WebRootPath, "swagger.yaml");

                using var stream = new StreamWriter(file);

                var result = writer.ToString();

                stream
                   .WriteLine(
                        result
                           .Replace("2.0",  "\"2.0\"", StringComparison.OrdinalIgnoreCase)
                           .Replace("ref:", "$ref:",   StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Nested type: PropertiesIgnoreTypeInspector

        /// <summary>
        /// </summary>
        private sealed class PropertiesIgnoreTypeInspector : TypeInspectorSkeleton
        {
            /// <summary>
            /// </summary>
            private readonly ITypeInspector _typeInspector;

            /// <summary>
            /// </summary>
            /// <param name="typeInspector"></param>
            public PropertiesIgnoreTypeInspector(ITypeInspector typeInspector) => _typeInspector = typeInspector;

            /// <summary>
            /// </summary>
            /// <param name="type"></param>
            /// <param name="container"></param>
            /// <returns></returns>
            public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
            {
                return _typeInspector.GetProperties(type, container)
                   .Where(p => p.Name != "extensions" && p.Name != "operation-aggregateId");
            }
        }

        #endregion
    }
}