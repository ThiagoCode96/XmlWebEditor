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
        Guid cookieID = Guid.NewGuid();
        private string archiveUserName = "user"; //Serve para demonstrar aonde está o local do usuário
        
        TextMananger textUpdate = new TextMananger();
        private void ArchiveDelete(string fileName)
        {
            var filePath = fileName;
            if (Directory.Exists(filePath))
            {
                foreach (string file in Directory.GetFiles(filePath))
                {
                    System.IO.File.Delete(file);
                }
                System.IO.Directory.Delete(filePath);
            }
        }
        private string SetUserId()
        {
            string cookie = "";
            cookie = Request.Cookies["userId"];
            if (cookie == null) 
            {
                Response.Cookies.Append("UserId", cookieID.ToString());
                return cookieID.ToString();
            }
            return cookie;
        }
        private string SetFilePath()//set file apenas falará QUAL FILE É. Se deseja pegar o json ou o xml terá que fazer file=setfile()+(".json" ou ".xml") 
        {
            string cookieName = SetUserId();
            string file=Path.Combine(environment.ContentRootPath, archiveName,archiveUserName,cookieName);
            file = Path.Combine(file, fileName);
            return file;
        }
        public void OnGet()
        {
            
            text1 = textUpdate.NewJsonFile(environment, fileName, archiveName);//limpar o Json
            SaveFile(text1);//limpar o save file
        }
        public void OnPostXmlTextStart()
        {
            text1 = textUpdate.NewXmlFile(environment, fileName, archiveName);
            SaveFile(text1);
        }
        public void OnPostJsonTextStart()
        {
            string idCookie = SetUserId();
            text1 = textUpdate.NewJsonFile(environment, fileName, archiveName);
            SaveFile(text1);
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
            text1 = textUpdate.SetFile(Upload, environment,SetFilePath(), ref message);
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
            string file=SetFilePath()+".json";
            try
            {
                
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
            string file=SetFilePath()+".xml";
            try
            {

                
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
            var file =SetFilePath()+".xml";
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            text1 = doc.OuterXml;
            SaveFile(text1);
        }
        public void OnPostJsonMinimize()
        {
            var file = SetFilePath() + ".json";
            string dataFile = System.IO.File.ReadAllText(file);
            var obj = JsonConvert.DeserializeObject(dataFile);
            text1 = JsonConvert.SerializeObject(obj);
            SaveFile(text1);

        }
        public void OnPostXmlIdent()
        {
            var file = SetFilePath() + ".xml";
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            XDocument text = XDocument.Parse(doc.OuterXml);//automaticamente identa
            text1 = text.ToString();
            SaveFile(text1);
        }
        public void OnPostJsonIdent()
        {
            var file = SetFilePath() + ".json";
            string dataFile = System.IO.File.ReadAllText(file);
            text1 = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(dataFile), Newtonsoft.Json.Formatting.Indented);

        }
        public void OnPostXmlToJson()
        {
            var file = SetFilePath() + ".xml";
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataFile);
            text1 = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
            SaveFile(text1);
        }
        public void OnPostJsonToXml()
        {
            var file = SetFilePath() + ".json";
            string dataFile = System.IO.File.ReadAllText(file);
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(dataFile);
            doc.PreserveWhitespace = true;
            //identação do XML
            XDocument text = XDocument.Parse(doc.OuterXml);//automaticamente identa
            text1 = text.ToString();
            SaveFile(text1);
        }
        public void OnPostJstreeToData(string jstreeData, string document)
        {
           string file= SetFilePath(); //tentei de todas as formas, porém a melhor forma é es
            text2 = "";
            text2 += textUpdate.ConvertJstree( ref message, document, jstreeData);
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
                SaveFile(text1);
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
                text2 += textUpdate.UpdateFile(environment,SetFilePath(), finalText, ref message);
                if (message != "")
                {

                    text1 = textUpdate.NewXmlFile(environment, fileName, archiveName);
                    IsResponse = true;
                    IsSuccess = false;
                }
                else
                {
                    text1 = text2;
                    SaveFile(text1);
                }
            }catch(Exception ex)
            {
                message = "URL ou arquivo inválido";
                IsResponse = true;
                IsSuccess = false;
            }

           

        }

        //criação save file
        private void SaveFile(string text)
        {
            string cookieName = SetUserId();
            var pathFile = Path.Combine(environment.ContentRootPath, archiveName , archiveUserName,cookieName);
            var file = SetFilePath();
            
            if (text.StartsWith("<"))
            {
                file += ".xml";
                
                if (!Directory.Exists(pathFile))
                {
                    Directory.CreateDirectory(pathFile);
                }
                if (!System.IO.File.Exists(file))
                {
                    var myFile = System.IO.File.Create(file);
                    myFile.Close();
                }
            }
            else
            {
                file += ".json";
                if (!Directory.Exists(pathFile))
                {
                    Directory.CreateDirectory(pathFile);
                }
                if (!System.IO.File.Exists(file))
                {
                    var myFile = System.IO.File.Create(file);
                    myFile.Close();
                }
                

            }
            //resumidamente, para não dar problemas com o atualizador
            Thread.Sleep(100);
            System.IO.File.WriteAllText(file, text);
        }
        //timed file
        public void GetTimedFile()
        {
            string file=Path.Combine(environment.ContentRootPath, archiveName, archiveUserName);
            DateTime modification;
            DateTime today= DateTime.Now;
            foreach (string files in Directory.GetDirectories(file))
            {
                modification = System.IO.File.GetLastWriteTime(files) + TimeSpan.FromMinutes(5);
                if (modification < today)
                   ArchiveDelete(files);
            }
                
        }

    }//Fim Classe IndexModel

    ///////////////////////////////////////
    ////////new timed host service/////////
    ///////////////////////////////////////
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> logger;
        private Timer? timer = null;
        IndexModel index;
        public TimedHostedService(ILogger<TimedHostedService> _logger, ILogger<IndexModel> _logger2, IWebHostEnvironment _environment)
        {
            logger = _logger;
            index = new IndexModel(_logger2, _environment);
        }
       

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("starting Timed clean");
            timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }
        ///parte para modificar
        private async void DoWork(object? state)
        {
            logger.LogInformation("Timed Background Service is working.");
            index.GetTimedFile();
        }
        /// fim da modificação
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
