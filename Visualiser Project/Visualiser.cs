using Love;
using NAudio.Wave;
using NAudio.Dsp;
using System;

namespace Visualiser_Project
{
    class Visualiser : VisWindow
    {
        //*bar specific variables
        int M = 6;

        //*used in both
        WaveBuffer buffer;
        bool hidden = false;
        int visType = 0;
        const int maxVisType = 1;
        bool changeColour = false;
        /*
         * Changing this to an integer allows more freedom, e.g. if I want to add more types.
         * 0 = Bar
         * 1 = Graph
         */

        //*graph specific variables
        int intensity = 2;

        //*colour variables
        int red = 255; //defaults to red
        int green = 000;
        int blue = 000;
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
            buffer = new WaveBuffer(e.Buffer); //*saves buffer in the class variable
        }
        public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat)
        {
            base.KeyPressed(key, scancode, isRepeat);

            switch (key)
            {
                case KeyConstant.S:
                    if (visType < maxVisType)
                    {
                        visType += 1;
                    }else if (visType == maxVisType)
                    {
                        visType -= 1;
                    }
                    else
                    {
                        visType = 0; //! default to 0 in case of error 
                    }
                    break;

                case KeyConstant.H:
                    hidden = !hidden;
                    break;

                case KeyConstant.C:
                    changeColour = !changeColour;
                    hidden = !hidden;
                    break;
            }

            if (changeColour == true) //*add colours here
            {
                switch (key)
                {
                    case KeyConstant.R:
                        red = 255;
                        green = 000;
                        blue = 000;
                        break;

                    case KeyConstant.G:
                        red = 000;
                        green = 255;
                        blue = 000;
                        break;

                    case KeyConstant.B:
                        red = 000;
                        green = 000;
                        blue = 255;
                        break;

                    case KeyConstant.P:
                        red = 128;
                        green = 000;
                        blue = 128;
                        break;

                    case KeyConstant.W:
                        red = 255;
                        green = 255;
                        blue = 255;
                        break;

                    case KeyConstant.Y:
                        red = 255;
                        green = 255;
                        blue = 000;
                        break;
                }
            }
        }
        public override void Draw()
        {
            Graphics.SetColor(255, 255, 255);
            if (buffer == null)
            {
                Graphics.Print("No buffer available");
                return;
            }

            if (hidden == false) //*prints the text that describes what each key does
            {
                Graphics.Print(
                    "Press 'Escape' to exit" + 
                    "\nPress 'F' to enter or exit full screen mode" +
                    "\nPress 'H' to hide this text" + 
                    "\nPress 'S' to change the visualiser style" + 
                    "\nPress 'C' to change colour"
                );
            }

            if (changeColour == true)
            {
                Graphics.Print(
                        "Press 'R' for red" + 
                        "\nPress 'G' for green" + 
                        "\nPress 'B' for blue" + 
                        "\nPress 'P' for purple" + 
                        "\nPress 'W' for white" + 
                        "\nPress 'Y' for yellow" + 
                        "\nPress 'C' to return to the previous menu"
                    );
            }
            
            if (visType == 0)
            {
                int len = buffer.FloatBuffer.Length / 8;

                //fft
                Complex[] values = new Complex[len];
                for (int i = 0; i < len; i++)
                {
                    values[i].Y = 0;
                    values[i].X = buffer.FloatBuffer[i];

                }
                FastFourierTransform.FFT(true, M, values);

                float size = (float)WindowWidth / ((float)Math.Pow(2, M));

                Graphics.SetColor(red, green, blue);
                for (int i = 1; i < Math.Pow(2, M); i++)
                {
                    Graphics.Rectangle(DrawMode.Fill, (i - 1) * size, WindowHeight, size, -Math.Abs(values[i].X) * (WindowHeight / 2) * 8);
                }
            }
            
            if (visType == 1)
            {
                int len = buffer.FloatBuffer.Length / 10;
                float spp = len / WindowWidth; //*samples per pixel

                for (int i = 0; i < WindowWidth; i++)
                {
                    //*current sample
                    int x = (int)Math.Round(i * spp);
                    float y = buffer.FloatBuffer[i];

                    //*previous sample
                    int prevx = x - 1;
                    int previ = (int)Math.Round((i - 1) * spp);
                    float prevy = buffer.FloatBuffer[Math.Max(previ, 0)]; //*Math.Max is used to prevent out of bounds error (0 is used as a fallback)

                    //*render graph
                    Graphics.SetColor(red, green, blue);
                    Graphics.Line(prevx, WindowHeight / 2 + prevy * (WindowHeight / (intensity * 2)), x, WindowHeight / 2 + y * (WindowHeight / (intensity * 2)));
                }
            }
        }
    }
}