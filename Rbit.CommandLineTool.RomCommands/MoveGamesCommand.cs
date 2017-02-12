using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject.Extensions.Logging;
using Rbit.CommandLineTool.Support;

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
            _logger.Info($"Moving roms and info found in {Arguments["i"]}");

            if (!File.Exists(Arguments["c"])) { throw new Exception($"{Arguments["c"]} could not be found, please verify that the file exists."); }
            if (!File.Exists(Arguments["f"])) { throw new Exception($"{Arguments["f"]} could not be found, please verify that the file exists."); }

            var currentBaseDir = Arguments["f"].Substring(0, Arguments["f"].LastIndexOf("\\"));
            var currentEmulator = currentBaseDir.Substring(currentBaseDir.LastIndexOf("\\") + 1);

            var games = LoadGameIds(Arguments["c"]);

            _logger.Info($"Found {games.Count} unique games in input file.");

            var sourceGameList = XDocument.Load(Arguments["f"]);

            var gamelist = new XDocument(new XElement("gameList"));

            foreach (var game in games)
            {
                // Get the source game node
                var gamesXml = sourceGameList.Element("gameList").Elements("game").Where(g => g.Attribute("id").Value == game);

                _logger.Info("------------------------------------------------------------------------------------------------");
                _logger.Info($"Processing game: {gamesXml.First().Element("name").Value}");

                if (gamesXml.Count() > 1)
                {
                    _logger.Info($"Found {gamesXml.Count()} variations");
                }

                foreach (var gameXml in gamesXml)
                {
                    var image = GetValueSubString(gameXml.Element("image")?.Value, gameXml.Element("image").Value.LastIndexOf("/") + 1);
                    var thumbnail = GetValueSubString(gameXml.Element("thumbnail")?.Value, gameXml.Element("thumbnail").Value.LastIndexOf("/") + 1);
                    var rom = GetValueSubString(gameXml.Element("path")?.Value, gameXml.Element("path").Value.LastIndexOf("/") + 1);

                    // replace the path
                    _logger.Info($"Current rom path: {gameXml.Element("path").Value}");
                    gameXml.Element("path").Value = gameXml.Element("path").Value.Replace(currentEmulator, Arguments["e"]);
                    _logger.Info($"Rom path changed to: {gameXml.Element("path").Value}");

                    // replace the image
                    _logger.Info($"Current image path: {gameXml.Element("image").Value}");
                    gameXml.Element("image").Value = gameXml.Element("image").Value.Replace(currentEmulator, Arguments["e"]);
                    _logger.Info($"Image path changed to: {gameXml.Element("image").Value}");

                    // replace the thumbnail
                    _logger.Info($"Current thumbnail path: {gameXml.Element("thumbnail").Value}");
                    gameXml.Element("thumbnail").Value = gameXml.Element("thumbnail").Value.Replace(currentEmulator, Arguments["e"]);
                    _logger.Info($"Thumbnail path changed to: {gameXml.Element("thumbnail").Value}");

                    // add to new xml
                    gamelist.Root.Add(gameXml);

                    // move image
                    if (!string.IsNullOrEmpty(image))
                    {
                        CopyFile($"{currentBaseDir}\\..\\..\\downloaded_images\\{currentEmulator}\\{image}",
                            $"{Arguments["l"]}\\downloaded_images\\{Arguments["e"]}\\{image}");
                    }

                    // move thumbnail
                    if (!string.IsNullOrEmpty(thumbnail))
                    {
                        CopyFile($"{currentBaseDir}\\..\\..\\downloaded_images\\{currentEmulator}\\{thumbnail}",
                            $"{Arguments["l"]}\\downloaded_images\\{Arguments["e"]}\\{thumbnail}");
                    }

                    // move rom
                    if (!string.IsNullOrEmpty(rom))
                    {
                        CopyFile($"{currentBaseDir}\\..\\..\\roms\\{currentEmulator}\\{rom}",
                            $"{Arguments["l"]}\\roms\\{Arguments["e"]}\\{rom}");
                    }

                }
            }

            // save gamelist
            gamelist.Save($"{Arguments["l"]}\\gamelists\\{Arguments["e"]}\\gamelist.xml");
        }

        private string GetValueSubString(string value, int index)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Substring(index);
        }

        private void CopyFile(string source, string target)
        {
            _logger.Info($"Copying {new DirectoryInfo(source).FullName}, to {new DirectoryInfo(target).FullName}");

            File.Copy(source, target, true);

        }

        private List<string> LoadGameIds(string file)
        {
            return File.ReadAllLines(file).Select(game => game.Split(',')[0]).Distinct().ToList();
        }

        public override string Name => "MoveGames";

        public override string Description => "Moves selected roms and its related images and gamelist item to a new location.";
        public override string Usage => "MoveGames -i <input csv file that has at least the game id> -l <location folder to move to> -e <emulator name (like; arcade, fba, nes)>";
    }
}
