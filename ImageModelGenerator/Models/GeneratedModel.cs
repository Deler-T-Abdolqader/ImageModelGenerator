using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator.Models
{
    public class GeneratedModel
    {
        public string Label { get; set; }
        public string Font { get; set; }
        public Bitmap Model { get; set; }
    }
}
