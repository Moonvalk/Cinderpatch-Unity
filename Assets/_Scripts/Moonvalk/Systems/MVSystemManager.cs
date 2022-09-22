using System.Collections.Generic;

namespace Moonvalk.Systems
{
    /// <summary>
    /// A manager for handling all MVSystems.
    /// </summary>
    public class MVSystemManager
    {
        #region Data Fields
        /// <summary>
        /// A map that stores reference to all MVSystems.
        /// </summary>
        protected List<IMVSystem> _systemMap = new List<IMVSystem>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Update method that runs each MVSystem in order stored within _systemMap.
        /// </summary>
        /// <param name="delta_">The duration of time between last and current frame.</param>
        public void Update(float delta_)
        {
            foreach(IMVSystem system in _systemMap)
            {
                system.Execute(delta_);
            }
        }

        /// <summary>
        /// Registers a new MVSystem here.
        /// </summary>
        /// <param name="system_">The MVSystem object to be registered.</param>
        public void RegisterSystem(IMVSystem system_)
        {
            _systemMap.Add(system_);
        }

        /// <summary>
        /// Gets an MVSystem stored within this manager by type.
        /// </summary>
        /// <typeparam name="T">The type of the MVSystem to find.</typeparam>
        /// <returns>Returns the matching MVSystem of the type T, if possible.</returns>
        public IMVSystem Get<T>()
        {
            foreach (IMVSystem system in _systemMap)
            {
                if (system.GetType() == typeof(T))
                {
                    return system;
                }
            }
            return null;
        }

        public void ClearAllSystems()
        {
            foreach (IMVSystem system in _systemMap)
            {
                system.Clear();
            }
        }
        #endregion
    }
}
