using System;
using System.Text;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoKml.Data;
using PhotoKml.Models;
using XmpCore.Impl;

namespace PhotoKml
{
    internal class PhotoData
    {
        static void Main(string[] args)
        {
            // Replace this with the path to your image file
            string imagePath = @"Image path here.";
            var baseDirectory = Environment.CurrentDirectory; 

            List<Photo> photoList = new();
            List<string> imageList = new();

            ImageData.GetImageList(imageList, imagePath);

            ExifData.GetPhotoMetaData(photoList, imageList, imagePath);

            StringBuilder sb = new();

            sb = KmlData.CreateHeader(sb);

            sb = KmlData.BuildContent(sb, photoList);

            sb = KmlData.CreateFooter(sb);

            KmlData.CreateKmlFile(sb, baseDirectory);
        }
    }
}