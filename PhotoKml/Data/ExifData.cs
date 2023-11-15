using MetadataExtractor.Formats.Exif;
using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoKml.Models;
using System.Net;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace PhotoKml.Data
{
    internal class ExifData
    {
        internal static void GetPhotoMetaData(List<Photo> photoList, List<string> imageList, string imagePath)
        {
            foreach (var image in imageList)
            {
                Photo photo = CreatePhoto(image, imagePath);

                var directories = ImageMetadataReader.ReadMetadata(image);

                foreach (var directory in directories)
                {                   
                    if (directory is GpsDirectory gpsDirectory)
                    {
                        ProcessGeoLocation(photo, gpsDirectory);
                    }

                    if (directory is ExifSubIfdDirectory exifSubIfDirectory)
                    {
                        ProcessExifData(photo, exifSubIfDirectory);
                    }
                }

                // if (photo.Latitude != 0.0 && photo.Longitude != 0.0)
                // {
                    AddDescription(photo);
                    photoList.Add(photo);
                // }
                // else
                // {
                    Console.WriteLine($"{photo.Name}");
                // }
            }
        }

        private static void AddDescription(Photo photo)
        {
            string description = $"<![CDATA[Date: {photo.CreationTime}<br />Folder: {photo.Folder}<br />Altitude: {photo.Altitude} metres]]>";
            photo.Description = description;
        }

        private static void ProcessExifData(Photo photo, ExifSubIfdDirectory exifSubIfDirectory)
        {
            var timeTaken = exifSubIfDirectory.GetDescription(ExifSubIfdDirectory.TagDateTimeOriginal);
            DateTime creationTime = new();
            creationTime = DateTime.ParseExact(timeTaken, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(timeTaken))
            {
                photo.TimeTaken = timeTaken;
                photo.CreationTime = creationTime;
            }
        }

        private static void ProcessGeoLocation(Photo photo, GpsDirectory gpsDirectory)
        {
            string latitude = gpsDirectory.GetDescription(GpsDirectory.TagLatitude);
            string latitudeRef = gpsDirectory.GetDescription(GpsDirectory.TagLatitudeRef);
            string longitude = gpsDirectory.GetDescription(GpsDirectory.TagLongitude);
            string longitudeRef = gpsDirectory.GetDescription(GpsDirectory.TagLongitudeRef);
            string altitude = gpsDirectory.GetDescription(GpsDirectory.TagAltitude);

            string latitudeDMS = latitude + latitudeRef;
            string longitudeDMS = longitude + longitudeRef;

            double latitudeDD = ConvertDMSToDD(latitudeDMS);
            double longitudeDD = ConvertDMSToDD(longitudeDMS);

            photo.Latitude = latitudeDD;
            photo.Longitude = longitudeDD;

            if (altitude != null)
            {
                string[] parts = altitude.Split(' ');

                if (Int32.TryParse(parts[0], out int numValue))
                {
                    photo.Altitude = numValue;
                }
            }
        }

        private static Photo CreatePhoto(string image, string imagePath)
        {
            Photo photo = new()
            {
                Name = Path.GetFileNameWithoutExtension(image),
                Folder = image.Replace(imagePath + "\\", string.Empty),
                Latitude = 0.0,
                Longitude = 0.0,
                Altitude = 0
            };

            return photo;
        }

        private static double ConvertDMSToDD(string dms)
        {
            dms = dms.Replace(" ", string.Empty);
            string[] parts = dms.Split('°', '\'', '\"', 'N', 'S', 'E', 'W', 'n', 's', 'e', 'w');

            if (parts.Length < 2)
            {
                throw new ArgumentException("Invalid DMS format.");
            }

            int degrees = Int32.Parse(parts[0]);
            degrees = Math.Abs(degrees);
            int minutes = Int32.Parse(parts[1]);
            double seconds = double.Parse(parts[2]);

            double decimalDegrees = (double)degrees + (minutes / 60.0) + (seconds / 3600.0);

            // Check for north or east (positive) vs. south or west (negative)
            if (dms.Contains("S") || dms.Contains("W") || dms.Contains("s") || dms.Contains("w"))
            {
                decimalDegrees = -decimalDegrees;
            }

            return decimalDegrees;
        }
    }
}
