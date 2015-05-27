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
            // Remember where this contact took place.  
            base.Ellipse_TouchLeave(sender, e);
            Debug.WriteLine("Ball pressed1");
            Debug.WriteLine(deltaY);
            Debug.WriteLine(deltaX);
            Debug.WriteLine(isPressingMovement);
            if (isPressingMovement)
            {
                Debug.WriteLine("Ball pressed2");
                //CanvasController.openVirtualKeyBoard();
            }
        }
    }
}
