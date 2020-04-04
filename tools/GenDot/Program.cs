using Superpower;
using System;
using System.Diagnostics;
using System.IO;
using Wgaffa.DMToolkit.Parser;

namespace GenDot
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                string programName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
                PrintUsage(programName);
                return 0;
            }

            var filename = args[0];

            var tokens = new DiceNotationTokenizer().Tokenize(ReadFile(filename));
            var program = DiceNotationParser.Program.Parse(tokens);

            var dotGenerator = new CompileToDot("AST");
            Console.WriteLine(dotGenerator.Evaluate(program));

            return 0;
        }

        private static void PrintUsage(string programName)
        {
            Console.WriteLine($"{programName} <source>");
        }

        private static string ReadFile(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}
