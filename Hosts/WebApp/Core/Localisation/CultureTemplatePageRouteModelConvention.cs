namespace CleanDDDArchitecture.Hosts.WebApp.Core.Localisation
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

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
             select new SelectorModel
             {
                 AttributeRouteModel = new AttributeRouteModel
                 {
                     Order = -1,
                     Template = AttributeRouteModel.CombineTemplates(
                         "/{culture:required}",
                         selectorModel.AttributeRouteModel.Template)
                 }
             })
               .ToList()
               .ForEach(s => model.Selectors.Add(s));
        }
    }
}
