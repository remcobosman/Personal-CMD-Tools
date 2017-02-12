using Ninject.Extensions.Logging;

namespace Rbit.CommandLineTool.Interfaces
{
    public abstract class CommandBase : ICommand, ICommandFactory
    {
        protected readonly ILogger Logger;

        protected CommandBase(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// The command line arguments for this command.
        /// </summary>
        protected Arguments Arguments { get; set; }

        /// <summary>
        /// Implement this to validate the command. This can be used to validate parameters given to the command so we can assure 
        /// it can be executed correctly or give a usefull message about the usage of the command in case the validate fails.
        /// </summary>
        /// <returns>True if the command can be executed.</returns>
        public abstract bool CanExecute();

        /// <summary>
        /// Implement this with what the command should do.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Implement this to specifiy a meaningfull name for the command. This name is used to specify 
        /// which command should be executed and how it is retrieved from the collection of commands.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Implement this to be able to show the calling user a meaningfull message about what the command should be used for.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Implement this to show the calling user how the command should be used.
        /// </summary>
        public abstract string Usage { get; }

        /// <summary>
        /// Initializes and creates an instance of the command.
        /// </summary>
        /// <param name="args">The command line arguments for the command.</param>
        /// <returns>The current and initialized instance of the command.</returns>
        public ICommand CreateCommand(string[] args)
        {
            Arguments = new Arguments(args);
            return this;
        }
    }
}
