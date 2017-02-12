namespace Rbit.CommandLineTool.Interfaces
{
    /// <summary>
    /// This interface defines a command that can be executed.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Implement this to validate the command. This can be used to validate parameters given to the command so we can assure 
        /// it can be executed correctly or give a usefull message about the usage of the command in case the validate fails.
        /// </summary>
        /// <returns>True if the command can be executed.</returns>
        bool CanExecute();

        /// <summary>
        /// Implement this with what the command should do.
        /// </summary>
        void Execute();
    }
}