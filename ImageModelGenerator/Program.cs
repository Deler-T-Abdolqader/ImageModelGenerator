using ImageModelGenerator.Generator;
using ImageModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            IInitial modelGenerator = new ModelGenerator();
            modelGenerator.Initialization(new Options
            {
                SourceDataPath = @"d:\ML\source",
                DestinationPath = @"d:\ML\destination",
                FontPath = @"d:\ML\fonts",
                FontSize = 24,
                BrushColor = Brushes.Black,
                MaxThreadLimit = 20
            }).BuildModels();
            Console.ReadLine();
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
