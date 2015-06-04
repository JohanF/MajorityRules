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
    public class QuestionBall : IdeaBall
    {
        private CanvasController CanvasCtrl;
        public QuestionBall(Vector Position, Vector Velocity, Canvas mainCanvas, int rad, Color c, CanvasController CC, String text)
            : base(Position, Velocity, mainCanvas, rad, c, CC, text)
        {
            Title.Text = "?";
            Title.FontSize = 32;
            Title.FontWeight = FontWeights.UltraBold;
            Debug.WriteLine("Ball button created");
            this.CanvasCtrl = CC;
        }

        protected override void Ellipse_TapGestureEvent(object sender, RoutedEventArgs e)
        {
           
        }

       
    }
}
