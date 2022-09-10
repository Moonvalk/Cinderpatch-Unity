
namespace Moonvalk.Utilities
{
    /// <summary>
    /// A simple use timer object with basic functionality.
    /// </summary>
    public class MicroTimer : BaseTimer
    {
        #region Constructors
        /// <summary>
        /// Default constructor taking no additional properties.
        /// </summary>
        public MicroTimer() : base()
        {
        }

        /// <summary>
        /// Constructor that allows the user to set a duration.
        /// </summary>
        /// <param name="duration_">The duration in seconds that this timer will run for.</param>
        public MicroTimer(float duration_) : base(duration_)
        {
        }

        /// <summary>
        /// Constructor that allows the user to start the Timer immediately.
        /// </summary>
        /// <param name="duration_">The duration in seconds that this timer will run for.</param>
        /// <param name="start_">Set to true if this Timer should start immediately.</param>
        public MicroTimer(float duration_, bool start_) : base(duration_, start_)
        {
        }
        #endregion
    }
}