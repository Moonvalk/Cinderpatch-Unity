
namespace Moonvalk.Systems
{
    /// <summary>
    /// Contract for an update-able MVSystem object.
    /// </summary>
    public interface IQueueItem
    {
        /// <summary>
        /// Updates this object.
        /// </summary>
        /// <param name="deltaTime_">The duration of time between last and current game tick.</param>
        /// <returns>Returns true when this object is active and false when it is complete.</returns>
        public bool Update(float deltaTime_);
    }
}