using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject.Extensions.Logging;
using Rbit.CommandLineTool.RomCommands.Support;
using Rbit.CommandLineTool.Support;

namespace Rbit.CommandLineTool.RomCommands
{
    public class ListByPublisherCommand : CommandBase
    {
        public ListByPublisherCommand(ILogger logger) : base(logger) { }

        public override bool CanExecute()
        {
            return Arguments.Contains("p") && Arguments.Contains("f");
        }

        public override void Execute()
        {
            _logger.Info($"Listing games by publisher name {Arguments["p"]}, from file {Arguments["f"]}");
            var columns = new List<string>();

            if (Arguments.Contains("c"))
            {
                columns = Arguments["c"].Split(',').ToList();
            }

            var gamelist = XDocument.Load(this.Arguments["f"]);

            var games = gamelist.Descendants().Where(g => g.Element("publisher") != null && g.Element("publisher").Value == Arguments["p"]);

            foreach (var game in games)
            {
                var info = GameListManager.CreateOutputInfoLine(game, columns);
                _logger.Info(info);

                if (Arguments.Contains("o"))
                {
                    WriteToFile(Arguments["o"], info);
                }
            }
        }

        private void WriteToFile(string file, string info)
        {
            using (var f = new StreamWriter(file, true))
            {
                f.WriteLine(info);
            }
        }

        public override string Name => "ListByPublisher";
        public override string Description => "Lists games from the specified gameslist for the given publisher.";
        public override string Usage => "ListGamesPerPublisher -f <gamelists xml file> -p <the name of the publisher (i.e. SNK)>";
    }
}
