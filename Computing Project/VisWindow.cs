using Love;
using System;

namespace Computing_Project
{
    class VisWindow : Scene
    {
        public int WindowWidth;
        public int WindowHeight;
        public string WindowTitle;

        public override void Load()
        {
            WindowSettings mode = Window.GetMode();
            mode.Resizable = true;
            Window.SetMode(mode);
            Window.SetTitle(WindowTitle);
            WindowWidth = Graphics.GetWidth();
            WindowHeight = Graphics.GetHeight();
        }

        public override void WindowResize(int w, int h)
        {
            WindowWidth = w;
            WindowHeight = h;
        }

        public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat)
        {
            if (key == KeyConstant.F) Window.SetFullscreen(!Window.GetFullscreen());
            if (key == KeyConstant.Escape) Environment.Exit(0);//This now works but complains about cleanup - not an issue OS handles it for me
        }
    }
}
