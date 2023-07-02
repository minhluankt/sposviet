using Application.Constants;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemVariable;

namespace Infrastructure.Infrastructure.Repositories
{
    public class FormFileHelperRepository : IFormFileHelperRepository
    {
        private readonly ILogger<FormFileHelperRepository> _log;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public FormFileHelperRepository(IHostingEnvironment hostingEnvironment, ILogger<FormFileHelperRepository> log)
        {
            _log = log;
            _hostingEnvironment = hostingEnvironment;
        }

        [Obsolete]
        public string UploadedFile(IFormFile Image, string name, string path, bool rename = true)
        {
            try
            {
                string uniqueFileName = null;
                if (Image != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, SystemVariableHelper.FolderUpload + path);
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    //  uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    if (rename)
                    {
                        uniqueFileName = name + Guid.NewGuid().ToString() + "_" + Image.FileName;
                    }
                    else
                    {
                        uniqueFileName = name + Image.FileName;
                    }
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(fileStream);
                    }
                }

                return uniqueFileName;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                throw new ArgumentException(e.Message, e);
            }

        }
        public string UploadedListFile(IList<IFormFile> collection, string path)
        {
            try
            {
                string uniqueFileName = string.Empty;
                if (collection.Count() > 0)
                {
                    foreach (var Image in collection)
                    {
                        if (Image != null)
                        {
                            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/" + path);
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }
                            //  uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                            string FileName = Guid.NewGuid().ToString() + "_" + Image.FileName;
                            string filePath = Path.Combine(uploadsFolder, FileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                Image.CopyTo(fileStream);
                            }
                            uniqueFileName += FileName + "$";
                        }

                    }

                }
                uniqueFileName = uniqueFileName.Remove(uniqueFileName.LastIndexOf("$"));
                return uniqueFileName;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                throw new ArgumentException(e.Message, e);
            }

        }

        [Obsolete]
        public bool DeleteFile(string fileName, string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, SystemVariableHelper.FolderUpload + path + "/" + fileName);
                    if (File.Exists(uploadsFolder))
                    {
                        File.Delete(uploadsFolder);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                throw new ArgumentException(e.Message, e);
            }

        }
        public bool DeleteListFile(string[] collection, string path)
        {
            try
            {
                foreach (var fileName in collection)
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/" + path + "/" + fileName);
                        if (File.Exists(uploadsFolder))
                        {
                            File.Delete(uploadsFolder);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                throw new ArgumentException(e.Message, e);
            }

        }

        public string GetFileTemplate(string nameFile, string path = "", string folder = SystemVariableHelper.TemplateWord)
        {
            string filename = Path.Combine(_hostingEnvironment.WebRootPath, folder + path +"/"+ nameFile);
            if (File.Exists(filename))
            {
                return filename;
            }
            return string.Empty;
        }
      
          public string GetContentFile(string nameFile, string path = "", string folder = SystemVariableHelper.FolderUpload)
        {
            string filename = Path.Combine(_hostingEnvironment.WebRootPath, folder + path +"/"+ nameFile);
            if (File.Exists(filename))
            {
                return File.ReadAllText(filename);
            }
            return string.Empty;
        }
      

        public bool DeleteFile(string fullpath)
        {
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
                return true;
            }
            return false;
        }

        [Obsolete]
        public string UploadedFile(string base64, string name, string path)
        {
            string pathname = Path.Combine(_hostingEnvironment.WebRootPath, SystemVariableHelper.FolderUpload + path);
            if (!Directory.Exists(pathname))
            {
                Directory.CreateDirectory(pathname);
            }
            string imgPath = Path.Combine(pathname, name);
            var imageBytes = Convert.FromBase64String(base64);
            var imagefile = new FileStream(imgPath, FileMode.Create);
            imagefile.Write(imageBytes, 0, imageBytes.Length);
            imagefile.Flush();
            imagefile.Close();
            return name;
        }

        [Obsolete]
        public string ImagetoBase64(string filename, string pathfile)
        {
            byte[] base64 = null;
            if (!string.IsNullOrEmpty(filename))
            {
                string path = Path.Combine(_hostingEnvironment.WebRootPath, SystemVariableHelper.FolderUpload + pathfile);
                if (Directory.Exists(path))
                {
                    path = Path.Combine(path, filename);
                    if (!File.Exists(path))
                    {
                        return ConfigCustomerLogin.ImageDefault;
                    }
                }
                else
                {
                    _log.LogError("Không tìm thấy đường dẫn: "+ path);
                    return string.Empty;
                }
                using (var stream = System.IO.File.OpenRead(path))
                {
                    FormFile file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                    using (var stream1 = file.OpenReadStream())
                    {
                        using (var image = Image.Load(stream1))
                        {
                            using (var writeStream = new MemoryStream())
                            {
                                image.SaveAsPng(writeStream);
                                base64 = writeStream.ToArray();
                            }
                        }
                        stream1.Close();
                    }
                    stream.Close();
                };
            }
            if (base64!=null)
            {
                return Convert.ToBase64String(base64);
            }
            _log.LogError("Không lấy được dữ liệu: " + filename);
            return string.Empty;
        } 
        public string ImagetoBase64(IFormFile filename)
        {
            byte[] base64 = null;
            if (filename!=null)
            {
                using (var stream1 = filename.OpenReadStream())
                {
                    using (var image = Image.Load(stream1))
                    {
                        using (var writeStream = new MemoryStream())
                        {
                            image.SaveAsPng(writeStream);
                            base64 = writeStream.ToArray();
                        }
                    }
                    stream1.Close();
                }
            }
            if (base64!=null)
            {
                return Convert.ToBase64String(base64);
            }
            return string.Empty;
        }
    }
}

