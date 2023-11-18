using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PhotoKml.Models;

namespace PhotoKml.Data
{
    internal class KmlData
    {
        internal static void AddPhotos(List<Photo> photoList)
        {
            foreach (Photo photo in photoList)
            {
                photo.Name = photo.Name + ".jpg";
                var id = AddPhoto(photo);
            }
        }

        internal static int AddPhoto(Photo photo)
        {
            var kmlId = -1; // 0 is used for record is already in the db.

            using (SqlConnection cn = new SqlConnection(LoadConnectionString()))
            {
                var cmd = new SqlCommand("adm_AddPhoto", cn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("Name", photo.Name);
                cmd.Parameters.AddWithValue("Title", photo.Title);
                cmd.Parameters.AddWithValue("Description", photo.Description);
                cmd.Parameters.AddWithValue("TimeTaken", photo.TimeTaken);
                cmd.Parameters.AddWithValue("CreationTime", photo.CreationTime);
                cmd.Parameters.AddWithValue("Latitude", photo.Latitude);
                cmd.Parameters.AddWithValue("Longitude", photo.Longitude);
                cmd.Parameters.AddWithValue("Folder", photo.Folder);
                cmd.Parameters.AddWithValue("Altitude", photo.Altitude);
                cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                using (cn)
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    kmlId = (int)cmd.Parameters["@ReturnValue"].Value;
                }
            }

            return kmlId;
        }

        internal static List<Photo> CreatePhotoList()
        {
            var dt = new DataTable();

            using (SqlConnection cn = new SqlConnection(LoadConnectionString()))
            {
                var cmd = new SqlCommand("adm_GetAllPhotos", cn) { CommandType = CommandType.StoredProcedure };

                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                var query = from dr in dt.AsEnumerable()
                            select new Photo
                            {
                                KmlId = Convert.ToInt32(dr["KmlId"]),
                                Name = dr["Name"].ToString(),
                                Title = dr["Title"].ToString(),
                                Description = dr["Description"].ToString(),
                                TimeTaken = dr["TimeTaken"].ToString(),
                                CreationTime = DataConvert.ConvertTo<DateTime>(dr["CreationTime"], default(DateTime)),
                                Latitude = DataConvert.ConvertTo<double>(dr["Latitude"], default(float)),
                                Longitude = DataConvert.ConvertTo<double>(dr["Longitude"], default(float)),
                                Altitude = DataConvert.ConvertTo<int>(dr["Altitude"], default(int)),
                                Folder = dr["Folder"].ToString()
                            };

                return query.ToList();
            }
        }

        internal static StringBuilder BuildContent(StringBuilder sb, List<Photo> photos)
        {
            foreach (var photo in photos)
            {
                if (photo.Latitude == 0 && photo.Longitude == 0)
                {
                    Console.WriteLine($"{photo.Name} - {photo.Title}");
                }
                else
                {
                    sb.Append("\t<Placemark>\n");
                    var title = photo.Title;
                    sb.Append($"\t\t<name>{title}</name>\n");

                    var description = string.Empty;

                    description = photo.Description;
                    sb.Append($"\t\t<description>{description}</description>\n");
                    sb.Append($"\t\t<TimeStamp><when>{photo.CreationTime.Value}</when></TimeStamp>\n");
                    sb.Append("\t\t<styleUrl>#placemark-red</styleUrl>\n");
                    sb.Append($"\t\t<Point><coordinates>{photo.Longitude},{photo.Latitude}</coordinates></Point>\n"); // -0.12692465,51.51472
                    sb.Append("\t</Placemark>\n");
                }
            }

            return sb;
        }

        internal static StringBuilder CreateHeader(StringBuilder sb)
        {
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            sb.Append("<kml xmlns=\"http://earth.google.com/kml/2.2\">\n");
            sb.Append("<Document>\n");

            sb.Append("\t<Style id=\"placemark-red\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>https://maps.gstatic.com/mapfiles/ridefinder-images/mm_20_red.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-blue\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-blue.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-purple\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-purple.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-yellow\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-yellow.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-pink\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-pink.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-brown\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-brown.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-green\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-green.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-orange\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-orange.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-deeppurple\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-deeppurple.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-lightblue\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-lightblue.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-cyan\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-cyan.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-teal\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-teal.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-lime\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-lime.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-deeporange\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-deeporange.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-gray\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-gray.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<Style id=\"placemark-bluegray\">\n");
            sb.Append("\t\t<IconStyle>\n");
            sb.Append("\t\t\t<Icon>\n");
            sb.Append("\t\t\t\t<href>http://maps.me/placemarks/placemark-bluegray.png</href>\n");
            sb.Append("\t\t\t</Icon>\n");
            sb.Append("\t\t</IconStyle>\n");
            sb.Append("\t</Style>\n");
            sb.Append("\t<name>Europe 2023 - Photo Location Points</name>\n");
            sb.Append("\t<description>Our 2023 trip.</description>\n");
            sb.Append("\t<visibility>1</visibility>\n");

            return sb;
        }

        internal static StringBuilder CreateFooter(StringBuilder sb)
        {
            sb.Append("\t</Document>\n");
            sb.Append("</kml>\n");

            return sb;
        }

        internal static void CreateKmlFile(StringBuilder sb, string baseDirectory)
        {
            var fileName = "Europe2023Photos.kml";
            var fullPath = baseDirectory + "\\" + fileName;
            File.WriteAllText(fullPath, sb.ToString());
        }

        private static string LoadConnectionString(string id = "Photo")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
