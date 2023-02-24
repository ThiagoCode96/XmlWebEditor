using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System;
using XmlWebEditor.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

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
        private string fileName = "auxArchive";
        TextMananger textUpdate = new TextMananger();
        public void OnGet()
        {
            text1 = textUpdate.NewJsonFile(environment, fileName);//limpar o Json
            text1 = textUpdate.NewXmlFile(environment, fileName);//limpar o Xml
        }
        public void OnPostXmlTextStart()
        {
            text1 = textUpdate.NewXmlFile(environment, fileName);
        }
        public void OnPostJsonTextStart()
        {
            text1 = textUpdate.NewJsonFile(environment, fileName);
        }
        /*
         * fução que controla a entrada e saída dos dois textos
         */
        public void OnPostTextController(string text)
        {

            string aux = text;
            text1 = text;//Xml que as pessoas editam
            text2 = "";//Xml que estará disponível para a pessoa pegar. 



            text2 += textUpdate.UpdateFile(environment, fileName, aux, ref message);
            if (message != "")
            {
                IsResponse = true;
                IsSuccess = false;
            }
            else
            {
                text1 = text2;
                SaveFile(text1);
            }

        }
        public void OnPost()
        {

        }
        public void OnPostSetFile()//send file to the textes
        {
            if (Upload == null)
                return;
            text1 = textUpdate.SetFile(Upload, environment, fileName, ref message);
            if (message != "")
            {
                if (message.Contains("documento inválido"))
                    text1 = "";
                IsResponse = true;
                IsSuccess = false;
            }
            else
            {
                IsResponse = true;
                IsSuccess = true;
                message = "arquivo enviado com sucesso";
            }
        }
        public ActionResult OnGetJsonFile()
        {
            string file;
            try
            {
                file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
                byte[] bytes = System.IO.File.ReadAllBytes(file);
                string contentType;
                new FileExtensionContentTypeProvider().TryGetContentType(file, out contentType);
                return PhysicalFile(file, contentType, "json_file_download.json");
            }
            catch (System.IO.FileNotFoundException)
            {
                message = "Não há nenhum arquivo no sistema. Se desejar, por favor crie um novo arquivo";
                IsResponse = true;
                IsSuccess = false;
                return Redirect(Request.Headers["Referer"].ToString());//retun "null"
            }
            catch (System.NullReferenceException e)
            {
                message = e.Message;
                IsResponse = true;
                IsSuccess = false;
                return Redirect(Request.Headers["Referer"].ToString());//retun "null"
            }

        }// end GetJsonFile
        public ActionResult OnGetXmlFile()
        {
            string file;
            try
            {

                file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
                byte[] bytes = System.IO.File.ReadAllBytes(file);
                string contentType;
                new FileExtensionContentTypeProvider().TryGetContentType(file, out contentType);
                return PhysicalFile(file, contentType, "xml_file_download.xml");
            }
            catch (System.IO.FileNotFoundException)
            {
                message = "Não há nenhum arquivo no sistema. Se desejar, por favor crie um novo arquivo";
                IsResponse = true;
                IsSuccess = false;
                return Redirect(Request.Headers["Referer"].ToString());//retun "null"
            }
            catch (System.NullReferenceException e)
            {
                message = e.Message;
                IsResponse = true;
                IsSuccess = false;
                return Redirect(Request.Headers["Referer"].ToString());//retun "null"
            }
        }//fim GetXmlFile
        public void OnPostXmlMinimize()
        {
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            text1 = doc.OuterXml;
        }
        public void OnPostJsonMinimize()
        {
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
            string dataFile = System.IO.File.ReadAllText(file);
            var obj = JsonConvert.DeserializeObject(dataFile);
            text1 = JsonConvert.SerializeObject(obj);
          
        }
        public void OnPostXmlIdent()
        {
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            XDocument text = XDocument.Parse(doc.OuterXml);//automaticamente identa
            text1 = text.ToString();
            
        }
        public void OnPostJsonIdent()
        {

            var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
            string dataFile = System.IO.File.ReadAllText(file);
            text1 = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(dataFile), Newtonsoft.Json.Formatting.Indented);

        }
        public void OnPostXmlToJson()
        {
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            text1 = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
        }
        public void OnPostJsonToXml()
        {
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(dataFile);
            doc.PreserveWhitespace = true;
            //identação do XML
            XDocument text = XDocument.Parse(doc.OuterXml);//automaticamente identa
            text1 = text.ToString();
        }
        public void OnPostJstreeToData (string jstreeData, string document)
        {
            text2 = "";
            text2+= textUpdate.ConvertJstree(environment, fileName,ref message, document, jstreeData);
            if (message != "")
            {
                string file;
                if(document=="xml")
                     file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
                else
                    file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
                string dataFile = System.IO.File.ReadAllText(file);
                text1 = dataFile;
                IsResponse = true;
                IsSuccess = false;
            }
            else {
                text1 = text2;
                SaveFile(text1);
            }
        }


        //criação save file
        private void SaveFile(string text){
            
            if (text.StartsWith("<"))
            {
                var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
                System.IO.File.WriteAllText(file, text);
            }
            else
            {
                var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
                
                System.IO.File.WriteAllText(file, text);
                  
            }
        }

    }//Fim Classe IndexModel
    

}
