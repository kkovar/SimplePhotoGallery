using System;
using System.IO;

namespace jQuery_File_Upload.MVC4.Upload
{
    using System.Drawing;

    public class FilesStatus
    {
        public const string HandlerPath = "/Upload/";

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }

        public FilesStatus() { }

        public FilesStatus(FileInfo fileInfo) { SetValues(fileInfo.Name, (int)fileInfo.Length, fileInfo.FullName); }

        public FilesStatus(string fileName, int fileLength, string fullPath) { SetValues(fileName, fileLength, fullPath); }

        private void SetValues(string fileName, int fileLength, string fullPath)
        {
            name = fileName;
            type = "image/png";
            size = fileLength;
            progress = "1.0";
            url = HandlerPath + "UploadHandler.ashx?f=" + fileName;
            delete_url = HandlerPath + "UploadHandler.ashx?f=" + fileName;
            delete_type = "DELETE";

            var ext = Path.GetExtension(fullPath);

            if (!IsImage(ext))
            {
                thumbnail_url = "/Content/img/generalFile.png";
            }
            else
            {
                Image thumbnail;

                using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(fullPath)))
                {
                    thumbnail = new Bitmap(stream).GetThumbnailImage(80, 60, new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                }

                using (var stream = new MemoryStream())
                {
                    thumbnail.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                    thumbnail_url = thumbnail_url = @"data:image/png;base64," + EncodeFile(stream.ToArray());
                }
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        private bool IsImage(string ext)
        {
            return ext == ".gif" || ext == ".jpg" || ext == ".png";
        }

        private string EncodeFile(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}