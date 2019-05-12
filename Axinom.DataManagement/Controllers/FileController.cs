using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Axinom.DataManagement.Services.Decryption;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Web;
using Microsoft.Extensions.Primitives;
using System.Text;
using Axinom.DataManagement.Commons;

namespace Axinom.DataManagement.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AESDecryptor _AESDecryptor;
        private IConfiguration configuration;
        public FilesController(IConfiguration iConfig) {
            configuration = iConfig;
            _AESDecryptor = new AESDecryptor(configuration.GetSection("AESKey").Value, configuration.GetSection("AESIV").Value);
        }
        [HttpPost]
        public IActionResult Post([FromBody] string filenames)
        {
            string path = "./filenames.json";
            string err = "";
            try {
                StringValues auth;
                HttpContext.Request.Headers.TryGetValue("Authorization", out auth);
                string authHeader = auth.First();
                string username; string password;
                Helper.GetUsernamePassword(authHeader, out username, out password);
                if (username == configuration.GetSection("uname").Value && password == configuration.GetSection("password").Value) {
                    byte[] byteArr = Convert.FromBase64String(filenames);
                    string payload = _AESDecryptor.Decrypt(byteArr);
                    TreeNode root = JsonConvert.DeserializeObject<TreeNode>(payload);
                    System.IO.File.WriteAllText(path, payload);
                    return Ok("File was successfully decrypted and saved");
                } else {
                    err = "Incorrect username/password";
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                err = "Some error occurred";
            }
            return BadRequest(err);
        }
    }
}
