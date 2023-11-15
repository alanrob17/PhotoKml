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
            string imagePath = @"E:\Images";
            var baseDirectory = Environment.CurrentDirectory; 

            List<Photo> photoList = new();
            List<string> imageList = new();

            ImageData.GetImageList(imageList, imagePath);

            ExifData.GetPhotoMetaData(photoList, imageList, imagePath);

            foreach (Photo photo in photoList) 
            {
                Console.WriteLine($"{photo.Name}, {photo.TimeTaken}, {photo.CreationTime}, {photo.Description}, {photo.Latitude}, {photo.Longitude}, {photo.Altitude}");
            }

            return;

            StringBuilder sb = new();

            sb = KmlData.CreateHeader(sb);

            sb = KmlData.BuildContent(sb, photoList);

            sb = KmlData.CreateFooter(sb);

            KmlData.CreateKmlFile(sb, baseDirectory);

            Console.WriteLine("Finito...");
        }
    }
}