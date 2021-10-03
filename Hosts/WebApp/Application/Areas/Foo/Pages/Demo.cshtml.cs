using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CleanDDDArchitecture.Hosts.WebApp.Application.Areas.Foo.Pages
{
    public class Demo : PageModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Slug { get; set; }

        public void OnGet(string slug)
        {
            Slug = slug;
        }
    }
}
