﻿using Microsoft.AspNetCore.Mvc.RazorPages;
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
        TextMananger xmlUpdate = new TextMananger();
        public void OnGet()
        {
            text1 = xmlUpdate.NewJsonFile(environment, fileName);//limpar o Json
            text1 = xmlUpdate.NewXmlFile(environment, fileName);//limpar o Xml
        }
        public void OnPostXmlTextStart()
        {
            text1 = xmlUpdate.NewXmlFile(environment, fileName);
        }
        public void OnPostJsonTextStart()
        {
            text1 = xmlUpdate.NewJsonFile(environment, fileName);
        }
        /*
         * fução que controla a entrada e saída dos dois textos
         */
        public void OnPostTextController(string text)
        {

            string aux = text;
            text1 = text;//Xml que as pessoas editam
            text2 = "";//Xml que estará disponível para a pessoa pegar. 



            this.text2 += xmlUpdate.UpdateFile(environment, fileName, aux, ref message);
            if (message != "")
            {
                IsResponse = true;
                IsSuccess = false;
            }
            else
                text1 = text2;

        }
        public void OnPost()
        {

        }
        public void OnPostXmlSetFile()//send file to the textes
        {
            if (Upload == null)
                return;
            text1 = xmlUpdate.SetXmlFile(Upload, environment, fileName, ref message);
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
        }// end GetXmlFile
    }//end class
        /*
        public List<Node> getNode()
        {

            string file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
            XmlDocument document = new XmlDocument();
            document.Load(file);
            List <Node> tree = ConvertToTree(document.DocumentElement);
            return tree;
        }
        private List<Node> ConvertToTree(XmlNode  xmlNode)
        {
            
            List<Node> nodes = new List<Node>();
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                //if (childNode.Attributes != null && childNode.Attributes["text"] != null)
                {
                    Node node = new Node
                    {
                        text = childNode.Attributes["text"].Value,
                        children = ConvertToTree(childNode)
                    };
                    nodes.Add(node);
                }
            }
            return nodes;
        }// end ConvertToTree



    }//end class
    public class Node
    {
        public string text { get; set; }
        public List<Node> children { get; set; }
    }*/

}//end namespace
