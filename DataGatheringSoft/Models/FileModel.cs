using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.IO;

namespace DataGatheringSoft.Models
{
    public class FileModel
    {
        public FileModel(string fullname, string relativePath)
        {
            var info = new FileInfo(fullname);
            Name = Path.GetFileNameWithoutExtension(info.FullName);
            Directory = relativePath;
            CreationDate = info.CreationTime;
            ModificationDate = info.LastWriteTime;
            AccessDate = info.LastAccessTime;
            Size = info.Length;
            Extension = info.Extension;
            FileSecurity = File.GetAccessControl(fullname);
            FileAttributes = info.Attributes;
        }
        public string Name { get; set; }
        public string Directory { get; set; }
        public long Size { get; set; }
        public string SizeStr {
            get
            {
                if (Size > Math.Pow(2, 30))
                    return new StringBuilder().Append(Math.Round(Size / Math.Pow(2, 30),2)).Append(' ').Append("GB").ToString();
                if (Size > Math.Pow(2, 20))
                    return new StringBuilder().Append(Math.Round(Size / Math.Pow(2, 20),2)).Append(' ').Append("MB").ToString();
                if (Size > Math.Pow(2, 10))
                    return new StringBuilder().Append(Math.Round(Size / Math.Pow(2, 10),2)).Append(' ').Append("KB").ToString();
                return new StringBuilder().Append(Size).Append(' ').Append("B").ToString();
            }
        }
        public DateTime CreationDate { get; set; }
        public string CreationDateStr {
            get
            {
                return CreationDate.ToString("yyyy-MM-dd hh:mm");
            }
        }

        public DateTime ModificationDate { get; set; }
        public string ModificationDateStr {
            get
            {
                return ModificationDate.ToString("yyyy-MM-dd hh:mm");
            }
        }

        public DateTime AccessDate { get; set; }
        public string AccessDateStr
        {
            get
            {
                return AccessDate.ToString("yyyy-MM-dd hh:mm");
            }
        }

        public string Extension { get; set; }

        public FileSecurity FileSecurity { get; set; }
        public string Author { get { return FileSecurity.GetOwner(typeof(System.Security.Principal.NTAccount)).ToString(); }
        }

        public FileAttributes FileAttributes { get; set; }
        public bool IsReadonly { set { } get { return (FileAttributes & FileAttributes.ReadOnly) != 0; } }
        public bool IsHidden { set { } get { return (FileAttributes & FileAttributes.Hidden) != 0; } }
        public bool IsCompressed { set { } get { return (FileAttributes & FileAttributes.Compressed) != 0; } }
    }
}
