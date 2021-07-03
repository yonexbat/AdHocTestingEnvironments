using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Pages
{
    public class ReverseProxyModel : PageModel
    {
        private readonly ILogger<ReverseProxyModel> _logger;

        public ReverseProxyModel(ILogger<ReverseProxyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
