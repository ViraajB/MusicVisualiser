using Love;
using NAudio.Wave;
using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Collections.Generic;

namespace Computing_Project
{
    class VisMaths : VisWindow
    {
        private WaveBuffer buffer;
        private float size = 10;
        private List<Complex[]> smooth = new List<Complex[]>();
        private Complex[] values;

        public override void Load()
        {
            WindowTitle = "Visualiser";
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

            //fft
            values = new Complex[len];
            for (int i = 0; i < len; i++)
                values[i] = new Complex(buffer.FloatBuffer[i], 0.0);
            Fourier.Forward(values, FourierOptions.Default);

            //shift array
            smooth.Add(values);
            if (smooth.Count > 3)
                smooth.RemoveAt(0);
        }
        //There has to be a way to clean up the stuff below...
        public double vSmooth(int i, Complex[][] s)
        {
            double value = 0;

            for (int v = 0; v < s.Length; v++)
                value += Math.Abs(s[v] != null ? s[v][i].Magnitude : 0.0);

            return value / s.Length;
        }
        public double BothSmooth(int i)
        {
            var s = smooth.ToArray();

            double value = 0;

            for (int h = Math.Max(i - 1, 0); h < Math.Min(i + 1, 64); h++)
                value += vSmooth(h, s);

            return value / ((1 + 1) * 2);
        }

        private void DrawVis(int i, double c, float size, double value)
        {
            value *= WindowHeight / 2;
            value += BothSmooth(i - 1) + BothSmooth(i + 1);
            value /= 3;

            Graphics.Rectangle(DrawMode.Fill, i * size, WindowHeight, size, (float)-value);
        }

        public override void Draw()
        {
            Graphics.SetColor(102, 255, 102); //I think this is how you change colours but I have no clue how it works.
            if(buffer == null)
            {
                Graphics.Print("No buffer available");
                return;
            }

            Graphics.Print("Press 'Escape' to exit" + "\nPress 'F' to enter or exit full screen mode");

            size = WindowWidth / 128;
            for (int i = 0; i < 128; i++)
            {
                double value = BothSmooth(i);
                DrawVis(i, 128, size, value);
            }
        }
    }
}
