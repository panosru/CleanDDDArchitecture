using System.Collections;
using System.Reflection;
using Aviant.Core.Collections.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Hosts.RestApi.Core.Features;

public sealed class CustomControllerFeatureProvider
    : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly IFeatureManager _featureManager;

    private ControllerFeature? _feature;

    public CustomControllerFeatureProvider(IFeatureManager featureManager) => _featureManager = featureManager;

    #region IApplicationFeatureProvider<ControllerFeature> Members

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        _feature = feature;

        feature.Controllers
           .Select((controller, index) => (controller, index))
           .ToList()
           .ForEach(
                x =>
                    GetCustomAttributes(x.controller, x.index));
    }

    #endregion

    private void GetCustomAttributes(TypeInfo controller, int index)
    {
        controller.AsType()
           .CustomAttributes
           .ForAll(
                customAttribute =>
                    FilterCustomAttributes(customAttribute, index));
    }


    private void FilterCustomAttributes(CustomAttributeData customAttribute, int index)
    {
        if (customAttribute.AttributeType.FullName != typeof(FeatureGateAttribute).FullName)
            return;

        if (customAttribute.ConstructorArguments.First().Value is IEnumerable arguments)
            arguments.Cast<object>()
               .ForAll(
                    @object =>
                        RemoveDisabledFeatureFromControllers(@object, index));
    }

    private void RemoveDisabledFeatureFromControllers(object argumentValue, int index)
    {
        var typedArgument      = (CustomAttributeTypedArgument)argumentValue;
        var typedArgumentValue = (Features)(int)typedArgument.Value!;

        var isFeatureEnabled = _featureManager
           .IsEnabledAsync(typedArgumentValue.ToString())
           .ConfigureAwait(false)
           .GetAwaiter()
           .GetResult();

        if (!isFeatureEnabled)
            _feature?.Controllers.RemoveAt(index);
    }
}
