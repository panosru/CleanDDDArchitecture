﻿namespace CleanDDDArchitecture.RestApi.Utils.Swagger
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
    public sealed class YamlDocumentFilter : IDocumentFilter
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlDocumentFilter" /> class.
        /// </summary>
        /// <param name="hostingEnvironment">IHostingEnvironment</param>
        public YamlDocumentFilter(IWebHostEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

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
                builder.WithNamingConvention(new CamelCaseNamingConvention());
                builder.WithTypeInspector(innerInspector => new PropertiesIgnoreTypeInspector(innerInspector));

                var serializer = builder.Build();

                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, swaggerDoc);

                    var file = Path.Combine(_hostingEnvironment.WebRootPath, "swagger.yaml");
                    using (var stream = new StreamWriter(file))
                    {
                        var result = writer.ToString();
                        stream
                            .WriteLine(result
                                .Replace("2.0", "\"2.0\"", StringComparison.OrdinalIgnoreCase)
                                .Replace("ref:", "$ref:", StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private class PropertiesIgnoreTypeInspector : TypeInspectorSkeleton
        {
            private readonly ITypeInspector _typeInspector;

            public PropertiesIgnoreTypeInspector(ITypeInspector typeInspector)
            {
                this._typeInspector = typeInspector;
            }

            public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
            {
                return _typeInspector.GetProperties(type, container)
                    .Where(p => p.Name != "extensions" && p.Name != "operation-id");
            }
        }
    }
}