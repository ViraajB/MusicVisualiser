using Love;
using NAudio.Wave;
using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Collections.Generic;

namespace Computing_Project
{
    class visBars : visWindow
    {
        private WaveBuffer buffer;
        private List<Complex[]> smooth = new List<Complex[]>();
        private int numbars = 64;
        private bool hidden = false;

        public override void Load()
        {
            WindowTitle = "Bar Visualiser";
            base.Load();

            //start audio capture
            var capture = new WasapiLoopbackCapture();
            capture.DataAvailable += DataAvailable;
            capture.RecordingStopped += (s, a) =>
            {
                capture.Dispose();
            };
            capture.StartRecording();
        }

        public void DataAvailable(object sender, WaveInEventArgs e)
        {
            buffer = new WaveBuffer(e.Buffer); //saves buffer in the class variable
            int len = buffer.FloatBuffer.Length / 8;

            //fft begins here
            Complex[] values = new Complex[len];
            for (int i = 0; i < len; i++)
                values[i] = new Complex(buffer.FloatBuffer[i], 0.0);
            Fourier.Forward(values, FourierOptions.Default);

            //shift array
            smooth.Add(values);
            if (smooth.Count > 3)
                smooth.RemoveAt(0);
        }
        public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat)
        {
            base.KeyPressed(key, scancode, isRepeat);

            switch (key)
            {
                case KeyConstant.Up:
                    numbars += 64;
                    break;

                case KeyConstant.Down:
                    if(numbars > 64)
                    {
                        numbars -= 64;
                    }
                    break;

                case KeyConstant.H:
                    hidden = !hidden;
                    break;
            }
        }
        public double Scale(int i)
        {
            var s = smooth.ToArray();

            double value = 0;

            for (int h = Math.Max(i - 1, 0); h < Math.Min(i + 1, numbars); h++)
            {
                for (int v = 0; v < s.Length; v++)
                {
                    value += (Math.Abs(s[v] != null ? s[v][i].Magnitude : 0.0)) / s.Length;
                }
                value /= 4;
            }

            return value;
        }

        public override void Draw()
        {
            Graphics.SetColor(1, 1, 1);
            if(buffer == null)
            {
                Graphics.Print("No buffer available");
                return;
            }
            if(hidden == false)
            {
                Graphics.Print(
                    "Press 'Escape' to exit" +
                    "\nPress 'F' to enter or exit full screen mode" +
                    "\nPress 'H' to hide the text" +
                    "\nPress 'up' to increase number of bars" +
                    "\nPress 'down' to decrease number of bars" +
                    "\nNumber of bars = " + numbars.ToString()
                    );
            }

            float size = WindowWidth / numbars;
            for (int i = 0; i < numbars; i++)
            {
                double value = Scale(i);
                value = ((value * (WindowHeight / 2)) + (Scale(i - 1) + Scale(i + 1))) / 3;
                Graphics.SetColor(colour.r, colour.g, colour.b);
                Graphics.Rectangle(DrawMode.Fill, i * size, WindowHeight, size, (float)-value);
            }
        }
    }
}
