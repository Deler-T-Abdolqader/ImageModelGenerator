using ImageModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator.Generator
{
    public interface IInitial
    {
        /// <summary>
        /// InitialRequirements read input labels (Source) and generate models (Destination) base on types (Font) 
        /// </summary>
        /// <param name="InitialOptions">
        /// options variable contains generator prerequisites
        /// </param>
        /// <returns>Return a Build interface for start building models</returns>
        IBuild Initialization(Options options);
    }
}
