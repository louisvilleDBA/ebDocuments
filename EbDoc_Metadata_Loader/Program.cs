﻿using EbDoc_DAL;
using EbDoc_Processor;
using System.IO;

namespace EbDoc_Metadata_Loader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string metadataFilename;
            if (args.Length == 0)
            {
                metadataFilename = @"Y:\metadata\TRADE_LICENSE_APPLICATIONS.txt";
            }
            else
            {
                metadataFilename = args[0];
            }

            getData(metadataFilename);


            System.Console.WriteLine("\npress any key to coninue");
            System.Console.ReadKey();
        }

        internal static void getData(string metadataFilename)
        {
            if (!File.Exists(metadataFilename))
            {
                string errorMessage = $"{metadataFilename} not found!";
                throw new FileNotFoundException(errorMessage);
            }

            long result = FileProcessor.loadMetadata(new EbDocContext(), metadataFilename);
            System.Console.WriteLine($"[{metadataFilename}] contains [{result}] files");
        }
    }
}