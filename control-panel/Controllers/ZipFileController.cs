using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace control_panel {
    public class ZipFileController : Controller {
        [HttpPost("/upload")]
        public string Upload(IFormFile zip) {
            try {
                if (HttpContext.Request.Form != null && HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files[0] != null) {
                    var file = HttpContext.Request.Form.Files[0];
                    if (file.ContentType == "application/zip") {
                        using (var stream = file.OpenReadStream())
                        using (ZipArchive archive = new ZipArchive(stream)) {
                            foreach (ZipArchiveEntry item in archive.Entries) {
                                Console.WriteLine("Name: {0}", item.FullName);
                            }
                        }
                    }
                }
                return "View()";
            }
            catch (Exception ex) {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
            return "Yayy";
        }
    }
}
