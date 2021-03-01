using Love;
using NAudio.Wave;
using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Collections.Generic;

namespace Computing_Project
{
    class Visualiser : VisWindow
    {
        private bool barMode = true; //true for bars, false for graph.
        //bar specific variables
        private List<Complex[]> smooth = new List<Complex[]>();
        private int numbars = 64;
        //used in both
        private WaveBuffer buffer;
        private bool hidden = false;
        //graph specific variables
        private int intensity = 4;

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
            if (barMode == true)
            {
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
        }
        public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat)
        {
            base.KeyPressed(key, scancode, isRepeat);

            switch (key)
            {
                case KeyConstant.S:
                    barMode = !barMode;
                    break;
            }
            if (barMode == true)
            {
                switch (key)
                {
                    case KeyConstant.Up:
                        numbars += 64;
                        break;

                    case KeyConstant.Down:
                        if (numbars > 64)
                        {
                            numbars -= 64;
                        }
                        break;

                    case KeyConstant.H:
                        hidden = !hidden;
                        break;
                }
            }else if (barMode == false)
            {
                switch (key)
                {
                    case KeyConstant.Up:
                        if (intensity < 10)
                        {
                            intensity += 1;
                        }
                        break;

                    case KeyConstant.Down:
                        if (intensity > 1)
                        {
                            intensity -= 1;
                        }
                        break;

                    case KeyConstant.H:
                        hidden = !hidden;
                        break;
                }
            }
        }
        public double Scale(int i) //used only when the visualiser is a bar
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
            if (buffer == null)
            {
                Graphics.Print("No buffer available");
                return;
            }
            if (barMode == true)
            {
                if (hidden == false)
                {
                    Graphics.Print(
                        "Press 'Escape' to exit" +
                        "\nPress 'F' to enter or exit full screen mode" +
                        "\nPress 'H' to hide the text" +
                        "\nPress 'S' to change the visualiser style" +
                        "\nUse the 'up' and 'down' keys to change the number of bars" +
                        "\nCurrent number of bars = " + numbars.ToString()
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
            } else if(barMode == false)
            {
                if (hidden == false)
                {
                    Graphics.Print(
                        "Press 'Escape' to exit" +
                        "\nPress 'F' to enter or exit fullscreen mode" +
                        "\nPress 'H' to hide the text" +
                        "\nPress 'S' to change the visualiser style" +
                        "\nUse the 'up' and 'down' keys to change the intensity" +
                        "\nCurrent intensity = " + intensity.ToString()
                        );
                }

                int len = buffer.FloatBuffer.Length / 10;
                int spp = len / WindowWidth; //samples per pixel

                for (int i = 0; i < len; i += spp)
                {
                    //current sample
                    int x = i;
                    float y = buffer.FloatBuffer[i];

                    //previous sample
                    int prevx = i - 1;
                    float prevy = buffer.FloatBuffer[Math.Max((i - spp), 0)]; //Math.Max is used to prevent out of bounds error (0 is used as a fallback).

                    //render graph
                    Graphics.SetColor(colour.r, colour.g, colour.b);
                    Graphics.Line(prevx, WindowHeight / 2 + prevy * (WindowHeight / (intensity * 2)), x, WindowHeight / 2 + y * (WindowHeight / (intensity * 2)));
                    /*
                     * For some reason the line is not "stable", it duplicates the line if it moves too fast.
                     * Going to look into this.
                     */
                }
            }
        }
    }
}
