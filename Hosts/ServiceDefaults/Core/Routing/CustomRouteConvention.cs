using System.Reflection;
using CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;
using CleanDDDArchitecture.Hosts.RestApi.Core.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Routing;

public sealed class CustomRouteConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var routeTemplate = GetRouteTemplate(controller.ControllerType);

            foreach (var selector in controller.Selectors.Where(selector => selector.AttributeRouteModel is not null))
            {
                var template = selector.AttributeRouteModel!.Template!.Replace("[segments]", routeTemplate);
                selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(template.TrimEnd('/')));
            }
        }
    }

    private static string GetRouteTemplate(Type controllerType)
    {
        var segments = new List<string>();

        while (controllerType != null && controllerType != typeof(ControllerBase))
        {
            if (IsBaseControllerType(controllerType))
            {
                controllerType = controllerType.BaseType!;
                continue;
            }

            var segmentAttribute = controllerType.GetCustomAttribute<RouteSegmentAttribute>();
            if (segmentAttribute is not null)
            {
                segments.Insert(0, segmentAttribute.Segment);
            }
            else if (!IsBaseControllerType(controllerType.BaseType!) && !AnyParentControllerHasRouteSegment(controllerType))
            {
                segments.Insert(0, GetSegmentName(controllerType));
            }

            controllerType = controllerType.BaseType!;
        }

        return string.Join("/", segments);
    }

    private static bool AnyParentControllerHasRouteSegment(Type controllerType)
    {
        var baseType = controllerType.BaseType;
        while (baseType is not null && baseType != typeof(ControllerBase))
        {
            if (baseType.GetCustomAttribute<RouteSegmentAttribute>() is not null)
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool IsBaseControllerType(Type controllerType) =>
        controllerType == typeof(ApiController)
        || controllerType == typeof(ApiSharedController)
        || (controllerType.IsGenericType && controllerType.GetGenericTypeDefinition() == typeof(ApiController<,>));

    private static string GetSegmentName(Type type) =>
        type.IsGenericType
            ? type.Name.Split('`')[0]
            : type.Name.Replace("Controller", string.Empty, StringComparison.Ordinal);
}
