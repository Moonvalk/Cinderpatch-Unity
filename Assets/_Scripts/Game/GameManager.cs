using UnityEngine;
using Moonvalk.Animation;
using Moonvalk.Utilities;
using Moonvalk;

namespace Game
{
    /// <summary>
    /// Main game manager behavior.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Data Fields
        /// <summary>
        /// Reference to the main player.
        /// </summary>
        public PlayerController Player;

        /// <summary>
        /// Singleton instance of GameManager.
        /// </summary>
        protected static GameManager _instance;
        #endregion

        #region Public Getters/Setters
        /// <summary>
        /// Gets the GameManager Instance.
        /// </summary>
        /// <value>Returns the GameManager Singleton.</value>
        public static GameManager Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region Unity Events
        /// <summary>
        /// Unity Event - Called when this GameObject is first created.
        /// </summary>
        private void Awake()
        {
            this.initialize();
            this.registerSystems();
        }

        /// <summary>
        /// Unity Event - Called each game tick.
        /// </summary>
        private void Update()
        {
            Global.Systems.Update(Time.deltaTime);
        }
        #endregion

        /// <summary>
        /// Initializes this Component as a singleton which shall persist through Scenes.
        /// </summary>
        protected void initialize()
        {
            if (!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(this);
                return;
            }
            Destroy(this);
        }

        /// <summary>
        /// Registers all MVSystems that will be executed by this manager per game tick.
        /// </summary>
        protected void registerSystems()
        {
            new TweenSystem();
            new TimerSystem();
            new SpringSystem();
        }
    }
}
