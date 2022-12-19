using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace ConsoleApp3
{
    public class Loader
    {
        private readonly SpeechSynthesizer _synth;
        private readonly EdgeDriver _driver;
        private const string FileName = "screen.png";
        private readonly HashSet<Checker> _checkers = new();

        public Loader(SpeechSynthesizer synth)
        {
            _synth = synth;

            var opts = new EdgeOptions();
            opts.AddArgument("headless");
            opts.AddArgument("--log-level=3");
            _driver = new EdgeDriver(opts);
            _driver.Manage().Window.Size = new Size(1800, 2000);
        }

        public async Task Load()
        {
            Console.WriteLine("Starting...");
            while (true)
            {
                try
                {
                    if (!_checkers.Any())
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    
                    _driver.Navigate().GoToUrl("http://baskalka.e-rezervace.cz/Branch/pages/Schedule.faces");

                    await Task.Delay(4000);
                    

                    Screenshot takeScreenshot = ((ITakesScreenshot) _driver).GetScreenshot();
                    var fileName = DateTime.UtcNow.Ticks + FileName;
                    takeScreenshot.SaveAsFile(fileName);
                    //Console.WriteLine($"Got a new schedule at {DateTime.Now}");

                    lock (_checkers)
                    {
                        foreach (var checker in _checkers)
                        {
                            checker.Check(fileName);
                        }
                    }
                    
                    File.Delete(fileName);
                    
                    await Task.Delay(6000);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Loading Skalka schedule failed! Retrying...");
                    _synth.Speak("Couldn't load schedule.");
                    Console.WriteLine(e);
                }
            }
        }
        
        public void AddChecker(Checker checker)
        {
            lock (_checkers)
            {
                _checkers.Add(checker);
            }

            Console.WriteLine("Added a new checker.");
        }
    }
}