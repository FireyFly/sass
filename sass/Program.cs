﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace sass
{
    public class Program
    {
        public static Dictionary<string, InstructionSet> InstructionSets;

        static void Main(string[] args)
        {
            Console.WriteLine("SirCmpwn's Assembler     Copyright Drew DeVault 2012");

            InstructionSets = new Dictionary<string, InstructionSet>();
            InstructionSets.Add("z80", LoadInternalSet("sass.Tables.z80.table"));
            string instructionSet = "z80"; // Default
            string inputFile = null, outputFile = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "--input":
                        case "--input-file":
                            inputFile = args[++i];
                            break;
                        case "--output":
                        case "--output-file":
                            outputFile = args[++i];
                            break;
                        case "--instr":
                            instructionSet = args[++i];
                            break;
                        case "-h":
                        case "-?":
                        case "/?":
                        case "/help":
                        case "-help":
                        case "--help":
                            DisplayHelp();
                            return;
                    }
                }
                else
                {
                    if (inputFile == null)
                        inputFile = args[i];
                    else if (outputFile == null)
                        outputFile = args[i];
                    else
                    {
                        Console.WriteLine("Error: Invalid usage. Use sass.exe --help for usage information.");
                        return;
                    }
                }
            }

            if (inputFile == null)
            {
                Console.WriteLine("No input file specified. Use sass.exe --help for usage information.");
                return;
            }
            if (outputFile == null)
                outputFile = Path.GetFileNameWithoutExtension(inputFile) + ".bin";

            var assembler = new Assembler(InstructionSets[instructionSet]);
            string file = File.ReadAllText(inputFile);
            var output = assembler.Assemble(file, inputFile);

            File.WriteAllBytes(outputFile, output.Data);
            var errors = from l in output.Listing
                         where l.Warning != AssemblyWarning.None || l.Error != AssemblyError.None
                         orderby l.RootLineNumber
                         select l;
            foreach (var listing in errors)
            {
                if (listing.Error != AssemblyError.None)
                    Console.WriteLine(listing.FileName + " [" + listing.LineNumber + "]: Error: " + listing.Error);
                if (listing.Warning != AssemblyWarning.None)
                    Console.WriteLine(listing.FileName + " [" + listing.LineNumber + "]: Warning: " + listing.Warning);
            }
        }

        public static Stream LoadResource(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        }

        private static InstructionSet LoadInternalSet(string name)
        {
            InstructionSet set;
            using (var stream = new StreamReader(LoadResource(name)))
                set = InstructionSet.Load(stream.ReadToEnd());
            return set;
        }

        private static void DisplayHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Usage: Assembler.exe [options] <inputfile> [outputfile]");
            Console.WriteLine("Assemble <inputfile> to produce the output binary [outputfile], or <inputfile>.bin if omitted.");
            Console.WriteLine();
            Console.WriteLine("  --input, --input-file <name>    name of the input assembly file to work on");
            Console.WriteLine("  --output, --output-file <name>  name of the output object file to produce");
            Console.WriteLine("  --instr <instruction-set>       instruction set configuration to use");
            Console.WriteLine("  -h, -?, -help, --help           display this help and exit");
        }
    }
}
