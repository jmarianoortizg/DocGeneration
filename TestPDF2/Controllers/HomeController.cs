using Core.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestPDF2.Models;

namespace TestPDF2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IActionContextAccessor _actionContextAccessor;

        public HomeController(ILogger<HomeController> logger, IActionContextAccessor actionContextAccessor)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> GenerateReport(string PDCADocUUID)
        {   
            var fileName = string.Concat("AN_", ".pdf");
            return new HTMLToPDF("~/Views/Home/Privacy.cshtml", fileName: fileName.ToUpper(), _actionContextAccessor, model: null, size: "A1", switches: "-O Landscape").GeneratePdf();
        }
    }
}
