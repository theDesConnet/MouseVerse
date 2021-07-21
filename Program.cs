using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseVerse
{
    class Program
    {
        [DllImport("kernel32.dll")] //Импортируем в проект функцию GetConsoleWindow
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")] //Импортируем в проект функцию ShowWindow
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        private Point currentPosition;

        private bool running;

        private bool exit;

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            Program inverse = new Program();

            inverse.StartInverse();
            while(true)
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }

        public bool Running
        {
            get
            {
                return this.running;
            }
        }

        private void MouseLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            while (!this.exit)
            {
                Point newPosition = Cursor.Position;

                int bottom = this.currentPosition.Y - (newPosition.X - this.currentPosition.X);
                int maxHeight = SystemInformation.VirtualScreen.Height;
                if (bottom > maxHeight)
                {
                    bottom = maxHeight;
                }
                else if (bottom < 0)
                {
                    bottom = 0;
                }

                int right = this.currentPosition.X + (newPosition.Y - this.currentPosition.Y);
                int maxWidth = SystemInformation.VirtualScreen.Width;
                if (right > maxWidth)
                {
                    right = maxWidth;
                }
                else if (right < 0)
                {
                    right = 0;
                }

                Cursor.Position = new Point(right, bottom);
                this.currentPosition = Cursor.Position;
                Thread.Sleep(1);
            }
            this.exit = false;
        }

        public void StartInverse()
        {
            this.currentPosition = Cursor.Position;
            this.running = true;
            (new Thread(new ThreadStart(this.MouseLoop))).Start();
        }

        public void StopInverse()
        {
            this.running = false;
            this.exit = true;
        }
    }
}
