using System;
using System.Collections.Generic;
using Microsoft.Surface.Presentation.Input;
using System.Windows.Input;
using System.Timers;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SurfaceApplication1
{
    class VoteBall
    {

        public Vector position;
        private Color myColor;
        private CanvasController CanvasCtrl;
        private int _radius;
        public SolidColorBrush fill;
        private bool yes;
        public bool ballClicked;
        private int safeZoneT;

        public Ellipse Ellipse { get; private set; }
        public IdeaBall selectedBall { get; set; }
        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
            }
        }

        public VoteBall(Vector p, CanvasController CC, Color c, int R, bool yn)
        {
            fill = new SolidColorBrush()
            {
                Color = c
            };

            this.position = p;
            this.CanvasCtrl = CC;
            this._radius = R;
            this.yes = yn;
            this.selectedBall = null;
            this.ballClicked = false;
            this.safeZoneT = 0;

            Ellipse = new Ellipse()
            {
                Fill = fill,
            };

            Ellipse.AddHandler(TouchExtensions.TapGestureEvent, new RoutedEventHandler(Ellipse_TapGestureEvent));
        }

        void Ellipse_TapGestureEvent(object sender, RoutedEventArgs e)
        {
            if (ballClicked)
            {
                if (yes)
                {
                    selectedBall.Radius += 3;
                    safeZoneT = 3;

                }
                else
                {
                    selectedBall.Radius -= 3;
                    safeZoneT = -3;
                }

                CanvasCtrl.voteGotClicked(selectedBall, safeZoneT);
            }
        }

        public void Draw()
        {
            Ellipse.Width = (Radius * 2);
            Ellipse.Height = Ellipse.Width;
            Canvas.SetLeft(Ellipse, this.position.X - (Ellipse.Width / 2));
            Canvas.SetTop(Ellipse, this.position.Y - (Ellipse.Width / 2));
            
        }

        internal void AttachTo(Canvas MainCanvas)
        {
            MainCanvas.Children.Add(this.Ellipse);
        }
    }
}