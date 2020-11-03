using Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Console.Services
{
    public interface IArgsService
    {
        Configuration GetArgs(string[] args);
    }

    public class ArgsService : IArgsService
    {
        public Configuration GetArgs(string[] args)
        {
            HelpAsked(args);

            var configuration = new Configuration();

            for (var i = 0; i <= args.Count() - 1; i++)
            {
                var isCommand = i % 2 == 0;

                if (isCommand)
                {
                    var arg = args[i];

                    switch (arg)
                    {
                        case "-f":
                            configuration.File = GetParameter(i, "File", args);
                            break;
                        case "-o":
                            configuration.Output = GetParameter(i, "Output", args);
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid command {arg}. Type --help for help.");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(configuration.File) ||
                string.IsNullOrWhiteSpace(configuration.Output))
            {
                throw new InvalidOperationException("File and output parameters are mandatory. Type --help for help.");
            }

            return configuration;
        }

        private string GetParameter(int index, string command, string[] args)
        {
            if (index + 1 > args.Count() - 1)
            {
                throw new InvalidOperationException($"{command} parameter not informed.");
            }

            return args[index + 1];
        }

        private void HelpAsked(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg == "--help")
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("");
                    sb.AppendLine("These are the Genco commands:");
                    sb.AppendLine("\t-f\t\tfile\t\t\tYour project configuration json file");
                    sb.AppendLine("\t-o\t\toutput\t\t\tOutput folder where your project files will be created");
                    sb.AppendLine("");
                    sb.AppendLine("Go to https://gnllucena.github.io/genco/ for more information.");

                    throw new ArgumentException(sb.ToString());
                }
            }
        }
    }
}
