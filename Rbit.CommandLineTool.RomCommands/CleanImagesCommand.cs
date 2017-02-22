using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject.Extensions.Logging;
using Rbit.CommandLineTool.Interfaces;
using Rbit.CommandLineTool.RomCommands.Support;

namespace Rbit.CommandLineTool.RomCommands
{
    public class CleanImagesCommand : CommandBase
    {
        public CleanImagesCommand(ILogger logger) : base(logger) { }

        public override bool CanExecute()
        {
            return Arguments.Contains("g") && Arguments.Contains("i");
        }

        public override void Execute()
        {
            Logger.Info($"Cleaning images in: {Arguments["i"]}");

            if (!File.Exists(Arguments["g"])) { throw new Exception($"{Arguments["g"]} could not be found, please verify that the file exists."); }
            if (!Directory.Exists(Arguments["i"])) { throw new Exception($"{Arguments["i"]} could not be found, please verify that the folder exists."); }

            var manager = new GameListManager(Logger);

            var sourceGameList = XDocument.Load(Arguments["g"]);
            Logger.Info($"Loaded source gameslist from: {Arguments["g"]}");

            var imageList = manager.CleanImages(sourceGameList, Arguments["i"]);

            Logger.Info($"We removed up {imageList.Count} images");
        }

        public override string Name => "CleanImages";
        public override string Description => "Clean up the background_images folder by deleting images that are not referenced in the game list.";
        public override string Usage => "CleanImages -g <input gameslist.xml> -r <folder location containing the images>";
    }
}