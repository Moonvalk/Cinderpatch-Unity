
namespace Moonvalk.Systems
{
    /// <summary>
    /// Contract for any MVSystem that will be run within the MVSystemManager.
    /// </summary>
    public interface IMVSystem
    {
        /// <summary>
        /// Execution method for each System.
        /// </summary>
        /// <param name="deltaTime_">The duration in time between last and current frame.</param>
        void Execute(float deltaTime_);

        void Clear();
    }
}
