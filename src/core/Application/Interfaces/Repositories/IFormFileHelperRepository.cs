using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemVariable;

namespace Application.Interfaces.Repositories
{
    public interface IFormFileHelperRepository
    {
        string GetFileTemplate(string nameFile, string path = "", string folder = SystemVariableHelper.TemplateWord);
        string UploadedFile(IFormFile Image, string name, string path,bool rename = true);
        string UploadedFile(string base64, string nameFile, string path);
        string UploadedListFile(IList<IFormFile> collection,  string path);
        string ImagetoBase64(string filename, string pathfile);
        string ImagetoBase64(IFormFile filename);
        bool DeleteFile(string fileName, string path);
        bool DeleteFile(string fullpath);
        bool DeleteListFile(string[] fileName, string path);
    }
}
