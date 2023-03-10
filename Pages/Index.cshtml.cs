using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Net;
using System;
using System.Xml;
using System.Xml.Linq;
using XmlWebEditor.Controller;
using System.Runtime.Intrinsics.X86;
using System.Reflection.Metadata;
using Microsoft.Extensions.WebEncoders.Testing;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using System.Text;

namespace XmlWebEditor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHostApplicationLifetime _lifeTime;
        private readonly IWebHostEnvironment environment;
        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment _environment)
        {
            _logger = logger;
            environment = _environment;
        }


        [BindProperty]
        public IFormFile Upload { get; set; }
        public string text2 { get; set; }
        public string text1 { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsResponse { get; set; }

        public string message = "";
        private string fileName = "auxArchive";
        private string archiveName = "xml";
        private string archiveUserName = "user"; //Serve para demonstrar aonde está o local do usuário

        TextMananger textUpdate = new TextMananger();
        private string SetFilePath()//set file apenas falará QUAL FILE É. Se deseja pegar o json ou o xml terá que fazer file=setfile()+(".json" ou ".xml") 
        {
            string file = Path.Combine(environment.ContentRootPath, archiveName, archiveUserName);
            file = Path.Combine(file, fileName);
            return file;
        }
        public void OnGet()
        {

            text1 = textUpdate.NewJsonFile(environment, fileName, archiveName);//limpar o Json

        }
        public void OnPostXmlTextStart()
        {
            text1 = textUpdate.NewXmlFile(environment, fileName, archiveName);

        }
        public void OnPostJsonTextStart()
        {
            text1 = textUpdate.NewJsonFile(environment, fileName, archiveName);

        }
        /*
         * fução que controla a entrada e saída dos dois textos
         */
        public void OnPostTextController(string text)
        {
            string aux = text;
            text1 = text;//Xml que as pessoas editam
            text2 = "";//Xml que estará disponível para a pessoa pegar. 



            text2 += textUpdate.UpdateFile(environment, SetFilePath(), aux, ref message);
            if (message != "")
            {
                IsResponse = true;
                IsSuccess = false;
            }
            else
            {
                text1 = text2;
            }

        }
        public void OnPost()
        {

        }
        public void OnPostSetFile()//send file to the textes
        {
            if (Upload == null)
                return;
            text1 = textUpdate.SetFile(Upload, environment, SetFilePath(), ref message);
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
        public FileContentResult OnPostJsonFile(string text)
            {
            try
            {
                var fileContent = Encoding.UTF8.GetBytes(text);
                var fileName = "json_file.json";
                return File(fileContent, "application/json", fileName);

            }
            catch (System.IO.FileNotFoundException)
            {
                message = "Não há nenhum arquivo no sistema. Se desejar, por favor crie um novo arquivo";
                IsResponse = true;
                IsSuccess = false;
                return null;
            }
            catch (System.ArgumentNullException e)
            {
                message = e.Message;
                IsResponse = true;
                IsSuccess = false;
                return null;
            }

        }// end GetJsonFile
        public FileContentResult OnPostXmlFile(string text)
        {
            try
            {
                var fileContent = Encoding.UTF8.GetBytes(text);
                var fileName = "xml_file.xml";
                return File(fileContent, "application/xml", fileName);
            }
            catch (System.IO.FileNotFoundException)
            {
                message = "Não há nenhum arquivo no sistema. Se desejar, por favor crie um novo arquivo";
                IsResponse = true;
                IsSuccess = false;
                return null;
            }
            catch (System.ArgumentNullException e)
            {
                message = e.Message;
                IsResponse = true;
                IsSuccess = false;
                return null;
            }
        }//fim GetXmlFile
        public void OnPostXmlMinimize(string text)
        {
            
            string dataFile = text;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            text1 = doc.OuterXml;

        }
        public void OnPostJsonMinimize(string text)
        {
            string dataFile = text;
            var obj = JsonConvert.DeserializeObject(dataFile);
            text1 = JsonConvert.SerializeObject(obj);


        }
        public void OnPostXmlIdent(string text)
        {
            
            string dataFile = text;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            XDocument textXml = XDocument.Parse(doc.OuterXml);//automaticamente identa
            text1 = textXml.ToString();

        }
        public void OnPostJsonIdent(string text)
        {
            string dataFile = text;
            text1 = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(dataFile), Newtonsoft.Json.Formatting.Indented);

        }
        public void OnPostXmlToJson(string text)
        {
            string dataFile = text;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            text1 = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
        }
        public void OnPostJsonToXml(string text)
        {

            string dataFile = text;
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(dataFile);
            doc.PreserveWhitespace = true;
            //identação do XML
            XDocument textXml = XDocument.Parse(doc.OuterXml);//automaticamente identa
            text1 = textXml.ToString();
        }
        public void OnPostJstreeToData(string jstreeData, string document)
        {
            string file = SetFilePath(); //tentei de todas as formas, porém a melhor forma é es
            text2 = "";
            text2 += textUpdate.ConvertJstree(ref message, document, jstreeData);
            if (message != "")
            {
                if (document == "xml")
                    file += ".xml";
                else
                    file += ".json";
                string dataFile = System.IO.File.ReadAllText(file);
                text1 = dataFile;
                IsResponse = true;
                IsSuccess = false;
            }
            else
            {
                text1 = text2;
            }
        }
        public void OnPostFileFromStream(string link)
        {

            try
            {
                byte[] text = null;
                using (var wc = new System.Net.WebClient())
                    text = wc.DownloadData(link);
                string finalText = System.Text.Encoding.Default.GetString(text);
                text2 += textUpdate.UpdateFile(environment, SetFilePath(), finalText, ref message);
                if (message != "")
                {

                    text1 = textUpdate.NewXmlFile(environment, fileName, archiveName);
                    IsResponse = true;
                    IsSuccess = false;
                }
                else
                {
                    text1 = text2;
                }
            }
            catch (Exception ex)
            {
                message = "URL ou arquivo inválido";
                IsResponse = true;
                IsSuccess = false;
            }



        }

        //criação save file
    }
}
