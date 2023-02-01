using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System;
using XmlWebEditor.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;

namespace XmlWebEditor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
       
        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment _environment)
        {
            _logger = logger;
            environment = _environment;
        }
        
        private readonly IWebHostEnvironment environment;
        [BindProperty]
        public IFormFile Upload { get; set; }
        public string text2 { get; set; }
        public string text1 { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsResponse { get; set; }
        public string message = "";
        private string fileName = "xmlArchive.xml";
        XmlUpdater xmlUpdate = new XmlUpdater();
        public void OnGet()
        {
            text1=xmlUpdate.NewXmlFile( environment, fileName);
        }
        public void OnPostXmlTextStart()
        {
            text1 = xmlUpdate.NewXmlFile( environment, fileName);
        }
        /*
         * fução que controla a entrada e saída dos dois textos
         */
        public void OnPostXmlTextController( string xml)
        {
            string aux = xml;
            text1 = xml;//Xml que as pessoas editam
            text2 = "";//Xml que estará disponível para a pessoa pegar. 
            
            /*
             * Mudanças para o futuro: 
             * -texto2 irá capturar as informações do texto1 (ou seja: a pessoa irá editar
             * e irá atualizar no texto 2 mesmo
             * -função para atualizar texto 1 e aparecer as coisas para editar. 
             */

            
            this.text2 += xmlUpdate.UpdateXmlFile(Upload,environment,fileName,aux,ref message);
            if (message != "")
            {
                IsResponse = true;
                IsSuccess = false;
            }
            else 
                text1= text2;

         }
        public void OnPost()
        {

        }
        public void OnPostXmlSetFile()//send file to the textes
        {
            if (Upload == null)
                return;
            text1 = xmlUpdate.SetXmlFile(Upload, environment, fileName,ref message);
            if (message != "")
            {
                if (message.Contains("documento inválido"))
                    text1 = "";
                IsResponse = true;
                IsSuccess = false;
            }else
            {
                IsResponse = true;
                IsSuccess =true;
                message ="Xml enviada com sucesso";
            }
        }
        public ActionResult OnGetXmlGetFile()
        {
            try
            {
                var file = Path.Combine(environment.ContentRootPath, "xml", fileName);
                byte[] bytes = System.IO.File.ReadAllBytes(file);
                string contentType;
                new FileExtensionContentTypeProvider().TryGetContentType(file, out contentType);

                return PhysicalFile(file, contentType, "xml_file_download.xml");
            }
            catch (System.IO.FileNotFoundException)
            {
                text1 = "Não há nenhum arquivo no sistema. Se desejar, por favor crie um novo arquivo";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}