using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject.Extensions.Logging;
using Rbit.CommandLineTool.Interfaces;
using Rbit.CommandLineTool.RomCommands.Support;

namespace Rbit.CommandLineTool.RomCommands
{
    public class CleanGameListCommand : CommandBase
    {
        public CleanGameListCommand(ILogger logger) : base(logger) { }

        public override bool CanExecute()
        {
            return Arguments.Contains("g") && Arguments.Contains("r");
        }

        public override void Execute()
        {
            Logger.Info($"Cleaning gamelist in: {Arguments["g"]}");

            if (!File.Exists(Arguments["g"])) { throw new Exception($"{Arguments["g"]} could not be found, please verify that the file exists."); }
            if (!Directory.Exists(Arguments["r"])) { throw new Exception($"{Arguments["r"]} could not be found, please verify that the folder exists."); }

            var manager = new GameListManager(Logger);

            var sourceGameList = XDocument.Load(Arguments["g"]);
            Logger.Info($"Loaded source gameslist from: {Arguments["g"]}");

            var newGameList = manager.Clean(sourceGameList, Arguments["r"]);

            newGameList.Save(Arguments["g"].Replace("gamelist.xml", "gamelist.clean.xml"));

            Logger.Info($"We cleaned up {sourceGameList.Descendants("game").Count() - newGameList.Descendants("game").Count()} saved in gamelist {Arguments["g"].Replace("gamelist.xml", "gamelist.clean.xml")}");
        }

        public override string Name => "CleanGameList";
        public override string Description => "Clean up the gamelist by deleting entries without having the rom file.";
        public override string Usage => "CleanGameList -g <input gameslist.xml> -r <folder location containing the roms>";
    }
}