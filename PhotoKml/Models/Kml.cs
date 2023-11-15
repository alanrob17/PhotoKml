using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoKml.Models
{
    internal class Kml
    {
        public int KmlId { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TimeStamp { get; set; }
        public DateTime CreationTime { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string? Folder { get; set; }
        public int Altitude { get; set; }
    }
}


