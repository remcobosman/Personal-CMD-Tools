using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using Rbit.CommandLineTool.Interfaces;
using Rbit.CommandLineTool.Support;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace Rbit.CommandLineTool
{
    internal class Program
    {
        static IKernel _kernel = new StandardKernel();

        static ILogger _logger;

        /// <summary>
        /// Gets the logger associated with the command line tool.
        /// </summary>
        private static ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        private static void Main(string[] args)
        {
            // See if we have to run in verbose mode and configure the logger
            _logger = Program.ConfigureLog(args.Contains("-verbose") || args.Contains("-v"));

            Console.WriteLine();

            // I am using an ILMerge tool ILMerge-GUI that messes up the assembly info, its shows the title and description from the Microsoft CRM sdk in stead of
            // this one. However the version does show up correctly.
            Console.WriteLine(string.Format("{0} version {1}",
                                "Rbit Command Tooling",
                                Assembly.GetExecutingAssembly().GetName().Version));

            Console.WriteLine();

            try
            {
                // load all commands into the kernel
                LoadAvailableCommands(_kernel);

                // See if we have any parameters
                if (args.Length == 0 || args[0] == "/?" || args[0] == "-?")
                {
                    // No command specified or /? was specified, show the possible commands for the tool
                    PrintUsage(_kernel.GetAll<ICommandFactory>());
                }
                else
                {
                    try
                    {
                        // Initialize the command manager with the defined commands for this tool and the logger we want to use
                        var manager = new CommandManager(_kernel.GetAll<ICommandFactory>());

                        // Parse the arguments to get the required command we need to execute, syntax is 'tool.exe command [parameters]'
                        var command = manager.ParseCommand(args);

                        if (args.Length> 1 && (args[1] == "/?" || args[1] == "-?"))
                        {
                            WriteCommandUsage(command as ICommandFactory);
                        }
                        else
                        {
                            // Validate the command to see if the mandatory parameters are specified.
                            if (command.CanExecute())
                            {
                                Logger.Info(string.Format("Started at {0}", DateTime.Now));

                                // Execute the specified command
                                command.Execute();
                            }
                            else
                            {
                                Logger.Warn(string.Format("Incorrect command parameters specified for: {0}. Please see the usage for help on the required command.", args[0]));

                                // Cast it to the command factory as we know this should be implemented on the command object
                                WriteCommandUsage(command as ICommandFactory);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "We encountered an exception: \r\n '{0}'\r\nPlease verify the given command and parameters and try again.", ex.Message, ex);
                    }

                    Logger.Info(string.Format("Ended at {0}", DateTime.Now));
                }
            }
            catch(ReflectionTypeLoadException ex)
            {
                Logger.Error(ex, "We encountered an exception and cannot continue.\r\n{0}", ex.Message);
                foreach(var l in ex.LoaderExceptions)
                {
                    Logger.Error(l.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "We encountered an exception and cannot continue.\r\n{0}", ex.Message);
            }
        }

        /// <summary>
        /// Defines the commands that are supported by this tool
        /// </summary>
        /// <returns>A list of commands that can be executed with this tool.</returns>
        private static void LoadAvailableCommands(IKernel kernel)
        {
            // Bind all commands with ninject
            // NOTE: The commands require to be public with a public constructir, else we fail...
            kernel.Bind(x =>
                    x.FromAssembliesMatching("*.dll")
                    .SelectAllClasses()
                    .InheritedFrom<ICommandFactory>()
                    .BindAllInterfaces());
        }

        /// <summary>
        /// Prints out the usage for each command defined in this tool.
        /// </summary>
        /// <param name="commands"></param>
        private static void PrintUsage(IEnumerable<ICommandFactory> commands)
        {
            Console.WriteLine("Possible commands for this console application are:");
            Console.WriteLine();
            foreach (var command in commands)
            {
                WriteCommandUsage(command);
            }
        }

        /// <summary>
        /// Writes out the description and usage of a specific command.
        /// </summary>
        /// <param name="command">The command object.</param>
        private static void WriteCommandUsage(ICommandFactory command)
        {
            Console.WriteLine("Command: {0}", command.Name);
            Console.WriteLine("{0}", command.Description);
            Console.WriteLine("{0}", command.Usage);
            Console.WriteLine();
        }

        /// <summary>
        /// Create a Console Appender for log4net.
        /// </summary>
        /// <param name="level">The required logging level for the appender.</param>
        /// <returns>A configured console appender.</returns>
        private static ColoredConsoleAppender GetConsoleColoredAppender(Level level)
        {
            var appender = new ColoredConsoleAppender
            {
                Name = "Console",
                Layout = new log4net.Layout.PatternLayout("%timestamp | %date | %level | %message%newline"),
                Threshold = level
            };
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Fatal, BackColor = ColoredConsoleAppender.Colors.Red, ForeColor = ColoredConsoleAppender.Colors.White });

            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Error, BackColor = ColoredConsoleAppender.Colors.Red, ForeColor = ColoredConsoleAppender.Colors.White });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Critical, BackColor = ColoredConsoleAppender.Colors.Red, ForeColor = ColoredConsoleAppender.Colors.White });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Emergency, BackColor = ColoredConsoleAppender.Colors.Red, ForeColor = ColoredConsoleAppender.Colors.White });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Warn, BackColor = ColoredConsoleAppender.Colors.Yellow });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Alert, BackColor = ColoredConsoleAppender.Colors.Red, ForeColor = ColoredConsoleAppender.Colors.White });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Severe, BackColor = ColoredConsoleAppender.Colors.Red, ForeColor = ColoredConsoleAppender.Colors.White });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Debug, BackColor = ColoredConsoleAppender.Colors.Green });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Trace, ForeColor = ColoredConsoleAppender.Colors.Yellow });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Verbose, ForeColor = ColoredConsoleAppender.Colors.Yellow });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Info, ForeColor = ColoredConsoleAppender.Colors.White });


            appender.ActivateOptions();

            return appender;
        }

        /// <summary>
        /// Creates a rolling file appender for log4net.
        /// </summary>
        /// <param name="level">The required logging level for the logger.</param>
        /// <returns>A rolling file appender for log4net.</returns>
        private static FileAppender GetFileAppender(Level level)
        {
            var appender = new RollingFileAppender
            {
                Name = "File",
                AppendToFile = true,
                File = string.Format("{0}.log", Assembly.GetExecutingAssembly().GetName().Name),
                Layout = new log4net.Layout.PatternLayout("%timestamp | %date | %level | %message%newline"),
                Threshold = level,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = 10,
                MaximumFileSize = "50MB",
                StaticLogFileName = true
            };

            appender.ActivateOptions();

            return appender;
        }

        /// <summary>
        /// Configures log4net logging.
        /// </summary>
        /// <param name="verbose">Flag indicating wee need verbose logging (including debug level).</param>
        private static ILogger ConfigureLog(bool verbose)
        {
            var logger = new Log4NetLoggerFactory().GetCurrentClassLogger();

            // Now how to set this up

            var level = Level.Info;
            if (verbose)
            {
                level = Level.All;
            }

            var root = ((Hierarchy)LogManager.GetRepository()).Root;
            root.AddAppender(GetConsoleColoredAppender(level));
            root.AddAppender(GetFileAppender(level));
            root.Repository.Configured = true;

            return logger;
        }
    }
}