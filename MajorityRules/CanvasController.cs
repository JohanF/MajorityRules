﻿using System;
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



namespace SurfaceApplication1
{
    class CanvasController
    {
        private Canvas _mainCanvas;
        private List<IdeaBall> items = new List<IdeaBall>();
        private int _viewportWidthMax = 1920;
        private int _viewportWidthMin = 0;
        private int _viewportHeightMax = 1080;
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

        public CanvasController(Canvas MainCanvas)
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

            stopWatch.Start();
            lastTime = stopWatch.Elapsed.Milliseconds;
            // TODO: Complete member initialization
            this._mainCanvas = MainCanvas;
            this.centerOfGravity = new Point();
            this.centerOfRotation = new Point();

            centerOfGravity.X = 1920/2;
            centerOfGravity.Y = 1080/2;

            centerOfRotation.X = 1920/2;
            centerOfRotation.Y = 1080/2 + 100;

            theta = 180 / Math.PI;

            for (int i = 0; i < 80; i++)
            {
                items.Add(new IdeaBall(new Vector(random.Next(151, 800), random.Next(0, 600)), new Vector(random.Next(2, 5), random.Next(2, 5)), this._mainCanvas, random.Next(2, 8) * 10, Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255))));
            }
            //items.Add(new IdeaBall(new Vector(100, 100), new Vector(5.1, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            //items.Add(new IdeaBall(new Vector(200, 200), new Vector(5, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            foreach (IdeaBall ball in items)
            {
                MainCanvas.Children.Add(ball.Ellipse);
            }

            //MainCanvas.Children.Add(this.eli);

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

            //Canvas.SetLeft(eli, centerOfRotation.X);
            //Canvas.SetTop(eli, centerOfRotation.Y);

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


    }
}


//       MainCanvas.Children.Add(new Ellipse() { Width = 100, Height = 100, Fill = SurfaceColors.Accent1Brush });