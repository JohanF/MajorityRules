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
        private bool voteBool;

        public TextBlock Title;

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

        

        public VoteBall(Vector p, CanvasController CC, Color c, int R, bool yn, bool vB, string text)
        {
            fill = new SolidColorBrush()
            {
                Color = c
            };

            Title = new TextBlock()
            {
                Text = text,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 20,
                TextAlignment = System.Windows.TextAlignment.Center,
                Foreground = Brushes.Black
            };

            this.position = p;
            this.CanvasCtrl = CC;
            this._radius = R;
            this.yes = yn;
            this.voteBool = vB;
            this.selectedBall = null;
            this.ballClicked = false;
            this.safeZoneT = 0;
            this.Title.Visibility = Visibility.Hidden;

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
                if (voteBool)
                {
                    if (yes)
                    {
                        selectedBall.Radius += 3;
                        safeZoneT = 3;
                        CanvasCtrl.voteGotClicked(selectedBall, safeZoneT);

                    }
                    else
                    {
                        selectedBall.Radius -= 3;
                        safeZoneT = -3;
                        CanvasCtrl.voteGotClicked(selectedBall, safeZoneT);
                    }
                }
                else
                {
                    CanvasCtrl.voteGotCancelled(selectedBall);
                }

            }
        }

        public void Draw()
        {
            Ellipse.Width = (Radius * 2);
            Ellipse.Height = Ellipse.Width;
            Canvas.SetLeft(Ellipse, this.position.X - (Ellipse.Width / 2));
            Canvas.SetTop(Ellipse, this.position.Y - (Ellipse.Width / 2));

            Title.Height = Ellipse.Width;
            Title.Width = Ellipse.Width;

            Canvas.SetLeft(Title, this.position.X - Title.ActualWidth / 2);
            Canvas.SetTop(Title, this.position.Y - Title.ActualHeight / 3.5);
            
        }

        internal void AttachTo(Canvas MainCanvas)
        {
            MainCanvas.Children.Add(this.Ellipse);
            MainCanvas.Children.Add(this.Title);
        }
    }
}