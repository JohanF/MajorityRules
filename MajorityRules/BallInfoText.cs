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
    class BallInfoText
    {

        public Vector position;
        private CanvasController CanvasCtrl;
        private int _heigth;
        private int _width;
        public Grid grid { get; private set; }
        private Rectangle rectangle; 
        public TextBlock textBlock { get; private set; }

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
            }
        }
        public int Heigth
        {
            get { return _heigth; }
            set
            {
                _width = value;
            }
        }

        public BallInfoText(Vector p, CanvasController CC, int w, int h, string t)
        {
            
            this.position = p;
            this.CanvasCtrl = CC;
            this._width = w;
            this._heigth = h; 
            this.grid = new Grid();

            rectangle = new Rectangle()
            {

                Width = w,
                Height = h

            };

            textBlock = new TextBlock()
            {
                Text = t,
            };

            textBlock.Foreground = Brushes.White;

            grid.Children.Add(rectangle);
            grid.Children.Add(textBlock);

        }

        public void Draw()
        {
            grid.Width = _width; 
            grid.Height = _heigth;
            Canvas.SetLeft(grid, this.position.X - (grid.Width / 2));
            Canvas.SetTop(grid, this.position.Y - (grid.Height / 2));
            
        }

        internal void AttachTo(Canvas MainCanvas)
        {
            MainCanvas.Children.Add(this.grid);
        }
    }
}