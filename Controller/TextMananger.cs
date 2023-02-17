using AjaxControlToolkit;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Web.Helpers;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace XmlWebEditor.Controller
{
    public class TextMananger
    {
        public string VefiryXml(IFormFile Upload, IWebHostEnvironment environment, string fileName,ref string error)
        {
            
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName+".xml");
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
                if(e.Message== "Data at the root level is invalid. Line 1, position 1.")
                {
                    error = "documento inválido. por favor, digite um documento XML válido";
                    return xml;
                }
                error= "erro na linha " + e.LineNumber + " na posição: " +
                    e.LinePosition + " o erro:  " + e.Message;
                return xml;
            }
            
        }//end XmlVerification
        public string VerifyJson(IFormFile Upload, IWebHostEnvironment environment, string fileName, ref string error)
        {

            var file = Path.Combine(environment.ContentRootPath, "xml", fileName+".json");
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
        public string SetFile(IFormFile Upload, IWebHostEnvironment environment, string fileName,ref string error)
        {
            try
            {
                string contentType = Upload.ContentType;
                if (contentType == "text/xml" || contentType == "application/xml")
                    return VefiryXml(Upload, environment, fileName, ref error);
                else if (contentType == "text/json" || contentType == "application/json")
                    return VerifyJson(Upload, environment, fileName, ref error);
                else
                    throw new System.NullReferenceException();
            }
            catch (System.NullReferenceException)
            {
                error="Por favor, insira um documento Xml ou Json";
                return "";
            }
        }//end SetXmlFile
        public string NewXmlFile(IWebHostEnvironment environment, string fileName)
        {
            string aux = "";
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName);
            var newFile = Path.Combine(environment.ContentRootPath, "xml", "NewFile.xml");
            File.Copy(newFile,file,true);
            aux=File.ReadAllText(newFile);
            return aux;
        }// end NewXmlFile
        public string NewJsonFile(IWebHostEnvironment environment, string fileName)
        {
            string aux = "";
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName);
            var newFile = Path.Combine(environment.ContentRootPath, "xml", "NewFile.json");
            File.Copy(newFile, file, true);
            aux = File.ReadAllText(newFile);
            return aux;
        }// end NewXmlFile
        public string UpdateFile(IWebHostEnvironment environment, string fileName,string xmlText, ref string error)
        {
            try
            {

                if (xmlText.StartsWith("<"))
                {
                   
                    return UpdateXmlFile(environment, fileName, xmlText, ref error);
                }
                else if (xmlText.StartsWith("{") || xmlText.StartsWith("["))
                {
                    return UpdateJsonFile(environment, fileName, xmlText, ref error);
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
        public string UpdateXmlFile(IWebHostEnvironment environment, string fileName, string xmlText, ref string error)
        {
            string xml = xmlText;
            try
            {

                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                xml = XElement.Parse(xml).ToString();
                var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
                var fileAux = Path.Combine(environment.ContentRootPath, "xml", "fileAux.xml");
                File.WriteAllTextAsync(fileAux, xmlText);
                /*Resumidamente: File.WriteallTextAsync necessita de tempo
                 * para atualizar e então o document.load poder ser utilizado, por isto:
                 * */
                //Thread.Sleep(100);
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
        public string UpdateJsonFile(IWebHostEnvironment environment, string fileName, string xmlText, ref string error)
        {
            string json = xmlText;
            dynamic jsonAux;
            try
            {

                jsonAux = JsonConvert.DeserializeObject(json);
                json = JsonConvert.SerializeObject(jsonAux, Newtonsoft.Json.Formatting.Indented);
                var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".json");
                var fileAux = Path.Combine(environment.ContentRootPath, "xml", "fileAux.json");
                File.WriteAllTextAsync(fileAux, xmlText);
                /*Resumidamente: File.WriteallTextAsync necessita de tempo
                 * para atualizar e então o document.load poder ser utilizado, por isto:
                */
                //Thread.Sleep(100);
                //obs: por enquanto aos testes não fora mais necessário, porém deixo aqui "em caso de" 
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

       
        private string textToWriteJson(dynamic data)//texto feito para atualizar 
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                foreach (var father in data)
                {
                    if (father.children.Count < 1)
                    {
                        sb.Append(", ");
                        writer.WriteValue(father.text);
                        continue;
                    }
                    else
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName((string)father.text);
                    }
                    if (father.children.Count > 1)
                    {
                        writer.WriteStartArray();
                        var variable = textToWriteJson(father.children);
                        
                        sb.Append(variable);
                        writer.WriteEndArray();
                    }
                    else
                    foreach (var child in father.children)
                    {
                        if (child.children.Count>1)
                        {
                            writer.WriteStartArray();
                            var variable = textToWriteJson(father.children);
                            sb.Append(variable);
                            writer.Flush();
                            writer.WriteEndArray();
                            }
                        else if (child.children.Count==1)
                        {
                            var variable = textToWriteJson(father.children);
                            sb.Append(variable);
                            writer.Flush();
                        }
                        else
                        {
                                
                                writer.WriteValue(child.text);
                        }
                    }
                }
                if(!(sb.ToString()).EndsWith("}"))
                    writer.WriteEndObject();
                
            }
            string jsonstring = sb.ToString();
            return jsonstring.Replace("null","");
        }
        // FIM MINHA VERSÃO*/
        public string ConvertJstree(IWebHostEnvironment environment, string fileName, ref string error,string text1,string jstree)
        {
            XmlDocument auxdoc = new XmlDocument();
            // parece que não dá para chamar o descerialize dentro de outro descerialize, por isto:
            dynamic jsonAux = JsonConvert.DeserializeObject(jstree);
            //teste
            string finalJson = textToWriteJson(jsonAux); ; //textToWriteJson(jsonAux);
            return finalJson;
        }

    }//end TextMananger

}//end namespace
