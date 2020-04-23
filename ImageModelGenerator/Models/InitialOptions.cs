using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator.Models
{
    public class InitialOptions
    {
        /// <summary>
        /// SourceDataPath is data file path or directory path of data files
        /// </summary>
        public string SourceDataPath { get; set; }
        /// <summary>
        /// DestinationPath is a folder path for put results (model) on it
        /// </summary>
        public string DestinationPath { get; set; }
        /// <summary>
        /// FontPath is font file path or directory of fonts 
        /// </summary>
        public string FontPath { get; set; }
        public int FontSize { get; set; } = 14;
        public Color FontColor { get; set; } = Color.Black;
        public int MaxThreadLimit { get; set; } = 20;
    }
}
