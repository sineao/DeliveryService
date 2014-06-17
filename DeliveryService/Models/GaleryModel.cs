using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryService.Models
{
    public class GaleryModel
    {
        public GaleryModel()
        {
            GaleryFolders = new List<GaleryFolder>();
        }
        public List<string> Folders;
        public List<GaleryFolder> GaleryFolders;
    }

    public class GaleryFolder
    {
        public GaleryFolder()
        {
            Pictures = new List<Picture>();
        }
        public string Name;
        public List<Picture> Pictures;
    }

    public class Picture
    {
        public string File;
        public string Thumbnail;
        public string Description;
        public string Name;
    }
}