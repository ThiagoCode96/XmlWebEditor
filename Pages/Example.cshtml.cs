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

        public ExampleModel(IWebHostEnvironment _environment)
        {
            environment = _environment;
        }
        public void xml_tree()
        {
            string file = Path.Combine(environment.ContentRootPath, "xml", fileName+".xml");
            XmlDocument document= new XmlDocument();
            document.LoadXml(file);


        }

        //private string file = Path.Combine(environment.ContentRootPath, "xml", fileName);
       

    }
}
