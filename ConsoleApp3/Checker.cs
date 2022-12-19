using System;
using System.Drawing;
using System.Speech.Synthesis;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ConsoleApp3
{
    public class Checker
    {
        private readonly SpeechSynthesizer _synth;
        
        private Image<Bgr, byte>? _oldImage;
        
        private const int CellWidth = 46*2;
        private const int CellHeight = 26;
        private const int WidthOffset = 133;
        private const int HeightOffset = 313;
        private const int TableHeight = 337;
        private const int TablePadding = 54;
        
        
        private readonly bool _weekend;
        private readonly int _table;
        private readonly double _hourStart;
        private readonly double _hourEnd;

        public Checker(double hourStart, double hourEnd, int table, bool weekend, SpeechSynthesizer synth)
        {
            _hourEnd = hourEnd;
            _hourStart = hourStart;
            _table = table;
            _weekend = weekend;
            _synth = synth;
        }

        public void Check(string newImagePath)
        {
            try
            {
                var newImage = new Image<Bgr, byte>(newImagePath);

                var x1 = WidthOffset + (_hourStart - (_weekend ? 8 : 6.5)) * CellWidth;
                var y1 = HeightOffset + _table * (TableHeight + TablePadding);
                var x2 = WidthOffset + (_hourEnd - (_weekend ? 8 : 6.5)) * CellWidth;
                var y2 = y1 + TableHeight;

                newImage.ROI = new Rectangle((int)x1, y1, (int)(x2 - x1), y2 - y1);

                if (_oldImage != null)
                {
                    var diff = _oldImage - newImage;
                    FindColor(diff);
                    _oldImage = newImage;
                    Thread.Sleep(5000);
                }
                else
                {
                    _oldImage = newImage;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Checking schedule failed. Retrying...");
                _synth.Speak("Couldn't check schedule!");
                Console.WriteLine(e);
            }
        }
        
        private void FindColor(Image<Bgr, byte>? diff)
        {
            for (var i = 0; i < diff.Rows; i++)
            {
                for (var j = 0; j < diff.Cols; j++)
                {
                    if (diff.Data[i, j, 0] == 28)
                    {
                        Console.WriteLine(diff[i,j]);
                        // Speak a string.
                        _synth.Speak("Volny kurt!");
                        
                        Console.Beep(750, 500);
                        
                        // Speak a string.
                        _synth.Speak("Volny kurt!");
                        
                        
                        Console.Beep(750, 500);

                        return;
                    }
                }
            }
        }
    }
}