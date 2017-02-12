using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject.Extensions.Logging;

namespace Rbit.CommandLineTool.RomCommands.Support
{
    public class GameListManager
    {
        private readonly ILogger _logger;

        public GameListManager(ILogger logger)
        {
            _logger = logger;
        }
        public static string CreateOutputInfoLine(XElement item, List<string> columns)
        {
            if (columns.Count == 0)
            {
                return item.Attribute("id").Value;
            }

            return item.Attribute("id").Value + "," + columns.Aggregate((current, next) => $"{item.Element(current).Value},{item.Element(next).Value}");
        }

        public XDocument MoveGames(string currentBaseFolder, string currentEmulator, XDocument currentGameList, IEnumerable<string> inputGamesList, string targetLocation, string targetEmulator, bool removeFromSource = false)
        {
            var newGameList = new XDocument(new XElement("gameList"));

            foreach (var game in inputGamesList)
            {
                // Get the source game node
                var gamesXml = currentGameList.Element("gameList").Elements("game").Where(g => g.Attribute("id").Value == game);

                _logger.Info("------------------------------------------------------------------------------------------------");
                _logger.Info($"Processing game: {gamesXml.First().Element("name").Value}");

                if (gamesXml.Count() > 1)
                {
                    _logger.Info($"Found {gamesXml.Count()} variations");
                }

                foreach (var gameXml in gamesXml)
                {
                    newGameList.Root.Add(this.MoveGame(currentBaseFolder, currentEmulator, gameXml, targetLocation, targetEmulator, removeFromSource));
                    if (removeFromSource)
                    {
                        gameXml.Remove();
                    }
                }
            }

            return newGameList;
        }


        private XElement MoveGame(string currentBaseDir, string currentEmulator, XElement gameXml, string targetLocation, string targetEmulator, bool move = false)
        {
            var image = GetValueSubString(gameXml.Element("image")?.Value, gameXml.Element("image").Value.LastIndexOf("/") + 1);
            var thumbnail = GetValueSubString(gameXml.Element("thumbnail")?.Value, gameXml.Element("thumbnail").Value.LastIndexOf("/") + 1);
            var rom = GetValueSubString(gameXml.Element("path")?.Value, gameXml.Element("path").Value.LastIndexOf("/") + 1);

            // replace the path
            _logger.Info($"Current rom path: {gameXml.Element("path").Value}");
            gameXml.Element("path").Value = gameXml.Element("path").Value.Replace(currentEmulator, targetEmulator);
            _logger.Info($"Rom path changed to: {gameXml.Element("path").Value}");

            // replace the image
            _logger.Info($"Current image path: {gameXml.Element("image").Value}");
            gameXml.Element("image").Value = gameXml.Element("image").Value.Replace(currentEmulator, targetEmulator);
            _logger.Info($"Image path changed to: {gameXml.Element("image").Value}");

            // replace the thumbnail
            _logger.Info($"Current thumbnail path: {gameXml.Element("thumbnail").Value}");
            gameXml.Element("thumbnail").Value = gameXml.Element("thumbnail").Value.Replace(currentEmulator, targetEmulator);
            _logger.Info($"Thumbnail path changed to: {gameXml.Element("thumbnail").Value}");

            // move image
            if (!string.IsNullOrEmpty(image))
            {
                CopyFile($"{currentBaseDir}\\..\\..\\downloaded_images\\{currentEmulator}\\{image}",
                    $"{targetLocation}\\downloaded_images\\{targetEmulator}\\{image}", move);
            }

            // move thumbnail
            if (!string.IsNullOrEmpty(thumbnail))
            {
                CopyFile($"{currentBaseDir}\\..\\..\\downloaded_images\\{currentEmulator}\\{thumbnail}",
                    $"{targetLocation}\\downloaded_images\\{targetEmulator}\\{thumbnail}", move);
            }

            // move rom
            if (!string.IsNullOrEmpty(rom))
            {
                CopyFile($"{currentBaseDir}\\..\\..\\roms\\{currentEmulator}\\{rom}",
                    $"{targetLocation}\\roms\\{targetEmulator}\\{rom}", move);
            }

            return gameXml;
        }

        public List<string> LoadGameIds(string file)
        {
            _logger.Info($"Loading game id's from {file}, making unique list without duplicate id's.");

            return File.ReadAllLines(file).Select(game => game.Split(',')[0]).Distinct().ToList();
        }

        private static string GetValueSubString(string value, int index)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Substring(index);
        }

        private void CopyFile(string source, string target, bool move = false)
        {
            if (move)
            {
                _logger.Info($"Moving: {new DirectoryInfo(source).FullName}, to: {new DirectoryInfo(target).FullName}");
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
                File.Move(source, target);
                return;
            }

            _logger.Info($"Copying: {new DirectoryInfo(source).FullName}, to: {new DirectoryInfo(target).FullName}");
            File.Copy(source, target, true);
        }
    }
}
