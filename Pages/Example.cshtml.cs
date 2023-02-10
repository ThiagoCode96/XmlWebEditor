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

namespace XmlWebEditor.Pages
{
    public class ExampleModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        public string Result { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public string texto2 { get; set; }

        private string fileName = "auxText";

        public TextMananger textMananger= new TextMananger();

        public ExampleModel(IWebHostEnvironment _environment)
        {
            environment = _environment;
        }
        public string getFile()
        {
            return Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
        }
        public void OnGet()
        {
            texto2 = textMananger.NewJsonFile(environment, fileName);
        }
        /*
         * ver mais tarde:
        [HttpGet]
        public ActionResult JsTreeDemo()
        {
            return View();
        }
        */
        //private string file = Path.Combine(environment.ContentRootPath, "xml", fileName);


    }
}
