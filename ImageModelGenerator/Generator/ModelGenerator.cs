using ImageModelGenerator.Helper;
using ImageModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator.Generator
{
    public class ModelGenerator : IInitial, IBuild
    {
        private List<string> _fonts { get; set; }
        private List<string> _labels { get; set; }

        private List<GeneratedModel> _models { get; set; }
        private Options _options;

        IBuild IInitial.Initialization(Options options)
        {
            _options = options;
            readFonts();
            readLabels();
            return this;
        }
        void IBuild.BuildModels()
        {
            generateModels();
            saveModels();
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
                console($"Found fonts: {_fonts.Count},  Execution time: {stopwatch.ElapsedTime()}", "");
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
                console($"Found labels: {_labels.Count},  Execution time: {stopwatch.ElapsedTime()}", "");
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

        #region Build
        private void generateModels()
        {
            try
            {
                console("Generating models...");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var generatedModels = 0;
                var allModels = _fonts.Count * _labels.Count;
                console($"Generated models: {generatedModels} from {allModels},  Execution time: {stopwatch.ElapsedTime()}");
                _models = new List<GeneratedModel>();
                Parallel.ForEach(_labels, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 1 }, (currentLabel) =>
                 {
                     foreach (var item in _fonts.Select((font, index) => (font, index)))
                     {
                         SizeF size;
                         Image fakeImage = new Bitmap(1, 1);
                         using (Graphics g = Graphics.FromImage(fakeImage))
                         {
                             size = g.MeasureString(currentLabel, new Font(item.font, _options.FontSize));
                             g.Flush();
                         }
                         Bitmap bmp = new Bitmap(Convert.ToInt32(Math.Ceiling(size.Width)), Convert.ToInt32(Math.Ceiling(size.Height)));

                         using (Graphics g = Graphics.FromImage(bmp))
                         {
                             g.SmoothingMode = SmoothingMode.HighQuality;
                             g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                             g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                             g.DrawString(currentLabel, new Font(item.font, _options.FontSize), _options.BrushColor, new PointF(0, 0));

                             g.Flush();
                         }

                         _models.Add(new GeneratedModel { Label = currentLabel, Font = item.font, Model = bmp, Index = item.index });
                         clearLastLine();
                         console($"Generated models: {++generatedModels} from {allModels},  Execution time: {stopwatch.ElapsedTime()}");
                     }
                 });
                stopwatch.Stop();
                clearLastLine();
                console($"Generated models: {generatedModels},  Execution time: {stopwatch.ElapsedTime()}", "");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while generating the models list");
            }
        }
        private void saveModels()
        {
            try
            {
                console("Saving models...");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var savedModels = 0;
                var allModels = _models.Count;
                var tsvList = new List<string>();
                Parallel.ForEach(_models, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = _options.MaxThreadLimit }, (currentModel) =>
                {
                    var model = (GeneratedModel)currentModel;
                    var pathPattern = string.Format($"{_options.SavePattern}.png", model.Label, model.Font, model.Index);
                    var path = generateModelPath(_options.DestinationPath, pathPattern);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    model.Model.Save(path, ImageFormat.Png);
                    if (_options.GenerateTsvFile)
                    {
                        tsvList.Add($"{pathPattern}    {model.Label}");
                    }
                    savedModels++;
                });
                if (_options.GenerateTsvFile)
                {
                    var sw = new StreamWriter(generateModelPath(_options.DestinationPath, "tags.tsv"), false);
                    Parallel.ForEach(tsvList, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = _options.MaxThreadLimit }, (currentLine) =>
                    {
                        lock (sw)
                        {
                            sw.WriteLine(currentLine);
                        }
                    });
                    sw.Close();
                    sw.Dispose();
                }
                console($"Saved models: {allModels},  Execution time: {stopwatch.ElapsedTime()}", "");
                Process.Start(_options.DestinationPath);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the models list");
            }
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

        private void clearLastLine()
        {
            Console.CursorTop = Console.CursorTop - 1;
            Console.Write(new string(' ', Console.BufferWidth));
            Console.CursorTop = Console.CursorTop - 1;
        }

        private string generateModelPath(params string[] paths)
        {
            return Path.Combine(paths);
        }
        #endregion
    }
}
