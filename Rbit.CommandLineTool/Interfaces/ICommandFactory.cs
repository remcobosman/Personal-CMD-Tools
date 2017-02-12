namespace Rbit.CommandLineTool.Interfaces
{

    /// <summary>
    /// This interface defines the factory class for creating commands commands.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Implement this to specifiy a meaningfull name for the command. This name is used to specify 
        /// which command should be executed and how it is retrieved from the collection of commands.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Implement this to be able to show the calling user a meaningfull message about what the command should be used for.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Implement this to show the calling user how the command should be used.
        /// </summary>
        string Usage { get; }

        /// <summary>
        /// Implement this to create an instance of the command to be able to execute it. For instance you parse any parameters required for the command.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A command object that can be executed.</returns>
        ICommand CreateCommand(string[] args);
    }
}