using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace SurfaceApplication1
{
    class ButtonBall : IdeaBall
    {
        public ButtonBall(Vector Position, Vector Velocity, Canvas mainCanvas, int rad, Color c) : base(Position, Velocity, mainCanvas, rad, c)
        {
            Title.Text = "Add new Idea";
            Debug.WriteLine("Ball button created");
        }

        protected override void Ellipse_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Debug.WriteLine("Ball pressed1");
            if (base.deltaX < 2000 && deltaY < 2000)
            {
                Debug.WriteLine("Ball pressed2");
            }
        }
    }
}
