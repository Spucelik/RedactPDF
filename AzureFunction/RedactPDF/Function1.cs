using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Utils;
using iText.Layout;
using System.Text;
using Microsoft.VisualBasic;
using iText.IO.Source;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using iText.Kernel.Events;
using Org.BouncyCastle.Crypto.Paddings;
using System.IO.Compression;
using System.Collections.Generic;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;
using iText.PdfCleanup.Autosweep;
using iText.PdfCleanup;

using System.Text.RegularExpressions;
using iText.Kernel.Colors;

namespace RedactPDFDocument
{
    public static class ReactPDF
    {
        [FunctionName("RedactPDF")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            var file = req.Form.Files["file"];
            var formdata = await req.ReadFormAsync();

            string fileName = file.FileName + "_REDACTED.pdf";

            //var sTermsToRedact = await new StreamReader(req.Body).ReadToEndAsync();
            string sRedact = formdata["redact"].ToString();

            var outputStream = new MemoryStream();
            //var outputDocument = new PdfWriter(outputStream);
            byte[] fileOutput;

            PdfDocument pdf = new PdfDocument(new PdfReader(file.OpenReadStream()), new PdfWriter(outputStream));
            ICleanupStrategy cleanupStrategy = new RegexBasedCleanupStrategy(new Regex(@sRedact, RegexOptions.IgnoreCase)).SetRedactionColor(ColorConstants.BLACK);
            PdfCleaner.AutoSweepCleanUp(pdf, cleanupStrategy);
            pdf.Close();

            fileOutput = outputStream.ToArray();


            return new FileContentResult(fileOutput, "application/pdf")
            {
                FileDownloadName = fileName
            };
        }
    }
}
