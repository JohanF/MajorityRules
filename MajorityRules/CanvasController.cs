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
using System.Windows.Shapes;
using Microsoft.Surface.Presentation.Controls;



namespace SurfaceApplication1
{
    class CanvasController
    {
        private Canvas _mainCanvas;
        private List<IdeaBall> ideaBalls = new List<IdeaBall>();
        private List<IdeaBall> allBalls = new List<IdeaBall>();
        private ButtonBall addButtonBall;
        private int _viewportWidthMax = 0;
        private int _viewportWidthMin = 0;
        private int _viewportHeightMax = 0;
        private int _viewportHeightMin = 0;
        private const int Fps = 60;
        private Random random = new Random();
        private Stopwatch stopWatch = new Stopwatch();
        private int lastTime;
        private int deltaTime;
        private double gravity = 2.5;
        private double rotation = 0;
        private Point centerOfGravity;
        private Point centerOfRotation;
        private double theta;
        private Ellipse eli;
        private const int InitRadius = 25;  
        private int rotationDampening = 15;

        private int _radius;
        private Canvas MainCanvas;
        private int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                eli.Width = value * 2;
                eli.Height = value * 2;
            }
        }

        public CanvasController(Canvas MainCanvas, int width, int height)
        {

            /*
            SolidColorBrush fill = new SolidColorBrush()
            {
                Color = Colors.Blue
            };

            eli = new Ellipse()
            {
                Fill = fill,
            };

            
            Radius = InitRadius;
             */

            Debug.WriteLine("width");
            Debug.WriteLine(width);

            _viewportWidthMax = width;
            _viewportHeightMax = height;

            stopWatch.Start();
            lastTime = stopWatch.Elapsed.Milliseconds;
            // TODO: Complete member initialization
            this._mainCanvas = MainCanvas;
            this.centerOfGravity = new Point();
            this.centerOfRotation = new Point();

            centerOfGravity.X = width / 2;
            centerOfGravity.Y = height/2;

            centerOfRotation.X = width / 2;
            centerOfRotation.Y = height / 2 + 100;

            theta = 180 / Math.PI;

            //TODO Get balls from JSON
            for (int i = 0; i < 8; i++)
            {
                ideaBalls.Add(new IdeaBall(new Vector(random.Next(151, 800), random.Next(0, 600)), new Vector(random.Next(2, 5), random.Next(2, 5)), this._mainCanvas, random.Next(2, 8) * 10, Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255))));
            }
            //items.Add(new IdeaBall(new Vector(100, 100), new Vector(5.1, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            //items.Add(new IdeaBall(new Vector(200, 200), new Vector(5, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            //add buttonBall to add new idea
            addButtonBall = new ButtonBall(new Vector(random.Next(151, 800), random.Next(0, 600)), new Vector(random.Next(2, 5), random.Next(2, 5)), this._mainCanvas, random.Next(2, 8) * 10, Color.FromArgb(255, 255, 0, 0));
            allBalls.AddRange(ideaBalls);
            allBalls.Add(addButtonBall);

            foreach (IdeaBall ball in allBalls)
            {
                ball.AttachTo(MainCanvas);
            }

            

            //MainCanvas.Children.Add(this.eli);

            DispatcherTimer dispatchTimer = new DispatcherTimer();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(BackgroundUpdate);
            timer.Interval = 1000 / Fps;
            timer.Start();

            dispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / Fps);
            dispatchTimer.Tick += new EventHandler(Update);
            dispatchTimer.Start();

            MainCanvas.ManipulationDelta += new EventHandler<System.Windows.Input.ManipulationDeltaEventArgs>(MainCanvas_ManipulationDelta);
        }

      

        void BackgroundUpdate(object sender, ElapsedEventArgs e)
        {
            int currentTime = stopWatch.Elapsed.Milliseconds;
            deltaTime = lastTime - currentTime;

            lastTime = currentTime;
            Random randomGenerator = new Random();

            foreach (IdeaBall ball in allBalls)
            {

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
                if (rotationDampening <= 1)
                {
                    centerOfRotation = RotatePoint(centerOfRotation, centerOfGravity, 1);
                    rotationDampening = 15;
                }
                else
                {
                    rotationDampening--;
                }


                ball.Velocity = ball.Velocity * 0.85;
                if (!ball.IsTouched) ball.Velocity = ball.Velocity + calcGravity(ball.Position.X, ball.Position.Y, centerOfGravity, gravity);
                if (ball.Position.X >= _viewportWidthMax - ball.Radius && ball.Velocity.X > 0) ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
                if (ball.Position.X <= _viewportWidthMin + ball.Radius && ball.Velocity.X < 0) ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
                if (ball.Position.Y >= _viewportHeightMax - ball.Radius && ball.Velocity.Y > 0) ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
                if (ball.Position.Y <= _viewportHeightMin + ball.Radius && ball.Velocity.Y < 0) ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
            }
            foreach (IdeaBall ball in allBalls)
            {
                ball.DetectCollisions(allBalls);
            }
        }

        void MainCanvas_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            Debug.WriteLine(e.DeltaManipulation.Scale);
            foreach (IdeaBall ball in allBalls)
            {
                ball.ApplyScale(e.DeltaManipulation.Scale, e.DeltaManipulation.Translation.Length, e.ManipulationOrigin);
            }
            e.Handled = true;
        }



        private void Update(object sender, EventArgs e)
        {
            foreach (IdeaBall ball in allBalls)
            {
                ball.Draw();
            }
        }

        private Vector calcGravity(double vX, double vY, Point attractor, double G)
        {

            double dY = vY - attractor.Y;
            double dX = vX - attractor.X;



            Vector newGravVelocity = new Vector();

            if (Math.Abs(dY) < 250 && Math.Abs(dX) < 250) return newGravVelocity;

            double angleInDegrees = Math.Atan2(dY, dX) * 180 / Math.PI;

            //b.Text = System.Convert.ToString("Cos = " + Math.Cos(angleInDegrees) + "Sin = " + Math.Sin(angleInDegrees));

            if (Math.Abs(dY) < 1)
            {
                newGravVelocity.X = G;
            } else
            {
                newGravVelocity.X = G * (Math.Cos(angleInDegrees));
                newGravVelocity.Y = G * (Math.Sin(angleInDegrees));
            }

            return newGravVelocity;

        }

        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                (int)
                (cosTheta * (pointToRotate.X - centerPoint.X) -
                sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                (int)
                (sinTheta * (pointToRotate.X - centerPoint.X) +
                cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
        public void openVirtualKeyBoard()
        {

        }
    }
}


//       MainCanvas.Children.Add(new Ellipse() { Width = 100, Height = 100, Fill = SurfaceColors.Accent1Brush });