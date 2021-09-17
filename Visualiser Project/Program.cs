using Love;
using NAudio.Wave;
using NAudio.Dsp;
using System;

namespace Visualiser_Project
{
    class Program : Scene
    {
        //*used in both
        WaveBuffer buffer;
        bool hidden = false;
        int visType = 0; //*default vistype is bar
        bool changeColour = false;
        /*
         * Changing this to an integer allows more freedom, e.g. if I want to add more types.
         * 0 = Bar
         * 1 = Graph
         */

        //*graph specific variables
        int sensitivity = 2;

        //*colour variables
        public struct Colour
        {
            public int red;
            public int green;
            public int blue;
        }
        static Colour userColour = new Colour();

        //bar specific variables
        int barwidth = 32;

        //*window variables
        int WindowHeight;
        int WindowWidth;
        public override void Load()
        {
            userColour.red = 255; //default colour is pure red
            WindowSettings mode = Window.GetMode();
            mode.Resizable = true;
            Window.SetMode(mode);
            Window.SetTitle("Visualiser");
            WindowWidth = Graphics.GetWidth();
            WindowHeight = Graphics.GetHeight();

            //start audio capture
            var capture = new WasapiLoopbackCapture();
            capture.DataAvailable += DataAvailable;
            capture.RecordingStopped += (s, a) =>
            {
                capture.Dispose();
            };
            capture.StartRecording();
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            buffer = new WaveBuffer(e.Buffer); //*saves buffer in the class variable
        }

        public override void WindowResize(int w, int h)
        {
            WindowWidth = w;
            WindowHeight = h;
        }

        public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat)
        {
            if (key == KeyConstant.F)
            {
                Window.SetFullscreen(!Window.GetFullscreen()); //*sets fullscreen only if the current windowmode is not fullscreen
            }
            if (key == KeyConstant.Escape)
            {
                Environment.Exit(0);
            }

            switch (key)
            {
                case KeyConstant.Number1:
                    {
                        visType = 0;
                    }
                    break;

                case KeyConstant.Number2:
                    {
                        visType = 1;
                    }
                    break;

                case KeyConstant.H:
                    {
                        hidden = !hidden;
                    }
                    break;

                case KeyConstant.C:
                    {
                        changeColour = !changeColour;
                        hidden = !hidden;
                    }
                    break;
            }

            if (changeColour == true) //*add colours here
            {
                switch (key)
                {
                    case KeyConstant.R:
                        {
                            userColour.red = 255;
                            userColour.green = 000;
                            userColour.blue = 000;
                        }
                        break;

                    case KeyConstant.G:
                        {
                            userColour.red = 000;
                            userColour.green = 255;
                            userColour.blue = 000;
                        }
                        break;

                    case KeyConstant.B:
                        {
                            userColour.red = 000;
                            userColour.green = 000;
                            userColour.blue = 255;
                        }
                        break;

                    case KeyConstant.P:
                        {
                            userColour.red = 128;
                            userColour.green = 000;
                            userColour.blue = 128;
                        }
                        break;

                    case KeyConstant.W:
                        {
                            userColour.red = 255;
                            userColour.green = 255;
                            userColour.blue = 255;
                        }
                        break;

                    case KeyConstant.Y:
                        {
                            userColour.red = 255;
                            userColour.green = 255;
                            userColour.blue = 000;
                        }
                        break;
                }
            }
        }

        public override void WheelMoved(int x, int y)
        {
            if (visType == 0)
            {
                barwidth = Math.Max(barwidth - y, 1);
            }
            else if (visType == 1)
            {
                sensitivity = Math.Max(sensitivity - y, 1);
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
                    "\nUse the number keys to change the visualiser type" +
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
                if (hidden == false)
                {
                    Graphics.Print(
                        "\n" + "\n" + "\n" + "\n" + "\n" +
                        "\nUse the mouse wheel to adjust the bar width modifier" +
                        "\nCurrent bar width modifier: " + barwidth
                    );
                }

                int len = buffer.FloatBuffer.Length / 8;
                //fft
                Complex[] values = new Complex[len];
                for (int i = 0; i < len; i++)
                {
                    values[i].Y = 0;
                    values[i].X = buffer.FloatBuffer[i];
                }
                FastFourierTransform.FFT(true, 8, values);

                int size = WindowWidth / barwidth;

                Graphics.SetColor(userColour.red, userColour.green, userColour.blue);
                for (int i = 1; i < barwidth; i++)
                {
                    //Graphics.Print((values[i].X).ToString());
                    Graphics.Rectangle(DrawMode.Fill, (i - 1) * size, WindowHeight, size, -Math.Abs(values[i].X) * (WindowHeight / 4) * 10);
                }
            }

            if (visType == 1)
            {
                if (hidden == false)
                {
                    Graphics.Print(
                        "\n" + "\n" + "\n" + "\n" + "\n" +
                        "\nUse the mouse wheel to adjust sensitivity" +
                        "\nCurrent sensitivity = " + sensitivity
                    );
                }

                int len = buffer.FloatBuffer.Length / 10;
                int spp = len / WindowWidth; //*samples per pixel
                for (int i = 0; i < WindowWidth; i++)
                {
                    //*current sample
                    int x = i * spp;
                    float y = buffer.FloatBuffer[i];

                    //*previous sample
                    int prevx = x - 1;
                    int previ = (i - 1) * spp;
                    float prevy = buffer.FloatBuffer[Math.Max(previ, 0)]; //*Math.Max is used to prevent out of bounds error (0 is used as a fallback)

                    //*render graph
                    Graphics.SetColor(userColour.red, userColour.green, userColour.blue);
                    Graphics.Line(prevx, WindowHeight / 2 + prevy * (WindowHeight / (sensitivity * 2)), x, WindowHeight / 2 + y * (WindowHeight / (sensitivity * 2)));
                }
            }
        }

        static void Main()
        {
            Boot.Run(new Program());
        }
    }
}