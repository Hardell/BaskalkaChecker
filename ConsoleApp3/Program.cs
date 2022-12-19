using System;
using System.Speech.Synthesis;

namespace ConsoleApp3
{
    public class HelloSelenium
    {
        public static void Main()
        {
            var synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            var loader = new Loader(synth);
            loader.Load();

            while (true)
            {
                Console.WriteLine("Novy check:");
                
                Console.WriteLine("Tabulka 1/2/3/4:");
                var table = Console.ReadLine();
                var tableNumber = int.Parse(table) - 1;
                
                Console.WriteLine("Hodina OD (e.g. 15 alebo 9.5):");
                var hourStart = Console.ReadLine();
                var hourStartNumber = double.Parse(hourStart);
                
                Console.WriteLine("Hodina DO (e.g. 15 alebo 9.5):");
                var hourEnd = Console.ReadLine();
                var hourEndNumber = double.Parse(hourEnd);

                var weekend = false;
                var day = DateTime.Now.DayOfWeek;
                Console.WriteLine(day);
                day = day + tableNumber;
                Console.WriteLine(day);
                if (day is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    weekend = true;
                }

                day -= 7;
                if (day is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    weekend = true;
                }
                
                Console.WriteLine("Weekend: " + weekend);

                loader.AddChecker(new Checker(hourStartNumber, hourEndNumber, tableNumber, weekend, synth));
            }
        }
    }
}