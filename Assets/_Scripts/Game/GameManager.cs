using UnityEngine;
using UnityEngine.Audio;
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
        /// Singleton instance of GameManager.
        /// </summary>
        protected static GameManager _instance;

        /// <summary>
        /// Main audio mixer.
        /// </summary>
        public AudioMixer Mixer;
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

            this.Mixer.SetFloat("MusicVolume", Mathf.Log10(Global.MusicVolume) * 20f);
            this.Mixer.SetFloat("SoundVolume", Mathf.Log10(Global.SoundVolume) * 20f);
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
