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

namespace Axinom.ControlPanel {
    public class TreeNode {
        public string name { get; set; }
        public List<TreeNode> children { get; set; }

        public TreeNode(string n) {
            name = n;
            children = new List<TreeNode>();
        }
    }
}
