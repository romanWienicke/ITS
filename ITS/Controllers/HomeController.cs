using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
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
using System.Text.RegularExpressions;
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


            //var ms = new MemoryStream();
            ////var XhtmlHelper = new XhtmlToListHelper();
            //var document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, ms);
            //var htmlContext = new HtmlPipelineContext(null);
            //htmlContext.SetTagFactory(iTextSharp.tool.xml.html.Tags.GetHtmlTagProcessorFactory());
            //var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false); 
            //cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Content/bootstrap.min.css"), true);
            ////var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new ElementHandlerPipeline(XhtmlHelper, null)));//Here's where we add our IElementHandler
            //var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, null)));
            //var worker = new XMLWorker(pipeline, true);
            //var parser = new XMLParser();
            //parser.AddListener(worker);

            //using (TextReader sr = new StringReader(html))
            //{
            //    parser.Parse(sr);
            //}
            //ms.Position = 0;
            //return ms.ToArray();

            using (var ms = new MemoryStream())
            {
                html = FormatImageLinks(html);
                Document pdfDocument = new Document(PageSize.A3, 45, 5, 5, 5);
                //PdfWriter writer = PdfWriter.GetInstance(pdfDocument, Response.OutputStream);
                PdfWriter writer = PdfWriter.GetInstance(pdfDocument, ms);

                pdfDocument.Open();

                HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

                // Set css
                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                cssResolver.AddCssFile(HttpContext.Server.MapPath("~/Content/print.css"), true);
                IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(pdfDocument, writer)));

                XMLWorker worker = new XMLWorker(pipeline, true);
                XMLParser xmlParse = new XMLParser(true, worker);




                using (TextReader sr = new StringReader(html))
                {
                    xmlParse.Parse(sr);
                }
                xmlParse.Flush();
                pdfDocument.Close();
                return ms.ToArray();
            }
            //Response.Write(pdfDocument);

        }

        public static string FormatImageLinks(string input)

        {

            if (input == null)

                return string.Empty;

            string tempInput = input;

            const string pattern = @"<img(.|\n)+?>";

            HttpContext context = System.Web.HttpContext.Current;




            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.

            foreach (Match m in Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft))

            {

                if (m.Success)

                {

                    string tempM = m.Value;

                    string pattern1 = "src=[\'|\"](.+?)[\'|\"]";

                    Regex reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    Match mImg = reImg.Match(m.Value);




                    if (mImg.Success)

                    {

                        string src = mImg.Value.ToLower().Replace("src=", "").Replace("\"", "").Replace("\'", "");




                        if (!src.StartsWith("http://") && !src.StartsWith("https://"))

                        {

                            //Insert new URL in img tag

                            src = "src=\"" + context.Request.Url.Scheme + "://" +

                                  context.Request.Url.Authority + src + "\"";

                            try

                            {

                                tempM = tempM.Remove(mImg.Index, mImg.Length);

                                tempM = tempM.Insert(mImg.Index, src);




                                //insert new url img tag in whole html code

                                tempInput = tempInput.Remove(m.Index, m.Length);

                                tempInput = tempInput.Insert(m.Index, tempM);

                            }

                            catch (Exception)

                            {




                            }

                        }

                    }

                }

            }

            return tempInput;

        }

    }
}
