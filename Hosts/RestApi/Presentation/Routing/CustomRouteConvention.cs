using System.Reflection;
using CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;
using CleanDDDArchitecture.Hosts.RestApi.Core.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Routing;

/// <summary>
/// CustomRouteConvention is used to customize the route generation for controllers.
/// It implements the IApplicationModelConvention interface.
/// </summary>
public class CustomRouteConvention : IApplicationModelConvention
{
    /// <summary>
    /// Applies the custom routing convention to all controllers in the application.
    /// </summary>
    /// <param name="application">The application model representing all controllers.</param>
    public void Apply(ApplicationModel application)
    {
        // Iterate over each controller in the application.
        foreach (var controller in application.Controllers)
        {
            // Generate a route template based on the controller type.
            var routeTemplate = GetRouteTemplate(controller.ControllerType);

            // Iterate through each route selector in the controller.
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    // Replace the placeholder [segments] in the route template with the generated route.
                    var template = selector.AttributeRouteModel.Template.Replace("[segments]", routeTemplate);

                    // Update the controller's route model with the new template, ensuring no trailing slashes.
                    selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(template.TrimEnd('/')));
                }
            }
        }
    }

    /// <summary>
    /// Generates the route template for a given controller type.
    /// </summary>
    /// <param name="controllerType">The type of the controller for which to generate the route.</param>
    /// <returns>A string representing the route template.</returns>
    private string GetRouteTemplate(Type controllerType)
    {
        var segments = new List<string>();

        // Traverse the controller's inheritance hierarchy to build the route template.
        while (controllerType != null && controllerType != typeof(ControllerBase))
        {
            if (IsBaseControllerType(controllerType))
            {
                // Skip custom base controller types in route template generation.
                controllerType = controllerType.BaseType;
                continue;
            }

            // Check for the presence of the RouteSegmentAttribute.
            var segmentAttribute = controllerType.GetCustomAttribute<RouteSegmentAttribute>();
            if (segmentAttribute != null)
            {
                // Add the custom segment from the attribute to the segments list.
                segments.Insert(0, segmentAttribute.Segment);
            }
            else if (!IsBaseControllerType(controllerType.BaseType) && !IsAnyParentControllerHasRouteSegmentAttribute(controllerType))
            {
                // Use the default controller name as a segment if no RouteSegmentAttribute is present in the inheritance chain.
                var segmentName = GetSegmentNameFromType(controllerType);
                segments.Insert(0, segmentName);
            }

            // Move up the inheritance chain.
            controllerType = controllerType.BaseType;
        }

        // Join the collected segments to form the complete route template.
        return string.Join("/", segments);
    }
    
    /// <summary>
    /// Checks if any parent controller in the inheritance chain of the given controller type has the RouteSegmentAttribute.
    /// </summary>
    /// <param name="controllerType">The type of the controller to check for parent controllers.</param>
    /// <returns>True if any parent controller has the RouteSegmentAttribute; otherwise, false.</returns>
    private bool IsAnyParentControllerHasRouteSegmentAttribute(Type controllerType)
    {
        // Start checking from the parent of the current controller type.
        var baseType = controllerType.BaseType;

        // Traverse up the inheritance chain.
        while (baseType != null && baseType != typeof(ControllerBase))
        {
            // If the RouteSegmentAttribute is found on a parent controller, return true.
            if (baseType.GetCustomAttribute<RouteSegmentAttribute>() != null)
            {
                return true;
            }

            // Move to the next level up in the inheritance chain.
            baseType = baseType.BaseType;
        }

        // Return false if the RouteSegmentAttribute is not found on any parent controller.
        return false;
    }


    /// <summary>
    /// Checks if a given type is one of the base controller types.
    /// </summary>
    /// <param name="controllerType">The controller type to check.</param>
    /// <returns>True if the type is a base controller type; otherwise, false.</returns>
    private bool IsBaseControllerType(Type controllerType)
    {
        // A controller type is considered a base controller if it is either ApiController or
        // a generic variant of ApiController<TUseCase, TUseCaseOutput>.
        return controllerType == typeof(ApiController) ||
               controllerType == typeof(ApiSharedController) ||
               (controllerType.IsGenericType && 
                controllerType.GetGenericTypeDefinition() == typeof(ApiController<,>));
    }

    /// <summary>
    /// Extracts a suitable segment name from a given type.
    /// </summary>
    /// <param name="type">The type from which to extract the segment name.</param>
    /// <returns>A string representing the segment name derived from the type.</returns>
    private string GetSegmentNameFromType(Type type)
    {
        // Handle generic types by removing the generic type notation (e.g., `1, `2, etc.).
        if (type.IsGenericType)
        {
            return type.Name.Split('`')[0];
        }

        // For non-generic types, remove 'Controller' from the end to derive the segment name.
        return type.Name.Replace("Controller", "");
    }
}
