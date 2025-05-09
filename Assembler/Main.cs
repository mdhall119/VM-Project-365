using System;
using System.Collections.Generic;
using System.IO;

class Program 
{
    static void Main(string[] args) 
    {
        if (args.Length != 2) 
        {
            Console.WriteLine("Usage: dotnet run <input_file> <output_file>");
            return;
        }

        string inputFile = args[0];
        string outputFile = args[1];

        try
        {
            new Assembler(inputFile, outputFile);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
}