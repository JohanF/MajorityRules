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
using Microsoft.Surface.Presentation.Input;



namespace SurfaceApplication1
{
    public class CanvasController
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
        private double gravity = 1.5;
        private double rotation = 0;
        private Point centerOfGravity;
        private Point centerOfRotation;
        private double theta;
        private Ellipse eli;
        private const int InitRadius = 25;  
        private int rotationDampening = 15;
        private List<IdeaBall> gravityWells;
        private VoteBall yesBall;
        private VoteBall noBall;
        private bool votingMode;
        private bool gravityEnabled;
        private GravityBall gravityButtonBall;
        private BallInfoText ballInfoText;

        private int _radius;
        private Canvas MainCanvas;
        private bool addBallClicked;
        private int safeZoneY;
        private int safeZoneX;
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

            this.gravityWells = new List<IdeaBall>();
            this.votingMode = false;

            stopWatch.Start();
            lastTime = stopWatch.Elapsed.Milliseconds;
            // TODO: Complete member initialization
            this._mainCanvas = MainCanvas;
            this.centerOfGravity = new Point();
            this.centerOfRotation = new Point();
            this.gravityEnabled = true;

            centerOfGravity.X = width / 2;
            centerOfGravity.Y = height/2;

            centerOfRotation.X = width / 2;
            centerOfRotation.Y = height / 2 + 100;


            safeZoneY = 50;
            safeZoneX = 50;

            theta = 180 / Math.PI;

            

            //TODO Get balls from JSON
            for (int i = 0; i < 1; i++)
            {
                ideaBalls.Add(new IdeaBall(new Vector(random.Next(151, 800), random.Next(0, 600)), new Vector(random.Next(2, 5), random.Next(2, 5)), this._mainCanvas, random.Next(2, 8) * 10, Color.FromArgb((byte)random.Next(100, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)), this, "hej"));
            }
            //items.Add(new IdeaBall(new Vector(100, 100), new Vector(5.1, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            //items.Add(new IdeaBall(new Vector(200, 200), new Vector(5, 5)));
            //items.Add(new IdeaBall(new Vector(500, 500), new Vector(-5, -5)));

            //add buttonBall to add new idea
            addButtonBall = new ButtonBall(new Vector(random.Next(151, 800), random.Next(0, 600)), new Vector(random.Next(2, 5), random.Next(2, 5)), this._mainCanvas, random.Next(2, 8) * 10, Color.FromArgb(255, 255, 0, 0), this, "Add new Idea");
            gravityButtonBall = new GravityBall(new Vector(random.Next(151, 800), random.Next(0, 600)), new Vector(random.Next(2, 5), random.Next(2, 5)), this._mainCanvas, random.Next(2, 8) * 10, Color.FromArgb(255, 0, 0, 255), this, "Add new Idea");
            allBalls.AddRange(ideaBalls);
            allBalls.Add(addButtonBall);
            allBalls.Add(gravityButtonBall);


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

           // MainCanvas.ManipulationDelta += new EventHandler<System.Windows.Input.ManipulationDeltaEventArgs>(MainCanvas_ManipulationDelta);
            yesBall = new VoteBall(new Vector(0, 0), this, Color.FromArgb(0, 0, 255, 0), 25, true);
            noBall = new VoteBall(new Vector(0, 0), this, Color.FromArgb(0, 255, 0, 0), 25, false);
            ballInfoText = new BallInfoText(new Vector(0, 0), this, 250, 150, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean luctus aliquam justo quis iaculis. Donec a purus dignissim, scelerisque massa quis, dignissim erat. Fusce vestibulum ante eu lacinia interdum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. In hac habitasse platea dictumst. Nam orci turpis, imperdiet vitae ligula id, cursus iaculis orci.");
            ballInfoText.grid.Visibility = Visibility.Hidden;
            yesBall.AttachTo(MainCanvas);
            noBall.AttachTo(MainCanvas);
            ballInfoText.AttachTo(MainCanvas);

            //this._mainCanvas.AddHandler(TouchExtensions.TapGestureEvent, new RoutedEventHandler(Canvas_TapGestureEvent));
       
        }

        //protected virtual void Canvas_TapGestureEvent(object sender, RoutedEventArgs e)
        //{
        //    if(addButtonBall.Clicked){
        //        SurfaceWindow1.messageHub.Publish(new DismissTextboxEvent());
        //        addButtonBall.Clicked = false;
        //    }
        //}

        void BackgroundUpdate(object sender, ElapsedEventArgs e)
        {
            int currentTime = stopWatch.Elapsed.Milliseconds;
            deltaTime = lastTime - currentTime;

            lastTime = currentTime;
            Random randomGenerator = new Random();


            foreach (IdeaBall ball in allBalls)
            {
                if (!ball.IsTouched)
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


                    ball.Velocity = ball.Velocity * 0.95;
                    if (!ball.IsTouched && ball.affectedByGravity)
                    {

                        IdeaBall cloxest = null;

                        foreach (IdeaBall ib in gravityWells)
                        {
                            if (inGravityRange(ib, ball))
                            {
                                if (cloxest != null)
                                {
                                    if ((gravDist(ib, ball) < gravDist(cloxest, ball))) cloxest = ib;
                                }
                                else
                                {
                                    cloxest = ib;
                                }
                            }
                        }





                        if (cloxest != null)
                        {
                            ball.Velocity = ball.Velocity + calcGravity(ball.Position.X, ball.Position.Y, (Point)cloxest.Position, gravity, cloxest);
                        }
                        else
                        {
                            ball.Velocity = ball.Velocity + calcGravity(ball.Position.X, ball.Position.Y, centerOfGravity, gravity, null);
                        }
                    }
                    if (ball.Position.X >= _viewportWidthMax - ball.Radius && ball.Velocity.X > 0) ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
                    if (ball.Position.X <= _viewportWidthMin + ball.Radius && ball.Velocity.X < 0) ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
                    if (ball.Position.Y >= _viewportHeightMax - ball.Radius && ball.Velocity.Y > 0) ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
                    if (ball.Position.Y <= _viewportHeightMin + ball.Radius && ball.Velocity.Y < 0) ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
                    if (!ball.affectedByGravity && !ball.IsTouched) ball.Position = ball.gravPosition;

                }
            }
            foreach (IdeaBall ball in allBalls)
            {
                ball.DetectCollisions(allBalls);
                if (this.votingMode)
                {
                    if (ball.runHandler)
                    {
                        yesBall.position.X = ball.Position.X + ball.Radius + 55;
                        yesBall.position.Y = ball.Position.Y;
                        noBall.position.X = ball.Position.X - ball.Radius - 55;
                        noBall.position.Y = ball.Position.Y;
                        ballInfoText.position.X = ball.Position.X;
                        ballInfoText.position.Y = ball.Position.Y + ball.Radius + 75;
                    }
                }
            }
        }

        void MainCanvas_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            Debug.WriteLine(e.DeltaManipulation.Scale);
            foreach (IdeaBall ball in allBalls)
            {
              //  ball.ApplyScale(e.DeltaManipulation.Scale, e.DeltaManipulation.Translation.Length, e.ManipulationOrigin);
            }
            e.Handled = true;
        }



        private void Update(object sender, EventArgs e)
        {
            foreach (IdeaBall ball in allBalls)
            {
                ball.Draw();
            }
            yesBall.Draw();
            noBall.Draw();
            ballInfoText.Draw();

        }

        public void addGravityPoints(IdeaBall b)
        {
            this.gravityWells.Add(b);
        }

        public void removeGravityPoints(IdeaBall b)
        {
            gravityWells.Remove(b);
        }


        private Vector calcGravity(double vX, double vY, Point attractor, double G, IdeaBall b)
        {

            double dY = vY - attractor.Y;
            double dX = vX - attractor.X;


            Vector newGravVelocity = new Vector();



            if (gravityEnabled)
            {

                double angleInDegrees = Math.Atan2(dY, dX) * 180 / Math.PI;

                //b.Text = System.Convert.ToString("Cos = " + Math.Cos(angleInDegrees) + "Sin = " + Math.Sin(angleInDegrees));

                if (Math.Abs(dY) < 3 && dX < 0)
                {
                    newGravVelocity.X = G;
                }
                else if (Math.Abs(dY) < 3 && dX > 0)
                {
                    newGravVelocity.X = -G;
                }
                else if (Math.Abs(dX) < 3 && dY > 0)
                {
                    newGravVelocity.Y = -G;
                }
                else if (Math.Abs(dX) < 3 && dY > 0)
                {
                    newGravVelocity.Y = -G;
                }
                else
                {
                    if (attractor != centerOfGravity)
                    {
                        if (Math.Abs(dY) < (b.Radius + 30) && Math.Abs(dX) < (b.Radius + 30))
                        {
                            newGravVelocity.X = G * (Math.Cos(angleInDegrees)) * 0.1;
                            newGravVelocity.Y = G * (Math.Sin(angleInDegrees)) * 0.1;
                        }
                        else
                        {

                            newGravVelocity.X = G * (Math.Cos(angleInDegrees));
                            newGravVelocity.Y = G * (Math.Sin(angleInDegrees));
                        }
                    }
                    else
                    {
                        if (Math.Abs(dY) < safeZoneY && Math.Abs(dX) < safeZoneX)
                        {
                            newGravVelocity.X = G * (Math.Cos(angleInDegrees)) * 0.1;
                            newGravVelocity.Y = G * (Math.Sin(angleInDegrees)) * 0.1;
                        }
                        else
                        {

                            newGravVelocity.X = G * (Math.Cos(angleInDegrees));
                            newGravVelocity.Y = G * (Math.Sin(angleInDegrees));
                        }
                    }
                }
            }

            return newGravVelocity;

        }

        private static bool inGravityRange(IdeaBall a, IdeaBall b)
        {
            double dX = a.Position.X - b.Position.X;
            double dY = a.Position.Y - b.Position.Y;

            double gravRadius = a.Radius * 2 + b.Radius;
            double sqrRadius = gravRadius * gravRadius;

            double distSqr = (dX * dX) + (dY * dY);

            if (distSqr <= sqrRadius)
            {
                return true;
            }

            return false;
        }

        private double gravDist(IdeaBall a, IdeaBall b)
        {
            double dX = a.Position.X - b.Position.X;
            double dY = a.Position.Y - b.Position.Y;
            double distSqr = (dX * dX) + (dY * dY);
            return distSqr;
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

        public void votingInitiated(IdeaBall b)
        {
            if (!votingMode)
            {
                disableNonFocusedBalls(b);
                yesBall.fill.Color = Color.FromArgb(123, yesBall.fill.Color.R, yesBall.fill.Color.G, yesBall.fill.Color.B);
                yesBall.selectedBall = b;
                yesBall.ballClicked = true;
                Canvas.SetZIndex(yesBall.Ellipse, this._mainCanvas.Children.Count-2);
                noBall.fill.Color = Color.FromArgb(123, noBall.fill.Color.R, noBall.fill.Color.G, noBall.fill.Color.B);
                noBall.selectedBall = b;
                noBall.ballClicked = true;
                Canvas.SetZIndex(noBall.Ellipse, this._mainCanvas.Children.Count-1);
                ballInfoText.grid.Visibility = Visibility.Visible;
                Canvas.SetZIndex(ballInfoText.textBlock, this._mainCanvas.Children.Count);

                this.votingMode = true;
            }
        }

        public void disableNonFocusedBalls(IdeaBall b)
        {
            foreach (IdeaBall ball in allBalls)
            {
                if (!ball.Equals(b))
                {
                    int x = ball.fill.Color.A / 10;
                    ball.fill.Color = Color.FromArgb((byte)x, ball.fill.Color.R, ball.fill.Color.G, ball.fill.Color.B);
                    ball.runHandler = false;

                }
            }
        }

        public void enableNonFocusedBalls(IdeaBall b)
        {
            foreach (IdeaBall ball in allBalls)
            {
                if (ball != b)
                {
                    int x = ball.fill.Color.A * 10;
                    ball.fill.Color = Color.FromArgb((byte)x, ball.fill.Color.R, ball.fill.Color.G, ball.fill.Color.B);
                    ball.runHandler = true;
                }
            }
        }

        public void voteGotClicked(IdeaBall b, int safeZoneTransform)
        {
            enableNonFocusedBalls(b);
            safeZoneX += safeZoneTransform;
            safeZoneY += safeZoneTransform;
            yesBall.fill.Color = Color.FromArgb(0, yesBall.fill.Color.R, yesBall.fill.Color.G, yesBall.fill.Color.B);
            yesBall.ballClicked = false;
            noBall.fill.Color = Color.FromArgb(0, noBall.fill.Color.R, noBall.fill.Color.G, noBall.fill.Color.B);
            noBall.ballClicked = false;
            ballInfoText.grid.Visibility = Visibility.Hidden;
            this.votingMode = false;
        }



        internal void AddBall(string text)
        {
            addButtonBall.Clicked = false;
            enableNonFocusedBalls(addButtonBall);
            Debug.WriteLine("Add Ball");
            IdeaBall ideaBall = new IdeaBall(new Vector(random.Next(151, 800), random.Next(0, 600)), new Vector(0,0), this._mainCanvas, 4 * 10, Color.FromArgb((byte)random.Next(100, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)), this, text);
            safeZoneX += 10;
            safeZoneY += 10;
            ideaBalls.Add(ideaBall);
            ideaBall.AttachTo(this._mainCanvas);
            allBalls.Add(ideaBall);
        }

        public void disableGravity()
        {
            this.gravityEnabled = false;
        }
        public void enableGravity()
        {
            this.gravityEnabled = true;
        }
    }
}


//       MainCanvas.Children.Add(new Ellipse() { Width = 100, Height = 100, Fill = SurfaceColors.Accent1Brush });