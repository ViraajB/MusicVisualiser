using Love;
using System;

namespace Computing_Project
{
    class visWindow : Scene
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
            if (key == KeyConstant.Escape) Environment.Exit(0); //Closes program.
            /*
             * Initially I tried using Event.Quit(0), when trying to close the program.
             * This led to the program locking up.
             * If I manage to get it working then it would be preferable to the current solution, so I can return to the menu.
             */
        }
    }
}
