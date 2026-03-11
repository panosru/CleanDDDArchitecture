using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CleanDDDArchitecture.Hosts.WebApp.Core.Localisation;

/// <inheritdoc />
/// <summary>
/// </summary>
public sealed class CultureTemplatePageRouteModelConvention : IPageRouteModelConvention
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="model"></param>
    public void Apply(PageRouteModel model)
    {
        (from selectorModel in model.Selectors
         let template = selectorModel.AttributeRouteModel?.Template
         where !string.IsNullOrWhiteSpace(template)
         select new SelectorModel
         {
             AttributeRouteModel = new AttributeRouteModel
             {
                 Order = -1,
                 Template = AttributeRouteModel.CombineTemplates(
                     "/{culture:required}",
                     template)
             }
         })
           .ToList()
           .ForEach(s => model.Selectors.Add(s));
    }
}
