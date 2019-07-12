using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace HDT.Wpf
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The <see cref="Window"/> this model controls
        /// </summary>
        private static Window mWindow;

        private WindowResizer mWindowResizer;

        private int mOuterMarginSize = 2;

        private int mOutlineBorderSize = 1;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Public Properties

        /// <summary>
        /// The active page of the application
        /// </summary>
        public ApplicaionPage ActivePage { get; set; } = ApplicaionPage.Home;

        public int WindowMinWidth { get; set; } = 350;

        public int WindowMinHeight { get; set; } = 300;

        public int ContentPaddingSize { get; set; } = 4;

        /// <summary>
        /// True if the window is currently being moved/dragged
        /// </summary>
        public bool BeingMoved { get; set; }

        /// <summary>
        /// True if we should have a dimmed overlay on the window
        /// such as when a popup is visible or the window is not focused
        /// </summary>
        public bool DimmableOverlayVisible { get; set; }

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless
        {
            get
            {
                Debug.Print(mDockPosition.ToString());
                Debug.Print(mWindow.WindowState.ToString());
                //Debug.Print(((mWindow.WindowState == WindowState.Maximized) || (mDockPosition != WindowDockPosition.Undocked)).ToString());
                return (mWindow.WindowState == WindowState.Maximized) || (mDockPosition != WindowDockPosition.Undocked);
            }
        }

        public int OutlineBorderSize
        {
            /* if the Window is maximized we want 0 outer margin*/
            get { return mOutlineBorderSize; }
            set { mOutlineBorderSize = value; }
        }

        /// <summary>
        /// The size of the resizable border around the window
        /// </summary>
        public int ResizeBorder { get { return Borderless ? 0 : 6; } }

        /// <summary>
        /// The height of the title bar / window caption
        /// </summary>
        public int TitleHeight { get; set; } = 30;

        /// <summary>
        /// The height of the bottom Status bar on the window
        /// </summary>
        public int StatusBarHeight { get; set; } = 22;

        public int OuterMarginSize
        {
            /* if the Window is maximized we want 0 outer margin*/
            get { return Borderless ? 0 : mOuterMarginSize; }
            set { mOuterMarginSize = value; }
        }

        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        public Thickness OuterMarginThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// Left and right thickness
        /// </summary>
        public Thickness InnerContentPadding { get { return new Thickness(ContentPaddingSize, 0, ContentPaddingSize, 0); } }

        public Thickness OutlineBorderThickness
        {
            get
            {
                switch (mDockPosition)
                {
                    case WindowDockPosition.Left:
                        return new Thickness(0, 0, OutlineBorderSize, 0);
                    case WindowDockPosition.Right:
                        return new Thickness(OutlineBorderSize, 0, 0, 0);
                    case WindowDockPosition.TopBottom:
                        return new Thickness(OutlineBorderSize, 0, OutlineBorderSize, 0);
                    case WindowDockPosition.Undocked:
                    default:
                        return new Thickness(OutlineBorderSize);
                }
            }
        }

        public GridLength TitleHeightGridLenght { get { return new GridLength(TitleHeight); } }

        public GridLength StatusbarHeightGridLenght { get { return new GridLength(StatusBarHeight); } }

        #endregion

        #region Commands

        /// <summary>
        /// To minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// To maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// To close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// To open the system menu
        /// Bound to the appplication icon on the title bar
        /// </summary>
        public ICommand SystemMenuCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public WindowViewModel(Window window)
        {
            mWindow = window;
                            
            mWindow.StateChanged += (sender, e) => WindowResized();

            // Creating the title bar commands
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            SystemMenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));

            // Call window resizer
            mWindowResizer = new WindowResizer(mWindow);

            mWindowResizer.WindowDockChanged += (dock) =>
            {
                mDockPosition = dock;

                WindowResized();
            };

            // On window being moved/dragged
            mWindowResizer.WindowStartedMove += () => { BeingMoved = true; };

            // Fix dropping an undocked window at top which should be positioned at the
            // very top of screen
            mWindowResizer.WindowFinishedMove += () =>
            {
                // Update being moved flag
                BeingMoved = false;

                // Check for moved to top of window and not at an edge...
                if (mDockPosition == WindowDockPosition.Undocked && mWindow.Top == mWindowResizer.CurrentScreenSize.Top)
                {
                    // if so, move it to the true top ( the border size )
                    mWindow.Top = -OuterMarginThickness.Top;
                }
            };
        }

        private void WindowResized()
        {
            // Events for all properties that are affected by a resize
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginThickness));
            OnPropertyChanged(nameof(OutlineBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
        }

        #endregion

        #region Helpers

        private Point GetMousePosition()
        {
            return mWindowResizer.GetCursorPosition();
        }

        #endregion
    }
}
