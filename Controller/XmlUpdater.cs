using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace XmlWebEditor.Controller
{
    public class XmlUpdater
    {
        private string TextGrabber(ref string material, ref string tagType)//captura tags e textos
        {
            string tag = "";
            int n = 0;
            if (material[n] != '<')
            {
                while (material.Length > n && material[n] != '<')
                {
                    
                    tag += material[n].ToString();
                    n++;
                }
                tagType = "DT";
            }
            else
            {
                do
                {
                    
                    tag += material[n].ToString();
                    n++; 

                } while (material.Length > n && material[n - 1] != '>');
               
                

            }
            material = material.Substring(n);

            if (tag[0] == '<')
                if (tag[1] == '/')
                    tagType = "CT";
                else if (tag.Contains("/>"))
                    tagType = "FT";
                else
                    tagType = "OT";

            return tag;
        }//end textgrabber
        private string TextCleaner(ref string material)// limpa espaços e linhas
        {
            string aux="";
            while (material != "")
            {
                if (material[0] == '\t' || material[0] == '\n' || material[0] == '\r' || (material[0] == ' ' && !Char.IsLetter(material[1])))
                { //limpando os tabs e enters
                    material = material.Substring(1);
                    continue;
                }
                aux += material[0].ToString();
                material = material.Substring(1);
            }
            return aux;
        }//end textcleaner
        private string TagIdentity(string tag)
        {
            string aux =tag, identity="";
            
            while (aux != "")
            {
                if (aux[0] == '<' || aux[0] == '/' || aux[0] == '>')
                { //limpando os tabs e enters
                    aux = aux.Substring(1);
                    continue;
                }
                identity += aux[0].ToString();
                aux = aux.Substring(1);
            }
            return identity;

        }//end tag identity
        private string TextTab(int quantity)//coloca as tabs no texto
        {
            string tab = "";
            tab += "\n";
            for (int n = 0; n < quantity; n++)
            {
                tab += "\t";
            }
            return tab;
        }//end texttab
        public string TextIdentator(ref string textToIdent)//faz a verificação da identação
        {
            string tagGraberAux1 = "", tagGraberAux2 = "";
            string anterior="", agora="";
            int count=0;
            textToIdent = TextCleaner(ref textToIdent);
            tagGraberAux1=TextGrabber(ref textToIdent,ref anterior);
            string final = tagGraberAux1;
            do
            {
                if (textToIdent.Length > 0)
                {
                    tagGraberAux2 = TextGrabber(ref textToIdent,ref agora);
                    if(anterior=="OT"&& agora == "OT" || anterior == "OT" && agora == "FT")
                    {
                        count++;
                        final += TextTab(count);
                        final += tagGraberAux2;
                    }
                    else if (anterior == "CT" && agora == "CT"|| anterior=="FT"&& agora=="CT")
                    {
                        count--;
                        final += TextTab(count);
                        final += tagGraberAux2;

                    }
                    else if (anterior == "DT" && agora == "CT" || anterior == "OT" && agora=="CT" || anterior == "OT" && agora == "DT")
                        final += tagGraberAux2;
                    else
                    {
                        final += TextTab(count);
                        final += tagGraberAux2;
                    }

                    anterior = agora;
                    
                }
                else
                {
                    //throw new Exception("identation error");
                    return final;
                }
            } while (TagIdentity(tagGraberAux1) != TagIdentity(tagGraberAux2));
            return final;
        }//end tagInterator
        public string VefiryXml(IFormFile Upload, IWebHostEnvironment environment, string fileName,ref string error)
        {
            
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName);
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
                xml = TextIdentator(ref xml);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    byte[] information = new UTF8Encoding(true).GetBytes(xml);
                    fileStream.Write(information, 0, information.Length);
                    fileStream.Dispose();
                }
                
            
                XmlDocument document = new XmlDocument();
                document.Load(file);
                document.RemoveAll();//limpa o XmlDocument
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
        public string SetXmlFile(IFormFile Upload, IWebHostEnvironment environment, string fileName,ref string error)
        {


            var file = Path.Combine(environment.ContentRootPath, "xml", fileName);
            string aux = "";

            try
            {

                using (var reader = new StreamReader(Upload.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        aux += reader.ReadLine();
                    }
                    reader.Dispose();
                }
                XmlUpdater updater = new XmlUpdater();
                aux = updater.TextIdentator(ref aux);

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    byte[] information = new UTF8Encoding(true).GetBytes(aux);
                    fileStream.Write(information, 0, information.Length);
                    fileStream.Dispose();
                }
                return updater.VefiryXml(Upload, environment,fileName,ref error);
            }
            catch (System.NullReferenceException)
            {
                error="Por favor, insira um documento Xml";
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
        public string UpdateXmlFile(IWebHostEnvironment environment, string fileName,string xmlText, ref string error)
        {

            string aux=xmlText;
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName);
            var fileAux = Path.Combine(environment.ContentRootPath, "xml","fileAux.xml");
            File.WriteAllTextAsync(fileAux,xmlText);
            /*Resumidamente: File.WriteallTextAsync necessita de tempo
             * para atualizar e então o document.load poder ser utilizado, por isto:
             * */
            Thread.Sleep(100);

            try
            {

                aux = TextIdentator(ref aux);


                XmlDocument document = new XmlDocument();
                document.Load(fileAux);//Verifica se o Xml está funcional. Se estiver envia para o original
                document.RemoveAll();//limpa o XmlDocument
                File.Copy(fileAux, file, true);
                return aux;

            }
            catch (XmlException e)
            {
                //senão não envia a cópia do File.copy
                if (e.Message == "Data at the root level is invalid. Line 1, position 1.")
                {
                    error = "documento inválido. por favor, digite um documento XML válido";
                    return aux;
                }
                error = "erro na linha " + e.LineNumber + " na posição: " +
                    e.LinePosition + " o erro:  " + e.Message;
                return aux;
            }
        }//end UpdateXmlFile

    }//end XmlUpdater

}//end namespace
