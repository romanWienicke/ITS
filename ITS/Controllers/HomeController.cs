using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Pdf()
        {
            var pdfBytes = GetPdf(RenderViewToString("Index", null));
            return File(pdfBytes, "application/pdf", "example_document.pdf");
        }

        public string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private byte[] GetPdf(string html)
        {
            //MemoryStream stream = new MemoryStream();
            //using (Document document = new Document())
            //{
            //    //html - we can provide here any HTML, for example one rendered from Razor view
            //    StringReader htmlReader = new StringReader(html);
            //    PdfWriter writer = PdfWriter.GetInstance(document, stream);
            //    writer.PageEvent = new PdfPageEventHelper();
            //    document.SetPageSize(PageSize.A4);
            //    document.Open();
            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlReader);
            //    document.Close();
            //}
            //return stream.GetBuffer();

            //byte[] bytesArray = null;
            //using (var ms = new MemoryStream())
            //{
            //    using (var document = new Document())
            //    {
            //        using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
            //        {

            //            document.Open();
            //            using (var strReader = new StringReader(html))
            //            {
            //                //Set factories
            //                HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
            //                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
            //                //Set css
            //                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            //                cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Content/bootstrap.min.css"), true);
            //                //Export
            //                IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
            //                var worker = new XMLWorker(pipeline, true);
            //                var xmlParse = new XMLParser(true, worker);
            //                xmlParse.Parse(strReader);
            //                xmlParse.Flush();
            //            }

            //            writer.CloseStream = false;
            //            document.Close();
            //            bytesArray = ms.ToArray();
            //        }
            //    }

            //}
            //return bytesArray;


            var ms = new MemoryStream();
            //var XhtmlHelper = new XhtmlToListHelper();
            var document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(iTextSharp.tool.xml.html.Tags.GetHtmlTagProcessorFactory());
            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Content/bootstrap.min.css"), true);
            //var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new ElementHandlerPipeline(XhtmlHelper, null)));//Here's where we add our IElementHandler
            var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, null)));
            var worker = new XMLWorker(pipeline, true);
            var parser = new XMLParser();
            parser.AddListener(worker);

            using (TextReader sr = new StringReader(html))
            {
                parser.Parse(sr);
            }
            return ms.ToArray();
        }
    }
}