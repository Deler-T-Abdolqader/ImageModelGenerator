using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator.Models
{
    public class Options
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
        public int FontSize { get; set; } = 24;
        public Brush BrushColor { get; set; } = Brushes.Black;
        public int MaxThreadLimit { get; set; } = 20;
        /// <summary>
        /// {0}=Label name, 
        /// {1}=Font name, 
        /// {2}=Index
        /// Default Value: @"{0}\{1}" => {Label}\{Font}.png
        /// </summary>
        public string SavePattern { get; set; } = @"{0}\{1}";
        /// <summary>
        /// If set true, Tab-separated values file will be generated as "tags.tsv" in destination root. Format:FilePath Label
        /// </summary>
        public bool GenerateTsvFile { get; set; } = false;
    }
}
