using AjaxControlToolkit;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Web.Helpers;
using System.Web.Razor.Generator;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace XmlWebEditor.Controller
{
    public class TextMananger
    {
        public string VefiryXml(IFormFile Upload, IWebHostEnvironment environment, string fileName,string archiveName, ref string error)
        {

            var file = Path.Combine(environment.ContentRootPath, archiveName, fileName + ".xml");
            string xml = "";
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
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                xml = XElement.Parse(xml).ToString();
                var myFile = File.Create(file);
                myFile.Close();
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    byte[] information = new UTF8Encoding(true).GetBytes(xml);
                    fileStream.Write(information, 0, information.Length);
                    fileStream.Dispose();
                }
                return xml;

            }
            catch (XmlException e)
            {
                if (e.Message == "Data at the root level is invalid. Line 1, position 1.")
                {
                    error = "documento inválido. por favor, digite um documento XML válido";
                    return xml;
                }
                error = "erro na linha " + e.LineNumber + " na posição: " +
                    e.LinePosition + " o erro:  " + e.Message;
                return xml;
            }

        }//end XmlVerification
        public string VerifyJson(IFormFile Upload, IWebHostEnvironment environment, string fileName,string archiveName, ref string error)
        {

            var file = Path.Combine(environment.ContentRootPath, archiveName, fileName + ".json");
            string json = "";
            try
            {
                using (var reader = new StreamReader(Upload.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        json += reader.ReadLine();
                    }
                    reader.Dispose();
                }
                dynamic jsonAux = JsonConvert.DeserializeObject(json);

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    byte[] information = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(jsonAux, Newtonsoft.Json.Formatting.Indented));
                    fileStream.Write(information, 0, information.Length);
                    fileStream.Dispose();
                }
                return JsonConvert.SerializeObject(jsonAux, Newtonsoft.Json.Formatting.Indented);

            }
            catch (JsonReaderException e)
            {
                if (e.Message == "Data at the root level is invalid. Line 1, position 1.")
                {
                    error = "documento inválido. por favor, digite um documento Json válido";
                    return json;
                }
                error = e.Message;
                return json;
            }
        }//end VerifyJson
        public string SetFile(IFormFile Upload, IWebHostEnvironment environment, string fileName,string archiveName, ref string error)
        {
            try
            {
                string contentType = Upload.ContentType;
                if (contentType == "text/xml" || contentType == "application/xml")
                    return VefiryXml(Upload, environment, fileName,archiveName, ref error);
                else if (contentType == "text/json" || contentType == "application/json")
                    return VerifyJson(Upload, environment, fileName, archiveName, ref error);
                else
                    throw new System.NullReferenceException();
            }
            catch (System.NullReferenceException)
            {
                error = "Por favor, insira um documento Xml ou Json";
                return "";
            }
        }//end SetXmlFile
        public string NewXmlFile(IWebHostEnvironment environment, string fileName,string archiveName)
        {
            string aux = "";
            var file = Path.Combine(environment.ContentRootPath, archiveName, fileName);
            var newFile = Path.Combine(environment.ContentRootPath, archiveName, "NewFile.xml");
            File.Copy(newFile, file, true);
            aux = File.ReadAllText(newFile);
            return aux;
        }// end NewXmlFile
        public string NewJsonFile(IWebHostEnvironment environment, string fileName,string archiveName)
        {
            string aux = "";
            var file = Path.Combine(environment.ContentRootPath, archiveName, fileName);
            var newFile = Path.Combine(environment.ContentRootPath, archiveName, "NewFile.json");
            File.Copy(newFile, file, true);
            aux = File.ReadAllText(newFile);
            return aux;
        }// end NewXmlFile
        public string UpdateFile(IWebHostEnvironment environment, string fileName,string archiveName, string xmlText, ref string error)
        {
            try
            {

                if (xmlText.StartsWith("<"))
                {

                    return UpdateXmlFile(environment, fileName,archiveName, xmlText, ref error);
                }
                else if (xmlText.StartsWith("{") || xmlText.StartsWith("["))
                {
                    return UpdateJsonFile(environment, fileName,  archiveName, xmlText, ref error);
                }
                else
                    throw new System.NullReferenceException();
            }
            catch (System.NullReferenceException)
            {
                error = "Por favor, digite um  Xml ou Json";
                return xmlText;
            }

        }//end UpdateFile
        public string UpdateXmlFile(IWebHostEnvironment environment, string fileName, string archiveName, string xmlText, ref string error)
        {
            string xml = xmlText;
            try
            {

                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                xml = XElement.Parse(xml).ToString();
                var file = Path.Combine(environment.ContentRootPath, archiveName, fileName + ".xml");
                var fileAux = Path.Combine(environment.ContentRootPath, archiveName, "fileAux.xml");
                File.WriteAllTextAsync(fileAux, xmlText);
                /*Resumidamente: File.WriteallTextAsync necessita de tempo
                 * para atualizar e então o document.load poder ser utilizado, por isto:
                 * */
                //Thread.Sleep(100);
                var myFile = File.Create(file);
                myFile.Close();
                File.Copy(fileAux, file, true);
                return xml;

            }
            catch (XmlException e)
            {
                //senão não envia a cópia do File.copy
                if (e.Message == "Data at the root level is invalid. Line 1, position 1.")
                {
                    error = "documento inválido. por favor, digite um documento XML válido";
                    return xml;
                }
                error = "erro na linha " + e.LineNumber + " na posição: " +
                    e.LinePosition + " o erro:  " + e.Message;
                return xml;
            }
        }//end UpdateXmlFile
        public string UpdateJsonFile(IWebHostEnvironment environment, string fileName, string archiveName, string xmlText, ref string error)
        {
            string json = xmlText;
            dynamic jsonAux;
            try
            {

                jsonAux = JsonConvert.DeserializeObject(json);
                json = JsonConvert.SerializeObject(jsonAux, Newtonsoft.Json.Formatting.Indented);
                var file = Path.Combine(environment.ContentRootPath, archiveName, fileName + ".json");
                var fileAux = Path.Combine(environment.ContentRootPath, archiveName, "fileAux.json");
                File.WriteAllTextAsync(fileAux, xmlText);
                /*Resumidamente: File.WriteallTextAsync necessita de tempo
                 * para atualizar e então o document.load poder ser utilizado, por isto:
                */
                //Thread.Sleep(100);
                //obs: por enquanto aos testes não fora mais necessário, porém deixo aqui "em caso de" 
               
                var myFile=File.Create(file);
                myFile.Close();
                File.Copy(fileAux, file, true);
                return json;

            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                //senão não envia a cópia do File.copy
                if (e.Message == "Data at the root level is invalid. Line 1, position 1.")
                {
                    error = "documento inválido. por favor, digite um documento XML válido";
                    return json;
                }
                error = "erro na linha " + e.LineNumber + " na posição: " +
                    e.LinePosition + " o erro:  " + e.Message;
                return json;
            }
        }//end UpdateJsonFile
        private string CaptureText(dynamic data)
        {
            StringBuilder sb = new StringBuilder();
            foreach(dynamic item in data)
            {
                string chave = "", nome;
                if (data.Count >= 1 && item.children.Count>0 && (int.TryParse((string)item.text, out int ch) || item.text == "0"))
                {
                    foreach (dynamic itemChildren in data)
                    {
                        chave += CaptureText(itemChildren.children);
                        chave += ",";
                    }
                    chave=chave.Remove(chave.Length - 1, 1);//limpar a última vírgula
                    sb.Append("["+ chave+"]" );
                    break;
                }
                else if (data.Count > 1)
                {
                    string total = "";
                    foreach (dynamic itemChildren in data)
                    {
                        nome = "\"" + (string)itemChildren.text + "\"";
                        chave = CaptureText(itemChildren.children);
                        total += nome + ":" + chave + ",";
                    }
                    total = total.Remove(total.Length - 1, 1);//limpar a última vírgula
                    sb.Append("{" + total + "}");
                    break;
                }
                else if (item.children.Count >= 1)
                {
                    nome = "\"" + (string)item.text + "\"";
                    chave = CaptureText(item.children);
                    sb.Append("{" + nome + ":" + chave + "}");

                }
                else
                {
                    sb.Append(("\"" + item.text.ToString() + "\""));
                    if (data.Count > 1)
                        sb.Append(",");
                }
            }
            return sb.ToString();
        }

        private string TextToWriteJson(dynamic data)//texto feito para atualizar 
        {
            bool isContinue = false;//Para deletar o end object caso tenha continuação do caractere.
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            string chave="",nome;
            sb.Append("{");
            foreach (dynamic item in data)
            {
                    
                nome = "\""+(string)item.text+ "\"";
                chave = CaptureText(item.children);
                sb.Append( nome+":" +  chave);
                sb.Append(",");
               

            }
            sb.Remove(sb.Length - 1, 1);//deleta a ultima virgula
            sb.Append("}");



            return sb.ToString();
       }
        
    
            
        public string ConvertJstree( ref string error,string document,string jstree)
        {
            try
            {
                XmlDocument auxdoc = new XmlDocument();
                // parece que não dá para chamar o descerialize dentro de outro descerialize, por isto:
                dynamic jsonAux = JsonConvert.DeserializeObject(jstree);
                //colocado um adicional para verificar o retorno do arquivo em txt na depuração
                string updatedJson = TextToWriteJson(jsonAux);
                //abaixo está escrito assim para ser devolvido identado e sem erros (deserialize realinha os colchetes, serialize devolve em string e o json format identa


                if (document == "xml")
                {
                    XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(updatedJson);
                    doc.PreserveWhitespace = true;
                    //identação do XML
                    XDocument text = XDocument.Parse(doc.OuterXml);//automaticamente identa
                    return text.ToString();
                }
                string finalJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(updatedJson), Newtonsoft.Json.Formatting.Indented);

                return finalJson;
            }
            catch 
            {
                error = "erro de edição. Verifique se o arquivo está conectado corretamente";
                return "";
            }
        }


    }//end TextMananger

}//end namespace
