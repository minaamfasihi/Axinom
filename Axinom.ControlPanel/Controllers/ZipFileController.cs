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
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Axinom.ControlPanel.Services.Encryption;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Axinom.ControlPanel.Controllers {
    public class ZipFileController : Controller {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AESEncryptor _AESEncryptor;
        private IConfiguration configuration;

        public ZipFileController(IHttpClientFactory httpClientFactory, IConfiguration iConfig) {
            _httpClientFactory = httpClientFactory;
            configuration = iConfig;
            _AESEncryptor = new AESEncryptor(configuration.GetSection("AESKey").Value, configuration.GetSection("AESIV").Value);
        }
        [HttpPost("/upload")]
        public async Task<ActionResult> Upload(IFormFile zip) {
            try {
                if (HttpContext.Request.Form != null && HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files[0] != null) {
                    var file = HttpContext.Request.Form.Files[0];
                    if (file.ContentType == "application/zip") {
                        string extractPath = "./" + file.FileName;
                        Dictionary<string, TreeNode> mappings = new Dictionary<string, TreeNode>();
                        using (var stream = file.OpenReadStream())
                        using (ZipArchive archive = new ZipArchive(stream)) {
                            if (Directory.Exists(extractPath)) {
                                Helper.DeleteDirectory(extractPath);
                            }
                            archive.ExtractToDirectory(extractPath);
                            TreeNode root = Helper.ListDirectory(_AESEncryptor, extractPath);
                            string payload = JsonConvert.SerializeObject(root);
                            byte[] byteArr = _AESEncryptor.Encrypt(payload); 
                            using (var httpClientHandler = new HttpClientHandler())
                            {
                                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                                using (var httpclient = new HttpClient(httpClientHandler))
                                {
                                    // var results = await httpclient.PostAsJsonAsync("https://localhost:5200/api/files", payload);
                                    httpclient.DefaultRequestHeaders.Authorization = 
                                        new AuthenticationHeaderValue(
                                            "Basic", Convert.ToBase64String(
                                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                                $"{HttpContext.Request.Form["username"]}:{HttpContext.Request.Form["password"]}")));
                                    var results = await httpclient.PostAsJsonAsync("https://localhost:5200/api/files", byteArr);
                                    string resultsContent = await results.Content.ReadAsStringAsync();
                                    return Ok(resultsContent);
                                }
                            }
                        }

                        // using (var stream = file.OpenReadStream())
                        // using (ZipArchive archive = new ZipArchive(stream)) {
                            // foreach (ZipArchiveEntry item in archive.Entries) {
                            //     var nestedFiles = item.FullName.Split("/", StringSplitOptions.RemoveEmptyEntries);
                            //     bool isDir = item.FullName.EndsWith("/") ? true : false;

                            //     // is dir, save them in mappings
                            //     if (isDir) {
                            //         var parentName = nestedFiles[0] + "/";
                            //         if (!mappings.ContainsKey(parentName)) {
                            //             mappings.Add(parentName, new TreeNode {
                            //                 parent = "root",
                            //                 name = parentName,
                            //                 children = new Dictionary<string, TreeNode>()
                            //             });
                            //         }
                            //         if (nestedFiles.Length > 1) {
                            //             for (int i = 0; i < nestedFiles.Length - 1; i++) {
                            //                 var childParentName = nestedFiles[i] + "/";
                            //                 var childFileName = nestedFiles[i + 1] + "/";
                            //                 if (childFileName == "assets/") {
                            //                     Console.WriteLine("weoriu");
                            //                 }
                            //                 TreeNode child = new TreeNode { name = childFileName, parent = childParentName };
                            //                 TreeNode targetParent = null;
                            //                 for (int j = i; j > 0; j--) {
                            //                     targetParent = mappings[parentName].Search(nestedFiles[j] + "/");
                            //                     if (targetParent != null) break;
                            //                 }
                            //                 if (targetParent == null) {
                            //                     targetParent = mappings[parentName].Search(nestedFiles[i] + "/");
                            //                 }
                                            
                            //                 if (targetParent != null && !targetParent.children.ContainsKey(childFileName)) {
                            //                     targetParent.children.Add(childFileName, 
                            //                     new TreeNode {
                            //                         name = childFileName,
                            //                         parent = parentName,
                            //                         children = new Dictionary<string, TreeNode>()
                            //                     });
                            //                 }
                            //             }

                            //         }
                            //     }
                            //     // is file 
                            //     else {
                            //         if (nestedFiles.Length == 1) {
                            //             if (!mappings.ContainsKey(nestedFiles[0])) {
                            //                 mappings.Add(nestedFiles[0], new TreeNode {
                            //                     name = nestedFiles[0],
                            //                     parent = "root",
                            //                     children = new Dictionary<string, TreeNode>()
                            //                 });
                            //             }
                            //         } else {
                            //             var parentName = nestedFiles[0] + "/";
                            //             if (!mappings.ContainsKey(parentName)) {
                            //                 mappings.Add(parentName, new TreeNode {
                            //                     parent = "root",
                            //                     name = parentName,
                            //                     children = new Dictionary<string, TreeNode>()
                            //                 });
                            //             }
                            //             for (int i = 0; i < nestedFiles.Length - 1; i++) {
                            //                 var childParentName = nestedFiles[i] + "/";
                            //                 var childFileName = nestedFiles[i + 1];
                            //                 if (i + 1 < nestedFiles.Length - 1) {
                            //                     childFileName += "/";
                            //                 }
                            //                 TreeNode child = new TreeNode { name = childFileName, parent = childParentName };
                            //                 TreeNode targetParent = null;
                            //                 for (int j = i; j >= 0; j--) {
                            //                     targetParent = mappings[parentName].Search(nestedFiles[j] + "/");
                            //                     if (targetParent != null) break;
                            //                 }
                            //                 if (targetParent == null) {
                            //                     targetParent = mappings[parentName].Search(nestedFiles[i] + "/");
                            //                 }
                            //                 if (targetParent != null && !targetParent.children.ContainsKey(childFileName)) {
                            //                     targetParent.children.Add(childFileName, 
                            //                     new TreeNode {
                            //                         name = childFileName,
                            //                         parent = childParentName,
                            //                         children = new Dictionary<string, TreeNode>()
                            //                     });
                            //                 }
                            //             }
                            //         }
                            //     }
                            //     // has no parent dir, and is a file/or empty dir, add as is
                            // }
                            // string output = JsonConvert.SerializeObject(mappings);
                            // return output;
                        // }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
            return Content("Some error occurred");
        }
    }
}
