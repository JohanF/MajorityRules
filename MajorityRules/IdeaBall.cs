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

namespace SurfaceApplication1
{
    class IdeaBall
    {
        private const float Friction = 0.6f;
        private const int InitRadius = 25;
        private const double Restitution = 1;
        private static Vector adjustment = new Vector(0.0001, 0.0001);
        private TouchDevice ellipseControlTouchDevice;
        private Canvas mainCanvas;
        private Point lastPoint;
        private double deltaX;
        private double deltaY;

        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public bool IsTouched { get; set; }

        private int _radius;
        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                Ellipse.Width = value * 2;
                Ellipse.Height = value * 2;
                Title.Width = value * 2;
            }
        }



        public IdeaBall(Vector Position, Vector Velocity, Canvas mainCanvas, int rad)
        {
            SolidColorBrush fill = new SolidColorBrush()
            {
                Color = Colors.Green
            };

            this.Velocity = Velocity;
            this.Position = Position;
            this.mainCanvas = mainCanvas;
            this.IsTouched = false;
            this.UseGravity = true;


            Ellipse = new Ellipse()
            {
                Fill = fill,
            };

            Title = new TextBlock()
            {
                Text = "hej",
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 20,
                IsHitTestVisible = false,
                TextAlignment = System.Windows.TextAlignment.Center
            };

            Radius = rad;

            Ellipse.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(Ellipse_TouchDown);
            Ellipse.TouchMove += new System.EventHandler<System.Windows.Input.TouchEventArgs>(Ellipse_TouchMove);
            Ellipse.TouchLeave += new System.EventHandler<System.Windows.Input.TouchEventArgs>(Ellipse_TouchLeave);
        }

        void Ellipse_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        {
            // If this contact is the one that was remembered  
            if (e.TouchDevice == ellipseControlTouchDevice)
            {
                // Forget about this contact.
                ellipseControlTouchDevice = null;
                this.Velocity = new Vector(deltaX+3, deltaY+3);
                this.IsTouched = false;
            }

            // Mark this event as handled.  
            e.Handled = true;
        }

        void Ellipse_TouchMove(object sender, System.Windows.Input.TouchEventArgs e)
        {

            if (e.TouchDevice == ellipseControlTouchDevice)
            {
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
                e.Handled = true;
            }
        }

        void Ellipse_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            /*
            this.Vx = 0;
            this.Vy = 0;
            */

            // Capture to the ellipse.  
            e.TouchDevice.Capture(sender as Ellipse);

            this.IsTouched = true;

            // Remember this contact if a contact has not been remembered already.  
            // This contact is then used to move the ellipse around.
            if (ellipseControlTouchDevice == null)
            {
                ellipseControlTouchDevice = e.TouchDevice;

                // Remember where this contact took place.  
                lastPoint = ellipseControlTouchDevice.GetTouchPoint(this.mainCanvas).Position;
            }

            // Mark this event as handled.  
            e.Handled = true;

        }





        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        public Ellipse Ellipse { get; private set; }
        public TextBlock Title { get; private set; }


        public void Draw()
        {
            Canvas.SetLeft(Ellipse, this.Position.X - Radius);
            Canvas.SetTop(Ellipse, this.Position.Y - Radius);

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
                a.Position = a.Position + (mtd * (im1 / (im1 + im2))) + adjustment;
                b.Position = b.Position - (mtd * (im2 / (im1 + im2))) - adjustment;




                // impact speed
                Vector v = a.Velocity - b.Velocity;
                Vector mtdNormalized = new Vector(mtd.X, mtd.Y);
                mtdNormalized.Normalize();
                double vn = v * mtdNormalized;

                // sphere intersecting but moving away from each other already
                if (vn > 0.0f) return;

                if (vn >= -10.0f)
                {
                    UseGravity = false;
                    return;
                }
                UseGravity = true;

            
                    
                    double i = (-(1.0 + Restitution) * vn) / (im1 + im2);
                    Vector impulse = mtdNormalized * (i);

                    Vector aNewVelocity = a.Velocity + (impulse * (im1));
                    Vector bNewVelocity = b.Velocity - (impulse * (im2));

                    // change in momentum
                    a.Velocity = aNewVelocity * 0.75;
                    b.Velocity = bNewVelocity * 0.75;
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

        internal void ApplyScale(Vector vector, double p, Point point)
        {
            throw new NotImplementedException();
        }

        public bool UseGravity { get; set; }
    }
}
