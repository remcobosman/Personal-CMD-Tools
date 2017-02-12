using System.Collections.Generic;
using System.Linq;
using Rbit.CommandLineTool.Commands;
using Rbit.CommandLineTool.Interfaces;

namespace Rbit.CommandLineTool.Support
{
    /// <summary>
    /// The CommandManager class is used to retrieve the required command factories from the collection of available commands.
    /// </summary>
    internal class CommandManager
    {
        /// <summary>
        /// List of command factories for commands supported for the tool.
        /// </summary>
        private readonly IEnumerable<ICommandFactory> _commands;

        /// <summary>
        /// Initializes a new instance of the CommandManager class.
        /// </summary>
        /// <param name="commands">The list factory classes for the supported commands.</param>
        internal CommandManager(IEnumerable<ICommandFactory> commands)
        {
            _commands = commands;
        }

        /// <summary>
        /// Retrieves the command that is requested by the tool.
        /// </summary>
        /// <param name="args">The arguments given to the commandline, the first parameter should be the name of an available command defined for the tool.</param>
        /// <returns></returns>
        internal ICommand ParseCommand(string[] args)
        {
            // Find the command in the collection
            var command = FindCommand(args[0]);

            // If no command is found a default 'UnknownCommand' is returned that shows just the message that the command specified does not exist.
            // This is the NullObject pattern, the caler can just do its work without any confusing checks and still get correct results
            if (command == null)
            {
                return new UnknownCommand { Name = args[0] };
            }

            // Create the command with the command factory class and return it to the caller so it can be executed.
            return command.CreateCommand(args);
        }

        /// <summary>
        /// Finds a command in the collection of available commands.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <returns>The factory class for creating the command.</returns>
        private ICommandFactory FindCommand(string name)
        {
            return _commands.FirstOrDefault(c => c.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}