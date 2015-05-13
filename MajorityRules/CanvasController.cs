using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;



namespace SurfaceApplication1
{
    class CanvasController
    {
        private Canvas _mainCanvas;
        private List<IdeaBall> items = new List<IdeaBall>();
        private int _viewportWidthMax = 600;
        private int _viewportWidthMin = 0;
        private int _viewportHeightMax = 600;
        private int _viewportHeightMin = 0;
        private const int Fps = 60;
        private Random random = new Random();
        private Stopwatch stopWatch = new Stopwatch();
        private int lastTime;
        private int deltaTime;

        public CanvasController(Canvas MainCanvas)
        {
            stopWatch.Start();
            lastTime = stopWatch.Elapsed.Milliseconds;
            // TODO: Complete member initialization
            this._mainCanvas = MainCanvas;

            for (int i = 0; i < 200; i++)
            {
                items.Add(new IdeaBall(new Vector(random.Next(0, 800), random.Next(0, 600)), new Vector(random.Next(2, 5), random.Next(2, 5))));
            }
            //items.Add(new IdeaBall(new Vector(100, 100), new Vector(5.1, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            //items.Add(new IdeaBall(new Vector(200, 200), new Vector(5, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            foreach (IdeaBall ball in items)
            {
                MainCanvas.Children.Add(ball.Ellipse);
            }

            DispatcherTimer timer = new DispatcherTimer();
            //System.Timers.Timer timer = new System.Timers.Timer();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / Fps);
            timer.Tick += new EventHandler(Update);
            timer.Start();
        }



        private void Update(object sender, EventArgs e)
        {
            int currentTime = stopWatch.Elapsed.Milliseconds;
            deltaTime = lastTime - currentTime;
            //Debug.WriteLine("----");
            //Debug.WriteLine(deltaTime);
            //Debug.WriteLine(1000/deltaTime);

            lastTime = currentTime;
            Random randomGenerator = new Random();
            foreach (IdeaBall ball in items){

                ball.Velocity *= 0.99;
                if (Math.Abs(ball.Velocity.X) < 0.00001) //TODO threashold
                {
                    ball.Velocity = new Vector(0, ball.Velocity.Y);
                }
                if (Math.Abs(ball.Velocity.Y) < 0.00001)
                {
                    ball.Velocity = new Vector(ball.Velocity.Y, 0);
                }

                ball.Position += ball.Velocity;

                if (ball.Position.X >= _viewportWidthMax - ball.Radius && ball.Velocity.X > 0) ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
                if (ball.Position.X <= _viewportWidthMin + ball.Radius && ball.Velocity.X < 0) ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
                if (ball.Position.Y >= _viewportHeightMax - ball.Radius && ball.Velocity.Y > 0) ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
                if (ball.Position.Y <= _viewportHeightMin + ball.Radius && ball.Velocity.Y < 0) ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);

                ball.Draw();
            }
            foreach (IdeaBall ball in items)
            {
                ball.DetectCollisions(items);
            }

            


            
        }

        public void DetectCollisions()
        {

        }





    }
}


//       MainCanvas.Children.Add(new Ellipse() { Width = 100, Height = 100, Fill = SurfaceColors.Accent1Brush });