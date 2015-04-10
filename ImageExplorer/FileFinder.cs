using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageExplorer
{
    class FileFinder
    {
        // starting from a root directory, get all files of specified types (extensions)
        // I want tags that the user can use to organize and group and find the files and
        Dictionary<string, FileInfo> FoundFiles = new Dictionary<string, FileInfo>() ;

        public Dictionary<string, FileInfo> FoundFilesDictionary
        {
            get { return FoundFiles; }
            set { FoundFiles = value; }
        }
        // do not use a dot preceding the extension
        public void FindFiles(string rootDir, string extension)
        {
            IEnumerable<string> foundFileList;
            // use try
            try
            {
                foundFileList = Directory.EnumerateFiles(rootDir).Where<string>(f => f.EndsWith("." + extension));
                foreach (var f in foundFileList)
                {
                    var fileInfo = new FileInfo(f);
                    FoundFiles.Add(f, fileInfo);
                }
            }
            catch
            {
                // access exception, just continue
            }
            try
            {
                var foundDirList = Directory.EnumerateDirectories(rootDir);
                foreach (var d in foundDirList)
                {
                    FindFiles(d, extension);
                }
            }
            catch
            {
                // access exception, just continue
            }

        }


    }
}
