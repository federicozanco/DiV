using System.Windows.Input;

namespace DummyImageViewer
{
    /// <summary>
    /// MouseWheelGesture
    /// </summary>
    public class MouseWheelGesture : MouseGesture
    {
        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public WheelDirection Direction { get; set; }

        /// <summary>
        /// Gets up.
        /// </summary>
        /// <value>
        /// Up.
        /// </value>
        public static MouseWheelGesture Up { get { return new MouseWheelGesture { Direction = WheelDirection.Up }; } }

        /// <summary>
        /// Gets down.
        /// </summary>
        /// <value>
        /// Down.
        /// </value>
        public static MouseWheelGesture Down { get { return new MouseWheelGesture { Direction = WheelDirection.Down }; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseWheelGesture"/> class.
        /// </summary>
        public MouseWheelGesture()
            : base(MouseAction.WheelClick) { }

        public MouseWheelGesture(ModifierKeys modifiers)
            : base(MouseAction.WheelClick, modifiers)
        {
        }

        /// <summary>
        /// Determines whether <see cref="T:System.Windows.Input.MouseGesture" /> matches the input associated with the specified <see cref="T:System.Windows.Input.InputEventArgs" /> object.
        /// </summary>
        /// <param name="targetElement">The target.</param>
        /// <param name="inputEventArgs">The input event data to compare this gesture to.</param>
        /// <returns>
        /// true if the event data matches this <see cref="T:System.Windows.Input.MouseGesture" />; otherwise, false.
        /// </returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs))
                return false;

            if (!(inputEventArgs is MouseWheelEventArgs))
                return false;

            var args = (MouseWheelEventArgs)inputEventArgs;

            switch (Direction)
            {
                case WheelDirection.None:
                    return args.Delta == 0;
                case WheelDirection.Up:
                    return args.Delta > 0;
                case WheelDirection.Down:
                    return args.Delta < 0;
                default:
                    return false;
            }
        }

        /// <summary>
        /// WheelDirection
        /// </summary>
        public enum WheelDirection
        {
            /// <summary>
            /// The none
            /// </summary>
            None,

            /// <summary>
            /// Up
            /// </summary>
            Up,

            /// <summary>
            /// Down
            /// </summary>
            Down,
        }
    }
}
