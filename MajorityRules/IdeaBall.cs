using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Surface.Presentation;
using System.Windows.Media;
using System.Windows;

namespace SurfaceApplication1
{
    class IdeaBall
    {
        const float Friction = 0.6f;
        const int InitRadius = 10;

        public int X { get; set; }
        public int Y { get; set; }

        public float Vx { get; set; }
        public float Vy { get; set; }

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



        public IdeaBall(int speed)
        {
            SolidColorBrush fill = new SolidColorBrush()
            {
                Color = Colors.Green
            };




            Vx = speed;
            Vy = speed;

            Ellipse = new Ellipse()
            {
                Fill = fill,
            };
            Radius = InitRadius;
        }





        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        public Ellipse Ellipse { get; private set; }
        public TextBlock Title { get; private set; }


        public void Draw()
        {
            Canvas.SetLeft(Ellipse, X - Radius);
            Canvas.SetTop(Ellipse, Y - Radius);
        }

        public bool intersects(IdeaBall ball)
        {
            return (X + Radius + ball.Radius > ball.X
                    && X < ball.X + Radius + ball.Radius
                    && Y + Radius + ball.Radius > ball.Y
                    && Y < ball.Y + Radius + ball.Radius);
        }

        private static float DistanceTo(IdeaBall a, IdeaBall b)
        {
            float distance = (float)Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
            if (distance < 0) { distance = distance * -1; }
            return distance;
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
                    float distance = IdeaBall.DistanceTo(this, ball);
                    if (this.intersects(ball) && (distance < Radius + ball.Radius))
                    {
                        this.Ellipse.Fill = new SolidColorBrush()
                        {
                            Color = Colors.Red
                        };
                        CalculateNewVelocities(this, ball, distance);
                    };
                }
            }
        }

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

        public static void HandleCollision(Vector v1, Vector v2, double weight1, double weight2)
        {
		    /*
		     * v1 = ( (m1 - m2)*u1 + 2*m2*u2 ) / (m1 + m2)
		     * v2 = ( (m2 - m1)*u2 + 2*m1*u1 ) / (m1 + m2)
                     */
            double momentum = v1.X * weight1 + v2.X * weight2;
            double kinetic = v1.X * v1.X * weight1 / 2 + v2.X * v2.X * weight2 / 2;
            Vector tmp = new Vector();
            tmp.X = ((weight1 - weight2) * v1.X + 2 * weight2 * v2.X) / (weight1 + weight2);
            v2.X = ((weight2 - weight1) * v2.X + 2 * weight1 * v1.X) / (weight1 + weight2);
            v1.X = tmp.X;
        
		
	    }

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
            b.Y = b.Y + Convert.ToInt16(newVelY2);*/
        }
    }
}
