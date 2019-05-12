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
using Axinom.ControlPanel.Services.Encryption;
using System.Text;

namespace Axinom.ControlPanel {
    public class Helper {
        public static TreeNode CreateDirectoryNode(AESEncryptor aesEncryptor, DirectoryInfo directoryInfo) {
            var node = new TreeNode(directoryInfo.Name);
            // var node = new TreeNode(encryptedName);
            foreach (var directory in directoryInfo.GetDirectories()) {
                node.children.Add(CreateDirectoryNode(aesEncryptor, directory));
            }
            
            foreach (var file in directoryInfo.GetFiles()) {
                node.children.Add(new TreeNode(file.Name));
            }
            return node;
        }

        public static TreeNode ListDirectory(AESEncryptor aesEncryptor, string fileName) {
            var rootDirectoryInfo = new DirectoryInfo(fileName);
            // var buffer = aesEncryptor.Encrypt(fileName);
            // var encryptedName = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            TreeNode root = new TreeNode(fileName);
            root.children.Add(CreateDirectoryNode(aesEncryptor, rootDirectoryInfo));
            return root;
        }

        public static void DeleteDirectory(string targetDir)
        {
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }
    }
}
