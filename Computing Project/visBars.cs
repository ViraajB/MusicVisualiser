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

        public override void Load()
        {
            WindowTitle = "Visualiser - Bars";
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
        public double vSmooth(int i, Complex[][] s)
        {
            double value = 0;

            for (int v = 0; v < s.Length; v++)
                value += Math.Abs(s[v] != null ? s[v][i].Magnitude : 0.0); //checks if the current value in the smooth[] is not null and either uses its magnitude or sets it to 0.0

            return value / s.Length;
        }
        public double BothSmooth(int i)
        {
            var s = smooth.ToArray();

            double value = 0;

            for (int h = Math.Max(i - 1, 0); h < Math.Min(i + 1, 128); h++)
                value += vSmooth(h, s);

            return value / ((1 + 1) * 2);
        }

        private void DrawVis(int i, float size, double value)
        {
            value *= WindowHeight / 2;
            value += BothSmooth(i - 1) + BothSmooth(i + 1);
            value /= 3;

            Graphics.SetColor(204, 0, 204); //This sets the colour
            Graphics.Rectangle(DrawMode.Fill, i * size, WindowHeight, size, (float)-value);
        }

        public override void Draw()
        {
            Graphics.SetColor(1, 1, 1);
            if(buffer == null)
            {
                Graphics.Print("No buffer available");
                return;
            }

            Graphics.Print("Press 'Escape' to exit" + "\nPress 'F' to enter or exit full screen mode");

            float size = WindowWidth / 128;
            for (int i = 0; i < 128; i++)
            {
                double value = BothSmooth(i);
                DrawVis(i, size, value);
            }
        }
    }
}
