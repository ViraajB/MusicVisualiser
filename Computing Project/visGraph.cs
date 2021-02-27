using System;
using Love;
using NAudio.Wave;

namespace Computing_Project
{
    class visGraph : visWindow
    {
        private WaveBuffer buffer;

        public override void Load()
        {
            WindowTitle = "Visualiser - Graph";
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
            buffer = new WaveBuffer(e.Buffer);
        }
        //maybe add the key presses to manipulate the graph...
        //maybe add the code required to mess with the intensity...

        public override void Draw()
        {
            Graphics.SetColor(1, 1, 1);
            if (buffer == null)
            {
                Graphics.Print("No buffer available");
                return;
            }
            Graphics.Print("Press 'Escape' to exit" + "\nPress 'F' to enter or exit fullscreen mode");
            
            int len =buffer.FloatBuffer.Length / 10;
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
                Graphics.SetColor(204, 0, 204); //change colour here
                Graphics.Line(prevx, WindowHeight / 2 + prevy * (WindowHeight / 4), x, WindowHeight / 2 + y * (WindowHeight / 4));
            }
        }
    }
}