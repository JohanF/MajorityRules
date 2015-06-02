using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Surface.Presentation;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using Microsoft.Surface.Presentation.Input;
using System.Windows.Input;
using System.Timers;

namespace SurfaceApplication1
{
    public class IdeaBall
    {
        private const float Friction = 0.6f;
        private const int InitRadius = 25;
        private Random random;
        private const double Restitution = 1;
        private static Vector adjustment = new Vector(0.0001, 0.0001);
        private TouchDevice ellipseControlTouchDevice;
        private Canvas mainCanvas;
        private Point lastPoint;
        private Point releasePoint;
        protected double deltaX;
        protected double deltaY;
        protected Boolean isPressingMovement;
        private const int holdTime = 10;
        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer pulseTimer = new System.Timers.Timer();
        private int timerReset;
        private CanvasController CanvasCtrl;
        private String text; 
        private TransformGroup transformGroup;
        private RotateTransform rotation;

        public SolidColorBrush fill;

        public Vector gravPosition { get; set; }
        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public bool IsTouched { get; set; }
        public bool affectedByGravity { get; set; }
        public bool runHandler { get; set; }

        private double _scale = 1;
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;

            }
        }

        private int _radius;
        private Point enterTouchPoint;
        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
            }
        }



        public IdeaBall(Vector Position, Vector Velocity, Canvas mainCanvas, int rad, Color c, CanvasController CC, String text)
        {
            random = new Random();
            fill = new SolidColorBrush()
            {
                Color = c
            };

            this.Velocity = Velocity;
            this.Position = Position;
            this.mainCanvas = mainCanvas;
            this.IsTouched = false;
            this.timerReset = 0;
            this.affectedByGravity = true;
            this.CanvasCtrl = CC;
            this.runHandler = true;
            this.text = text;
            this.transformGroup = new TransformGroup();
            this.rotation = new RotateTransform(0);
            this.transformGroup.Children.Add(this.rotation);


            Ellipse = new Ellipse()
            {
                Fill = fill,
            };


            Title = new TextBlock()
            {
                Text = text,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 20,
                IsHitTestVisible = false,
                TextAlignment = System.Windows.TextAlignment.Center
            };

            Radius = rad;

            timer.Elapsed += new ElapsedEventHandler(Ellipse_HoldGestureEvent);
            timer.Interval = 2000;

            pulseTimer.Elapsed += new ElapsedEventHandler(Ellipse_ClickedFeedbackEvent);
            pulseTimer.Interval = 500;

            Ellipse.IsManipulationEnabled = true;
            this.Ellipse.RenderTransform = this.transformGroup;

            Ellipse.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(Ellipse_TouchDown);
            Ellipse.TouchMove += new System.EventHandler<System.Windows.Input.TouchEventArgs>(Ellipse_TouchMove);
            Ellipse.TouchLeave += new System.EventHandler<System.Windows.Input.TouchEventArgs>(Ellipse_TouchLeave);
            Ellipse.AddHandler(TouchExtensions.TapGestureEvent, new RoutedEventHandler(Ellipse_TapGestureEvent));
            Ellipse.ManipulationStarting += this.TouchableThing_ManipulationStarting;
            Ellipse.ManipulationDelta += this.TouchableThing_ManipulationDelta;
            Ellipse.ManipulationInertiaStarting += this.TouchableThing_ManipulationInertiaStarting;
     }
 
        void TouchableThing_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = Ellipse;
        }
 
        void TouchableThing_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // the center never changes in this sample, although we always compute it.
            Point center = new Point(
                 this.Ellipse.RenderSize.Width / 2.0, this.Ellipse.RenderSize.Height / 2.0);

            // apply the rotation at the center of the rectangle if it has changed
            this.rotation.CenterX = center.X;
            this.rotation.CenterY = center.Y;
            this.rotation.Angle += e.DeltaManipulation.Rotation;

        }
       
        void TouchableThing_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.RotationBehavior = new InertiaRotationBehavior();
            e.RotationBehavior.InitialVelocity = e.InitialVelocities.AngularVelocity;
            // 720 degrees per second squared.
            e.RotationBehavior.DesiredDeceleration = 720 / (1000.0 * 1000.0);  
        }

        public bool Clicked { get; set; }

        protected virtual void Ellipse_TapGestureEvent(object sender, RoutedEventArgs e)
        {

            if (runHandler)
            {
                CanvasCtrl.votingInitiated(this);
            }
        }

        void Ellipse_HoldGestureEvent(object sender, ElapsedEventArgs e)
        {
            if (runHandler)
            {
                pulseTimer.Start();
                this.Radius = this.Radius + 5;

                if (this.affectedByGravity)
                {
                    this.gravPosition = this.Position;
                    this.affectedByGravity = false;
                    CanvasCtrl.addGravityPoints(this);
                }
                else
                {
                    this.affectedByGravity = true;
                    CanvasCtrl.removeGravityPoints(this);
                }
            }   
        }

        void Ellipse_ClickedFeedbackEvent(object sender, ElapsedEventArgs e)
        {
            this.Radius = this.Radius - 5;
            pulseTimer.Stop();
        }

        protected virtual void Ellipse_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (runHandler)
            {
                // If this contact is the one that was remembered
                if (e.TouchDevice == this.ellipseControlTouchDevice)
                {
                    // Forget about this contact.
                    ellipseControlTouchDevice = null;
                    if (this.affectedByGravity) this.Velocity = new Vector(deltaX * 3, deltaY * 3);
                    this.IsTouched = false;
                }
                releasePoint = e.TouchDevice.GetTouchPoint(this.mainCanvas).Position;
                double deltaTouchDownReleaseX = Math.Abs(releasePoint.X - enterTouchPoint.X);
                double deltaTouchDownReleaseY = Math.Abs(releasePoint.Y - enterTouchPoint.Y);
                if (deltaTouchDownReleaseX < 5 && deltaTouchDownReleaseY < 5)
                {
                    isPressingMovement = true;
                }
                else
                {
                    isPressingMovement = false;
                }

                timer.Stop();

                // Mark this event as handled.  
                // e.Handled = true;
            }
        }

        void Ellipse_TouchMove(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (runHandler)
            {
                if (e.TouchDevice == this.ellipseControlTouchDevice)
                {
                    if (timerReset == 10)
                    {
                        timer.Stop();
                        timer.Start();
                        timerReset = 0;
                    }

                    Ellipse x = sender as Ellipse;

                    timerReset++;

                    // Get the current position of the contact.  
                    Point currentTouchPoint = ellipseControlTouchDevice.GetCenterPosition(this.mainCanvas);

                    // Get the change between the controlling contact point and
                    // the changed contact point.  
                    deltaX = currentTouchPoint.X - lastPoint.X;
                    deltaY = currentTouchPoint.Y - lastPoint.Y;

                    // Get and then set a new top position and a new left position for the ellipse. 
                    this.Position = new Vector(this.Position.X + (int)deltaX, this.Position.Y + (int)deltaY);

                    // Forget the old contact point, and remember the new contact point.  
                    lastPoint = currentTouchPoint;

                    // Mark this event as handled.  
                    // e.Handled = true;
                    if (!this.affectedByGravity)
                    {
                        this.gravPosition = this.Position;
                    }
                }
            }
        }

        void Ellipse_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (runHandler)
            {
                /*
                this.Vx = 0;
                this.Vy = 0;
                */

                // Capture to the ellipse.  
                e.TouchDevice.Capture(sender as Ellipse);
                timer.Start();
                this.IsTouched = true;

                // Remember this contact if a contact has not been remembered already.  
                // This contact is then used to move the ellipse around.
                if (ellipseControlTouchDevice == null)
                {
                    ellipseControlTouchDevice = e.TouchDevice;

                    // Remember where this contact took place.  
                    lastPoint = ellipseControlTouchDevice.GetTouchPoint(this.mainCanvas).Position;
                }

                enterTouchPoint = ellipseControlTouchDevice.GetTouchPoint(this.mainCanvas).Position;

                // Mark this event as handled.  
                // e.Handled = true;
            }
        }





        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        public Ellipse Ellipse { get; private set; }
        public TextBlock Title { get; private set; }


        public void Draw()
        {
            Ellipse.Width = (Radius * 2) * Scale;
            Ellipse.Height = Ellipse.Width;
            Title.Width = Ellipse.Width;
            Canvas.SetLeft(Ellipse, this.Position.X - (Ellipse.Width/2));
            Canvas.SetTop(Ellipse, this.Position.Y - (Ellipse.Width/2));

            Canvas.SetLeft(Title, this.Position.X-Title.ActualWidth/2);
            Canvas.SetTop(Title, this.Position.Y-Title.ActualHeight/2);
        }

        public static bool Intersects(IdeaBall a, IdeaBall b)
        {
            return (a.Position.X + a.Radius + b.Radius > b.Position.X
                    && a.Position.X < b.Position.X + a.Radius + b.Radius
                    && a.Position.Y + a.Radius + b.Radius > b.Position.Y
                    && a.Position.Y < b.Position.Y + a.Radius + b.Radius);
        }

        private static bool Collides(IdeaBall a, IdeaBall b)
        {
            double dX = a.Position.X - b.Position.X;
            double dY = a.Position.Y - b.Position.Y;

            double sumRadius = a.Radius + b.Radius;
            double sqrRadius = sumRadius * sumRadius;

            double distSqr = (dX * dX) + (dY * dY);

            if (distSqr <= sqrRadius)
            {
                return true;
            }

            return false;
        }

        public void ResolveCollision(IdeaBall a, IdeaBall b)
        {
            // get the mtd
            //Vector deltaPosition = new Vector(a.Position.X - b.Position.X, a.Position.Y - b.Position.Y-0.00001);
            

                Vector deltaPosition = a.Position - b.Position;

                double deltaLength = deltaPosition.Length;

                if ((a.Radius + b.Radius) - deltaLength == 0)
                {
                    return;
                }
                // minimum translation distance to push balls apart after intersecting
                Vector mtd = deltaPosition * (((a.Radius + b.Radius) - deltaLength) / deltaLength);



                // resolve intersection --
                // inverse mass quantities
                double im1 = 1.0 / a.Radius;
                double im2 = 1.0 / b.Radius;

                // push-pull them apart based off their mass
                if (a.affectedByGravity) a.Position = a.Position + (mtd * (im1 / (im1 + im2))) + adjustment;
                if (b.affectedByGravity) b.Position = b.Position - (mtd * (im2 / (im1 + im2))) - adjustment;




                // impact speed
                Vector v = a.Velocity - b.Velocity;
                Vector mtdNormalized = new Vector(mtd.X, mtd.Y);
                mtdNormalized.Normalize();
                double vn = v * mtdNormalized;

                // sphere intersecting but moving away from each other already
                if (vn > 0.0f) return;

                // collision impulse
                if (vn >= -28.0f)
                {
                    return;
                }
                             
                double i = (-(1.0 + Restitution) * vn) / (im1 + im2);
                Vector impulse = mtdNormalized * (i);

                Vector aNewVelocity = a.Velocity + (impulse * (im1));
                Vector bNewVelocity = b.Velocity - (impulse * (im2));

                // change in momentum
                a.Velocity = aNewVelocity * 0.3;
                b.Velocity = bNewVelocity * 0.3;
        }

        internal void DetectCollisions(List<IdeaBall> items)
        {
            foreach (IdeaBall ball in items)
            {
                if (!this.Equals(ball))
                {
                    if (Intersects(this, ball) && Collides(this, ball))
                    {
                        ResolveCollision(this, ball);
                    };
                }
            }
        }

        internal void AttachTo(Canvas MainCanvas)
        {
            MainCanvas.Children.Add(this.Ellipse);
            MainCanvas.Children.Add(this.Title);
        }

        internal void ApplyScale(Vector scale, double length, Point center)
        {
            double translation =  length / 2;
            Scale = scale.X;
            if (center.X > Position.X) Position = new Vector(Position.X + translation, Position.Y);
            if (center.X < Position.X) Position = new Vector(Position.X - translation, Position.Y);
            if (center.Y > Position.Y) Position = new Vector(Position.X, Position.Y + translation);
            if (center.Y < Position.Y) Position = new Vector(Position.X, Position.Y - translation);
            
        }

        
    }
}
