using System.Web.Razor.Parser.SyntaxTree;
using System.Xml;

namespace XmlWebEditor.Controller
{
    public class TreeViewer
    {
        public void XmlTreeView(IFormFile Upload, IWebHostEnvironment environment, string fileName)
        {
            var file = Path.Combine(environment.ContentRootPath, "xml", fileName + ".xml");
            XmlDocument document= new XmlDocument();
            document.LoadXml(file);
            SyntaxTreeNode node;

        }
    }
}
