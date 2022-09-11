using Moonvalk.Systems;
using System;

namespace Moonvalk.Animation
{
    /// <summary>
    /// Contract that a Spring object must fulfill.
    /// </summary>
    public interface ISpring : IQueueItem
    {
        ISpring Dampening(float dampening_);
        ISpring Tension(float tension_);

        /// <summary>
        /// Defines Actions that will occur when this Spring begins.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        ISpring OnStart(params Action[] tasksToAdd_);

        /// <summary>
        /// Defines Actions that will occur when this Spring updates.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        ISpring OnUpdate(params Action[] tasksToAdd_);

        /// <summary>
        /// Defines Actions that will occur once this Spring has completed.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        ISpring OnComplete(params Action[] tasksToAdd_);
    }
}