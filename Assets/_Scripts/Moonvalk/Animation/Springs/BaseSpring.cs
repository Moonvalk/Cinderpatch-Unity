
namespace Moonvalk.Animation
{
    public class BaseSpring<T> : ISpring
    {
        #region Public Methods
        /// <summary>
        /// Starts this Tween with the current settings.
        /// </summary>
        public BaseSpring<T> Start()
        {
            (Global.GetSystem<SpringSystem>() as SpringSystem).Add(this);
            return this;
        }

        /// <summary>
        /// Updates this Spring.
        /// </summary>
        /// <param name="deltaTime_">The duration of time between last and current game tick.</param>
        /// <returns>Returns true when this Tween is active and false when it is complete.</returns>
        public bool Update(float deltaTime_)
        {
            
            return true;
        }
        #endregion
    }
}
