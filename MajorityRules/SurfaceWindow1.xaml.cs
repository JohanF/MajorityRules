using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace SurfaceApplication1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        // Declare the global variables.
        TouchDevice ellipseControlTouchDevice;
        Point lastPoint;

        private void DragEllipse_TouchDown(object sender, TouchEventArgs e)
        {
            // Capture to the ellipse.  
            e.TouchDevice.Capture(sender as Ellipse);

            // Remember this contact if a contact has not been remembered already.  
            // This contact is then used to move the ellipse around.
            if (ellipseControlTouchDevice == null)
            {
                ellipseControlTouchDevice = e.TouchDevice;

                // Remember where this contact took place.  
                lastPoint = ellipseControlTouchDevice.GetTouchPoint(this.MainCanvas).Position;
            }

            // Mark this event as handled.  
            e.Handled = true;
        }

        private void DragEllipse_TouchMove(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice == ellipseControlTouchDevice)
            {
                // Get the current position of the contact.  
                Point currentTouchPoint = ellipseControlTouchDevice.GetCenterPosition(this.MainCanvas);

                // Get the change between the controlling contact point and
                // the changed contact point.  
                double deltaX = currentTouchPoint.X - lastPoint.X;
                double deltaY = currentTouchPoint.Y - lastPoint.Y;

                // Get and then set a new top position and a new left position for the ellipse.  
                double newTop = Canvas.GetTop(sender as Ellipse) + deltaY;
                double newLeft = Canvas.GetLeft(sender as Ellipse) + deltaX;

                Canvas.SetTop(sender as Ellipse, newTop);
                Canvas.SetLeft(sender as Ellipse, newLeft);

                // Forget the old contact point, and remember the new contact point.  
                lastPoint = currentTouchPoint;

                // Mark this event as handled.  
                e.Handled = true;
            }
        }

        private void DragEllipse_TouchLeave(object sender, TouchEventArgs e)
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
    }
}