using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using TinyMessenger;

namespace SurfaceApplication1
{
    public class GravityBall : IdeaBall
    {
        private CanvasController CanvasCtrl;
        public GravityBall(Vector Position, Vector Velocity, Canvas mainCanvas, int rad, Color c, CanvasController CC, String text)
            : base(Position, Velocity, mainCanvas, rad, c, CC, text)
        {
            Title.Text = "G";
            this.CanvasCtrl = CC;
        }

        protected override void Ellipse_TapGestureEvent(object sender, RoutedEventArgs e)
        {

            this.pulseTimer.Start();
            this.Radius = this.Radius + 5;
            if (runHandler && Clicked == false)
            {

                CanvasCtrl.disableGravity();
                Clicked = true;
            }
            else if (runHandler && Clicked == true)
            {
                CanvasCtrl.enableGravity();
                Clicked = false;
            }
        }
    }
}
