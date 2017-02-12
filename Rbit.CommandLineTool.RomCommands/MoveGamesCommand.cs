using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject.Extensions.Logging;
using Rbit.CommandLineTool.Interfaces;
using Rbit.CommandLineTool.RomCommands.Support;

namespace Rbit.CommandLineTool.RomCommands
{
    public class MoveGamesCommand : CommandBase
    {
        public MoveGamesCommand(ILogger logger) : base(logger) { }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            Logger.Info($"Moving roms and info found in {Arguments["i"]}");

            if (!File.Exists(Arguments["c"])) { throw new Exception($"{Arguments["c"]} could not be found, please verify that the file exists."); }
            if (!File.Exists(Arguments["f"])) { throw new Exception($"{Arguments["f"]} could not be found, please verify that the file exists."); }

            var currentBaseFolder = Arguments["f"].Substring(0, Arguments["f"].LastIndexOf("\\"));
            Logger.Info($"Current base folder set to {currentBaseFolder}");

            var currentEmulator = currentBaseFolder.Substring(currentBaseFolder.LastIndexOf("\\") + 1);
            Logger.Info($"Current emulator name set to: {currentEmulator}");

            var manager = new GameListManager(Logger);

            var games = manager.LoadGameIds(Arguments["c"]);
            Logger.Info($"Found {games.Count} unique games in input file.");

            var sourceGameList = XDocument.Load(Arguments["f"]);
            Logger.Info($"Loaded source gameslist from: {Arguments["f"]}");

            var newList = manager.MoveGames(currentBaseFolder, currentEmulator, sourceGameList, games, Arguments["l"], Arguments["e"], Arguments.Contains("remove"));

            // save gamelist
            newList.Save($"{Arguments["l"]}\\gamelists\\{Arguments["e"]}\\gamelist.xml");

            if (Arguments.Contains("remove"))
            {
                sourceGameList.Save(Arguments["f"]);
            }
        }


        public override string Name => "MoveGames";

        public override string Description => "Moves selected roms and its related images and gamelist item to a new location.";
        public override string Usage => "MoveGames -f <input gameslist.xml> -c <input game id list cvs file> -l <target root location for new gamelist and images> -e <target emulator name, like: neogeo> [-remove <optional parameter when you want the moved stuff removed from the source>] ";
    }
}
