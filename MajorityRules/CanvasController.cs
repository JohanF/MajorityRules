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


namespace SurfaceApplication1
{
    class CanvasController
    {
        private Canvas _mainCanvas;
        private List<IdeaBall> items = new List<IdeaBall>();
        private int _viewportWidthMax = 800;
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

            for(int i = 0; i < 5; i++){
                items.Add(new IdeaBall(random.Next(2,10)));
            }

            foreach (IdeaBall ball in items)
            {
                MainCanvas.Children.Add(ball.Ellipse);
                Canvas.SetLeft(ball.Ellipse, random.Next(50, 500));
                Canvas.SetTop(ball.Ellipse, random.Next(50, 500));

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
            Debug.WriteLine("----");
            Debug.WriteLine(deltaTime);
            Debug.WriteLine(1000/deltaTime);

            lastTime = currentTime;
            Random randomGenerator = new Random();
            foreach (IdeaBall ball in items){

                ball.X += Convert.ToInt16(ball.Vx);
                ball.Y += Convert.ToInt16(ball.Vy);

                if (ball.X >= _viewportWidthMax-ball.Radius && ball.Vx > 0) ball.Vx = -ball.Vx;
			    if (ball.X <= _viewportWidthMin+ball.Radius && ball.Vx < 0) ball.Vx = -ball.Vx;
                if (ball.Y >= _viewportHeightMax-ball.Radius && ball.Vy > 0) ball.Vy = -ball.Vy;
			    if (ball.Y <= _viewportHeightMin+ball.Radius && ball.Vy < 0) ball.Vy = -ball.Vy;

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