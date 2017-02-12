using System.Reflection;
using log4net;
using Rbit.CommandLineTool.Interfaces;

namespace Rbit.CommandLineTool.Commands
{
    /// <summary>
    /// This class is meant to act as a command when no command could be found (a null pattern).
    /// </summary>
    internal class UnknownCommand : ICommand, ICommandFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            logger.Warn(string.Format("Unknown command: {0}, use /? to see help for the utility.", Name));
        }

        public string Name { get; set; }

        public string Description
        {
            get { return ""; }
        }

        public string Usage
        {
            get { return ""; }
        }

        public ICommand CreateCommand(string[] args)
        {
            return this;
        }
    }
}