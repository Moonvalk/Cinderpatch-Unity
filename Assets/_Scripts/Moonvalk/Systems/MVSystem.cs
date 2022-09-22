
namespace Moonvalk.Systems
{
    /// <summary>
    /// Abstract contract for any MVSystem to follow that will be managed by the MVSystemManager.
    /// </summary>
    public abstract class MVSystem<T> : IMVSystem
    {
        #region Constructors
        /// <summary>
        /// Constructs a new MVSystem with the default identity.
        /// </summary>
        public MVSystem()
        {
            this.initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes this new System.
        /// </summary>
        protected void initialize()
        {
            registerSelf();
        }

        /// <summary>
        /// Registers this System within the MVSystemManager.
        /// </summary>
        protected void registerSelf()
        {
            Global.Systems.RegisterSystem(this);
        }

        /// <summary>
        /// Execution method for each System.
        /// </summary>
        /// <param name="delta_">The duration in time between last and current frame.</param>
        public abstract void Execute(float delta_);

        public abstract void Clear();
        #endregion
    }
}
