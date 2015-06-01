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
    public class ButtonBall : IdeaBall
    {
        private CanvasController CanvasCtrl;
        public ButtonBall(Vector Position, Vector Velocity, Canvas mainCanvas, int rad, Color c, CanvasController CC, String text) : base(Position, Velocity, mainCanvas, rad, c, CC, text)
        {
            Title.Text = "Add new Idea";
            Debug.WriteLine("Ball button created");
            this.CanvasCtrl = CC;
        }

        protected override void Ellipse_TapGestureEvent(object sender, RoutedEventArgs e)
        {
            if (runHandler && Clicked == false)
            {

                CanvasCtrl.disableNonFocusedBalls(this);
                Clicked = true;
                Debug.WriteLine("Ball pressed2");
                SurfaceWindow1.messageHub.Publish(new NewIdeaEvent());
            }
        }

        //protected override void Ellipse_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        //{
        //    // Remember where this contact took place.  
        //    base.Ellipse_TouchLeave(sender, e);
        //    Debug.WriteLine("Ball pressed1");
        //    Debug.WriteLine(deltaY);
        //    Debug.WriteLine(deltaX);
        //    Debug.WriteLine(isPressingMovement);
        //    if (isPressingMovement)
        //    {
                
        //    }
        //}

       
    }
}
