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
using TinyMessenger;
using System.Diagnostics;

namespace SurfaceApplication1
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {

        public int Test { get; set; }
        public static TinyMessengerHub messageHub = new TinyMessengerHub();

        private CanvasController canvasController;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            canvasController = new CanvasController(MainCanvas, (int)MainWindow.Width, (int)MainWindow.Height, DebugText);

            IdeaInput.Visibility = System.Windows.Visibility.Hidden;
            IdeaInput.KeyUp += new KeyEventHandler(IdeaInput_KeyUp);

            messageHub.Subscribe<NewIdeaEvent>((m) =>
            {
                Debug.WriteLine("Received");
                toggleTextBoxHide();
            }
            );

            messageHub.Subscribe<DismissTextboxEvent>((m) =>
            {
                Debug.WriteLine("Dismiss textbox");
                IdeaInput.Visibility = Visibility.Hidden;
            }
            );
        }

        void IdeaInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && IdeaInput.Text != "" && IdeaInput.Text != null)
            {
                canvasController.AddBall(IdeaInput.Text);
                Keyboard.ClearFocus();
                IdeaInput.Text = "Enter new idea";
                IdeaInput.Visibility = Visibility.Hidden;
                BackgroundBall.Visibility = Visibility.Hidden;
            }
            else if (e.Key == Key.Escape)
            {
                canvasController.enableNonFocusedBallsButtonBall();
                Keyboard.ClearFocus();
                IdeaInput.Text = "Enter new idea";
                IdeaInput.Visibility = Visibility.Hidden;
                BackgroundBall.Visibility = Visibility.Hidden;
            }
        }

        private void toggleTextBoxHide()
        {
            BackgroundBall.Visibility = System.Windows.Visibility.Visible;
            IdeaInput.Visibility = System.Windows.Visibility.Visible;
            Debug.WriteLine("Toggle hide");
            //if (!IdeaInput.IsKeyboardFocused)
            //{
            //    Keyboard.Focus(IdeaInput);
            //}
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

        private void IdeaInput_GotFocus(object sender, RoutedEventArgs e)
        {
            IdeaInput.Text = "";
            IdeaInput.Foreground = new SolidColorBrush(Color.FromRgb(0,0,0));
        }
        private void IdeaInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IdeaInput.Text.Equals(""))
            {
                IdeaInput.Text = "Enter new idea";
            }
            IdeaInput.Foreground = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}