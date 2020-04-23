using ImageModelGenerator.Helper;
using ImageModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator.Generator
{
    public class ModelGenerator : IInitial, IBuild
    {
        private List<string> _labels { get; set; }
        private List<string> _fonts { get; set; }
        private InitialOptions _options;

        IBuild IInitial.InitialRequirements(InitialOptions options)
        {
            _options = options;
            readFonts();
            readLabels();
            return this;
        }
        void IBuild.BuildModels()
        {

        }

        #region Initials
        private void readFonts()
        {
            try
            {
                console("Reading fonts...");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                _fonts = new List<string>();
                FileAttributes attr = File.GetAttributes(_options.FontPath);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    var files = Directory.GetFiles(_options.FontPath, "*.txt", SearchOption.TopDirectoryOnly);
                    Parallel.ForEach(files, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = _options.MaxThreadLimit }, (currentFile) =>
                    {
                        _fonts.AddRange(readFileLines(currentFile));
                    });
                }
                else
                {
                    _fonts.AddRange(readFileLines(_options.FontPath));
                }
                stopwatch.Stop();
                console($"Found fonts: {_fonts.Count},  Read time: {stopwatch.ElapsedTime()}", "");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading the fonts list");
            }
        }
        private void readLabels()
        {
            try
            {
                console("Reading lables...");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                _labels = new List<string>();
                FileAttributes attr = File.GetAttributes(_options.SourceDataPath);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    var files = Directory.GetFiles(_options.SourceDataPath, "*.txt", SearchOption.TopDirectoryOnly);
                    Parallel.ForEach(files, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = _options.MaxThreadLimit }, (currentFile) =>
                    {
                        _labels.AddRange(readFileLines(currentFile));
                    });
                }
                else
                {
                    _labels.AddRange(readFileLines(_options.SourceDataPath));
                }
                stopwatch.Stop();
                console($"Found labels: {_labels.Count},  Read time: {stopwatch.ElapsedTime()}", "");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading the labels list");
            }
        }
        private IEnumerable<string> readFileLines(string filePath)
        {
            return File.ReadLines(filePath);
        }
        #endregion

        #region Generals
        private void console(params string[] messages)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
        }
        #endregion
    }
}
