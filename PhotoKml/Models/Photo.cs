using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoKml.Models
{
    internal class Photo
    {
        public int KmlId { get; set; }
        public string? Name { get; set; }  // Title
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TimeTaken { get; set; } // Formatted
        public DateTime? CreationTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Altitude { get; set; }
        public string? Folder { get; set; } = string.Empty;
    }
}
