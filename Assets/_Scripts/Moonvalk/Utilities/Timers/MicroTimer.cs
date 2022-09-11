using System;
using System.Collections.Generic;

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
        /// </summary>S
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
        /// Constructor that allows the user to set completion tasks.
        /// </summary>
        /// <param name="onCompleteTasks_">Tasks to run on completion.</param>
        public MicroTimer(params Action[] onCompleteTasks_) : base(onCompleteTasks_)
        {
        }

        /// <summary>
        /// Constructor that allows the user to set a duration and completion tasks.
        /// </summary>
        /// <param name="duration_">The duration in seconds that this timer will run for.</param>
        /// <param name="onCompleteTasks_">Tasks to run on completion.</param>
        public MicroTimer(float duration_, params Action[] onCompleteTasks_) : base(duration_, onCompleteTasks_)
        {
        }
        #endregion
    }
}