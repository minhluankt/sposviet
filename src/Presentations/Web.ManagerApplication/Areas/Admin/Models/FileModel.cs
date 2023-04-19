using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class FileModel
    {
        public IOrderedEnumerable<FileInfo> FileAll { get; set; }
        public string Html { get; set; }
    }
}
