using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Rbit.CommandLineTool.RomCommands.Support
{
    public static class GameListManager
    {
        public static string CreateOutputInfoLine(XElement item, List<string> columns)
        {
            if (columns.Count == 0)
            {
                return item.Attribute("id").Value;
            }

            return item.Attribute("id").Value + "," + columns.Aggregate((current, next) => $"{item.Element(current).Value},{item.Element(next).Value}");
        }
    }
}
