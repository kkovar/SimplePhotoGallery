using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using SimplePhotoGallery.Models;
using System;
using System.Configuration;

namespace jQuery_File_Upload.MVC4.Upload
{
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        private GalleryContext db = new GalleryContext();
        private readonly JavaScriptSerializer js;

        // this should be in the image manager controller or maybe at the site level
        private string GalleryDirectory = "~/GalleryImages/";

        // todo, make the uploader work with a user specified or "Current" gallery
        private string StorageRoot
        {
            get { 
                return System.Web.HttpContext.Current.Server.MapPath(GalleryDirectory); 
            } 
            //get { 
            //    return  VirtualPathUtility.GetDirectory(GalleryDirectory); 
            //} 
            //Path should! always end with '/'
        }

        public UploadHandler()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = 41943040;
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            HandleMethod(context);
        }

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context)) DeliverFile(context);
                    else ListCurrentFiles(context);
                    break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                case "DELETE":
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            var filePath = StorageRoot + context.Request["f"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var fullName = StorageRoot + Path.GetFileName(fileName);

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(new FilesStatus(new FileInfo(fullName)));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            // hack to see if thumbnails can be added
            //Thumbnail tn = new Thumbnail();
            //tn.Description = "medium";
            //tn.MaxWidth = 600;
            //db.Thumbnails.Add(tn);
            //db.SaveChanges(); 

            // in a controller we would probably use data binding
            if (context.Request.Form["uploadDestination"] != "")
            {
                // this changes value of StorageRoot
                GalleryDirectory = "~/" + context.Request.Form["uploadDestination"] + "/";
            }
            else
            {
                GalleryDirectory = "~/" + ConfigurationManager.AppSettings["uploadDestination"] + "/";
            }
            
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];

                // todo, see if this is really the right approach
                var fullPath =
                    Path.Combine(StorageRoot, Path.GetFileName(file.FileName));
                // hack using the file name to access the title field via the file name
                var fileTitle = context.Request.Form[file.FileName.Replace(" ", "").Replace(".","")];

                // the file title is a description


                try
                {
                    
                    GalleryImageProcessor ip = new GalleryImageProcessor();
                    OriginalImage img = new OriginalImage();
                    img.Filename = fullPath;
                    img.Title = fileTitle;
                    // todo, see if there are better ways of generating this url
                    img.UrlPath = GalleryDirectory + "/" + Path.GetFileName(file.FileName);
                    // todo, put this saving into the image as I had a bug where no
                    // files were saved because I omitted this step.
                    file.SaveAs(img.Filename);                   
                    ip.ProcessPostUpload(img);
                                

                    string fullName = Path.GetFileName(file.FileName);
                    statuses.Add(new FilesStatus(fullName, file.ContentLength, fullPath));
                }
                catch (Exception e)
                {
                    // todo, find out how to report the error to the web page
                    context.Response.Write("<p>Exception " + e.Message + "</p>" );

                }
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var filename = context.Request["f"];
            var filePath = StorageRoot + filename;

            if (File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private void ListCurrentFiles(HttpContext context)
        {
            var files =
                new DirectoryInfo(StorageRoot)
                    .GetFiles("*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(f => new FilesStatus(f))
                    .ToArray();

            string jsonObj = js.Serialize(files);
            context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            context.Response.Write(jsonObj);
            context.Response.ContentType = "application/json";
        }

    }
}