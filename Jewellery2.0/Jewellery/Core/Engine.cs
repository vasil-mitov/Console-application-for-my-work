﻿
namespace GoldJewelry.Core
{
    using Contracts;
    using GoldJewelry.IO.Contracts;
    using System.Globalization;

    public class Engine : IEngine
    {
        private readonly IReader reader;
        private readonly IWriter fileWriter;
        private readonly IWriter consoleWriter;


        public Engine(IReader reader, IWriter fileWriter, IWriter consoleWriter)
        {
            this.reader = reader;
            this.fileWriter = fileWriter;
            this.consoleWriter = consoleWriter;
        }

        public void Run()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            this.consoleWriter.WriteLine("Create folders? Y/N: ");

            string folderAnser = this.reader.ReadLine().ToLower();

            var clearConsole = this.consoleWriter as IClearable;
            clearConsole.Clear();

            bool wantFolders = (folderAnser == "y" || folderAnser == "у") ? true : false;

            this.consoleWriter.WriteLine("Enter price per gram: ");
            decimal pricePerGram = decimal.Parse(this.reader.ReadLine());

            this.consoleWriter.Write("Enter sell price per gram: ");
            decimal sellPrice = decimal.Parse(this.reader.ReadLine());

            this.consoleWriter.Write("Enter online shop sell price per gram: ");
            decimal onlinePrice = decimal.Parse(this.reader.ReadLine());

            var time = DateTime.Now;

            List<IJewelry> jewels = new List<IJewelry>();

            var txtPath = @$"..\Jewelry";

            if (!Directory.Exists(txtPath))
            {
                Directory.CreateDirectory(txtPath);
            }

            StreamWriter streamWriter = new StreamWriter(@$"..\Jewelry\Jewelry - {time.Day}.{time.Month}.{time.Year}.txt");

            string header = "|  Артикул  |  Грам  |  Главница  |  Цена продава  |  Онлайн  |";

            using (streamWriter)
            {
                string dividingLine = new string('-', header.Length);
                streamWriter.WriteLine(header);

                var counter = 0;

                while (true)
                {
                    string input = Console.ReadLine();

                    if (input.ToLower() == "end" || input.ToLower() == "край")
                    {
                        break;
                    }

                    streamWriter.WriteLine(dividingLine);

                    string[] args = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    string type = args[0];
                    double weight = double.Parse(args[1], culture);

                    Jewelry jewel = new Jewelry(type, weight);
                    jewels.Add(jewel);

                    string size = (args.Length == 3) ? args[2] : null;

                    if (wantFolders)
                    {
                        string foldersPath = $@"..\Jewelry\Folders\{type} - {weight}";

                        if (size != null && size.All((char c) => char.IsDigit(c)))
                        {
                            foldersPath += $" - {size}р-р";
                        }

                        if (Directory.Exists(foldersPath))
                        {
                            foldersPath += $"({++counter})";
                        }

                        Directory.CreateDirectory(foldersPath);
                    }

                    decimal price = (decimal)jewel.Weight * pricePerGram;

                    decimal sellSum = Math.Round((decimal)jewel.Weight * sellPrice);
                    decimal onlineSell = Math.Round((decimal)jewel.Weight * onlinePrice);

                    if (size != null && size.All((char c) => char.IsDigit(c)))
                    {
                        streamWriter.WriteLine($"{jewel.Type} - {jewel.Weight}g({size}р) * {pricePerGram}лв. = {price:f2}лв.|{sellPrice}лв. = {sellSum:f2}лв.|{onlinePrice}лв. = {onlineSell:f2}лв.|");
                    }
                    else
                    {
                        streamWriter.WriteLine($"{jewel.Type} - {jewel.Weight}g * {pricePerGram}лв. = {price:f2}лв.|{sellPrice}лв. = {sellSum:f2}лв.|{onlinePrice}лв. = {onlineSell:f2}лв.|");
                    }
                }

                streamWriter.WriteLine(dividingLine);
                double totalWeight = jewels.Sum((IJewelry j) => j.Weight);
                decimal totalSum = (decimal)totalWeight * pricePerGram;
                string footer = $"Total weight: {totalWeight}g * {pricePerGram}лв. = {totalSum:f2}лв.";

                streamWriter.WriteLine(footer);
            }
        }
    }
}
