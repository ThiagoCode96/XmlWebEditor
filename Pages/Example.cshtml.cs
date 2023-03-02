using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Xml;
using System.Xml.Schema;
using System.Linq.Expressions;
using XmlWebEditor.Controller;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Intrinsics.X86;
using System.Web;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Web.Helpers;

namespace XmlWebEditor.Pages
{
    public class ExampleModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        public string Result { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public string texto2 { get; set; }

        private string fileName = "auxArchive";

        private string archiveName = "xml";

        public TextMananger textMananger= new TextMananger();

        public ExampleModel(IWebHostEnvironment _environment)
        {
            environment = _environment;
            
        }
        public string getFile()
        {
            
            return Path.Combine(environment.WebRootPath, "xml", fileName + ".json");
        }
        public void OnGet()
        {
            texto2 = textMananger.NewJsonFile(environment, fileName, archiveName);

        }
        [HttpGet]
        public IActionResult GetData()
        {
            var filePath = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
            var jsonData = System.IO.File.ReadAllText(filePath);

            return new OkObjectResult(jsonData);
        }

    }
}
