using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoKml.Data
{
    public class ImageData
    {
        internal static List<string> GetImageList(List<string> imageList, string folder)
        {
            var dir = new DirectoryInfo(folder);
            GetImageFiles(dir, imageList);

            return imageList;
        }

        private static void GetImageFiles(DirectoryInfo d, List<string> imageList)
        {
            var files = d.GetFiles("*.*");

            foreach (FileInfo file in files)
            {
                var fileName = file.FullName;

                var newDirectory = Path.GetDirectoryName(fileName);
                var dirName = d.FullName;

                if (Path.GetExtension(fileName.ToLowerInvariant()) == ".jpg")
                {
                    imageList.Add(fileName);
                }
            }

            // get sub-folders for the current directory
            var dirs = d.GetDirectories("*.*");

            // recurse
            foreach (DirectoryInfo dir in dirs)
            {
                // Console.WriteLine("--------->> {0} ", dir.Name);
                GetImageFiles(dir, imageList);
            }
        }
    }
}
