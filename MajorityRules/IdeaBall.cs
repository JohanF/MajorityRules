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

        public Vector Position { get; set; }
        public Vector Velocity { get; set; }

        private int _radius;
        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                Ellipse.Width = value * 2;
                Ellipse.Height = value * 2;
            }
        }



        public IdeaBall(Vector Position, Vector Velocity, Canvas mainCanvas)
        {
            SolidColorBrush fill = new SolidColorBrush()
            {
                Color = Colors.Green
            };

            this.Velocity = Velocity;
            this.Position = Position;

            Ellipse = new Ellipse()
            {
                Fill = fill,
            };
            Radius = InitRadius;
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
                double deltaX = currentTouchPoint.X - lastPoint.X;
                double deltaY = currentTouchPoint.Y - lastPoint.Y;

                // Get and then set a new top position and a new left position for the ellipse. 
                this.Position = new Vector(this.Position.X + (int)deltaX, this.Position.Y + (int)deltaY);
                this.Velocity = new Vector(deltaX, deltaY);

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
            Vector v = a.Velocity-b.Velocity;
            Vector mtdNormalized = new Vector(mtd.X, mtd.Y);
            mtdNormalized.Normalize();
            double vn = v*mtdNormalized;

            // sphere intersecting but moving away from each other already
            if (vn > 0.0f) return;

            // collision impulse
            double i = (-(1.0 + Restitution) * vn) / (im1 + im2);
            Vector impulse = mtdNormalized * (i);

            Vector aNewVelocity = a.Velocity + (impulse * (im1));
            Vector bNewVelocity = b.Velocity - (impulse * (im2));



            // change in momentum
            a.Velocity = aNewVelocity * 0.58;
            b.Velocity = bNewVelocity * 0.58;

        }

        internal void DetectCollisions(List<IdeaBall> items)
        {
            this.Ellipse.Fill = new SolidColorBrush()
            {
                Color = Colors.Green
            };
            foreach (IdeaBall ball in items)
            {
                if (!this.Equals(ball))
                {
                    if (Intersects(this, ball) && Collides(this, ball))
                    {
                        //this.Ellipse.Fill = new SolidColorBrush()
                        {
                            //Color = Colors.Red
                        };
                        ResolveCollision(this, ball);
                    };
                }
            }
        }
        /*
	    public static Vector VectorNewPlane(Vector velocity, double rotation) {
		    //alfa = the x-vectors angle towards the new plane
		    //beta = the y-vectors angle towards the new plane
		    double alfa = Math.PI/2 - rotation;
		    double beta = - rotation;
		
		    //the x- and y-vectors for the new plane
		    Vector newVelocity = new Vector();
		    newVelocity.X = (float) (velocity.Y * Math.Cos(alfa) + velocity.X * Math.Cos(beta));
		    newVelocity.Y = (float) (velocity.Y * Math.Sin(alfa) + velocity.X * Math.Sin(beta));
		    return newVelocity;
	    }

        /*
        public static void HandleCollision(Vector v1, Vector v2, double weight1, double weight2)
        {
		    /*
		     * v1 = ( (m1 - m2)*u1 + 2*m2*u2 ) / (m1 + m2)
		     * v2 = ( (m2 - m1)*u2 + 2*m1*u1 ) / (m1 + m2)
                     
            double momentum = v1.X * weight1 + v2.X * weight2;
            double kinetic = v1.X * v1.X * weight1 / 2 + v2.X * v2.X * weight2 / 2;
            Vector tmp = new Vector();
            tmp.X = ((weight1 - weight2) * v1.X + 2 * weight2 * v2.X) / (weight1 + weight2);
            v2.X = ((weight2 - weight1) * v2.X + 2 * weight1 * v1.X) / (weight1 + weight2);
            v1.X = tmp.X;
        
		
	    }
        */
        /*
        private static void CalculateNewVelocities(IdeaBall a, IdeaBall b, float distance)
        {
            Vector vecA = new Vector(a.X, a.Y);
            Vector vecB = new Vector(b.X, b.Y);

            double rotation = Math.Atan((vecA.Y - vecB.Y) / (vecA.X - vecB.X));
            if (vecB.X > vecA.X)
            {
                rotation = Math.PI + rotation;
            }

            Vector oldVelocityA = new Vector(a.Vx, a.Vy);
            Vector oldVelocityB = new Vector(b.Vx, b.Vy);
            Vector newVelocityA = VectorNewPlane(oldVelocityA, rotation);
            Vector newVelocityB = VectorNewPlane(oldVelocityB, rotation);

            //TODO: fix radius
            double weightA = 1;
            double weightB = 1;


            // 3) handle collision (only x-velocity)
            HandleCollision(newVelocityA, newVelocityB, weightA, weightB);

            // 4) convert the new velocity back to the normal plane system
            Vector tmp = VectorNewPlane(newVelocityA, -rotation);
            oldVelocityA.X = tmp.X;
            oldVelocityA.Y = tmp.Y;
            tmp = VectorNewPlane(newVelocityB, -rotation);
            oldVelocityB.X = tmp.X;
            oldVelocityB.Y = tmp.Y;

            a.Vx = (float)oldVelocityA.X;
            a.Vy = (float)oldVelocityA.Y;

            b.Vx = (float)oldVelocityB.X;
            b.Vy = (float)oldVelocityB.Y;

            // 5) update balls position
            double collision = (a.Radius + b.Radius) - distance/2;
            
            a.X += Convert.ToInt16(collision * Math.Cos(rotation));
            a.Y += Convert.ToInt16(collision * Math.Sin(rotation));

            b.X += Convert.ToInt16(-collision * Math.Cos(rotation));
            b.Y += Convert.ToInt16(-collision * Math.Sin(rotation));
           
            /*
            int mass1 = a.Radius;
            int mass2 = b.Radius;
            float velX1 = a.Vx;
            float velX2 = b.Vx;
            float velY1 = a.Vy;
            float velY2 = b.Vy;

            float newVelX1 = (velX1 * (mass1 - mass2) + (2 * mass2 * velX2)) / (mass1 + mass2);
            float newVelX2 = (velX2 * (mass2 - mass1) + (2 * mass1 * velX1)) / (mass1 + mass2);
            float newVelY1 = (velY1 * (mass1 - mass2) + (2 * mass2 * velY2)) / (mass1 + mass2);
            float newVelY2 = (velY2 * (mass2 - mass1) + (2 * mass1 * velY1)) / (mass1 + mass2);

            a.Vx = newVelX1;
            b.Vx = newVelX2;
            a.Vy = newVelY1;
            b.Vy = newVelY2;

            a.X = a.X + Convert.ToInt16(newVelX1);
            a.Y = a.Y + Convert.ToInt16(newVelY1);
            b.X = b.X + Convert.ToInt16(newVelX2);
            b.Y = b.Y + Convert.ToInt16(newVelY2);
        }*/
        
    }
}
