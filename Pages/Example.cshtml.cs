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
        public ExampleModel(IWebHostEnvironment _environment)
        {
            environment = _environment;
        }
        public void OnPostXmlTextController2()
        {

            string fileName = "xmlText"; //Upload.FileName;
            var file=Path.Combine(environment.ContentRootPath,"xml", fileName);
            string xml="";
            try
            {
                using (var reader = new StreamReader(Upload.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        xml += reader.ReadLine();
                    }
                    reader.Dispose();
                }
                XmlUpdater updater = new XmlUpdater();
                texto2 = xml;
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    byte[] information = new UTF8Encoding(true).GetBytes(xml);
                    fileStream.Write(information, 0, information.Length);
                }
                ViewData["result"] = "test?";
            }
            catch(System.NullReferenceException )
            {
                ViewData["result"]="Por favor, insira um documento Xml";
            }
            





                
       
            
            
            
        }

    }
}
