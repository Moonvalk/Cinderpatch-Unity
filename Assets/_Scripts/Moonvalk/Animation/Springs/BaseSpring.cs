using System;
using System.Collections.Generic;
using Moonvalk.Accessory;

namespace Moonvalk.Animation
{
    public abstract class BaseSpring<T> : ISpring
    {
        #region Data Fields
        /// <summary>
        /// A reference to the property value(s) that will be modified.
        /// </summary>
        protected Ref<T>[] _properties;

        /// <summary>
        /// The target value that will be reached.
        /// </summary>
        protected T[] _targetProperties;

        /// <summary>
        /// The tension value applied to this spring.
        /// </summary>
        protected float _tension = 0.5f;

        /// <summary>
        /// The dampening value applied to this spring.
        /// </summary>
        protected float _dampening = 0.05f;

        /// <summary>
        /// The current speed applied to this spring.
        /// </summary>
        protected T[] _speed;

        /// <summary>
        /// The amount of force to be applied each frame.
        /// </summary>
        protected T[] _currentForce;

        /// <summary>
        /// The minimum force applied to a Spring before it is no longer updated until settings change.
        /// </summary>
        protected float _minimumForce = 0.005f;

        /// <summary>
        /// The current SpringState of this Spring object.
        /// </summary>
        protected SpringState _currentState = SpringState.Stopped;

        /// <summary>
        /// A map of Actions that will occur while this Tween is in an active SpringState.
        /// </summary>
        protected Dictionary<SpringState, InitValue<List<Action>>> _functions;
        #endregion

        #region Public Getters/Setters
        /// <summary>
        /// Gets the reference corresponding to the property value(s).
        /// </summary>
        /// <value>The function that returns reference to the properties being modified.</value>
        public Ref<T>[] Properties
        {
            get
            {
                return this._properties;
            }
        }

        /// <summary>
        /// Gets the current force(s) applied to this Spring.
        /// </summary>
        public T[] Force
        {
            get
            {
                return this._currentForce;
            }
        }

        /// <summary>
        /// Gets the current state of this Spring.
        /// </summary>
        public SpringState State
        {
            get
            {
                return this._currentState;
            }
        }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor for creating a new BaseSpring.
        /// </summary>
        /// <param name="referenceValues_">Array of references to float values.</param>
        public BaseSpring(params Ref<T>[] referenceValues_)
        {
            // Store reference to properties and build function maps.
            this._properties = referenceValues_;
            this._functions = new Dictionary<SpringState, InitValue<List<Action>>>();
            foreach (SpringState state in Enum.GetValues(typeof(SpringState)))
            {
                this._functions.Add(state, new InitValue<List<Action>>(() => { return new List<Action>(); }));
            }

            // Create new array for storing property targets.
            this._targetProperties = new T[referenceValues_.Length];
            this._currentForce = new T[referenceValues_.Length];
            this._speed = new T[referenceValues_.Length];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates this Spring.
        /// </summary>
        /// <param name="deltaTime_">The duration of time between last and current game tick.</param>
        /// <returns>Returns true when this Spring is active and false when it is complete.</returns>
        public bool Update(float deltaTime_)
        {
            if (this._currentState == SpringState.Complete)
            {
                return false;
            }
            if (this._currentState == SpringState.Stopped)
            {
                return true;
            }
            this._currentState = SpringState.Update;
            this.handleTasks(this._currentState);

            // Update springs each frame until settled.
            this.calculateForces();
            this.applyForces(deltaTime_);
            if (!this.minimumForcesMet())
            {
                this.snapSpringToTarget();
                this._currentState = SpringState.Complete;
                this.handleTasks(this._currentState);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Starts this Spring with the current settings if there is a need to apply forces.
        /// </summary>
        public void Start()
        {
            if (this.needToApplyForce()) 
            {
                this._currentState = SpringState.Start;
                this.handleTasks(this._currentState);
                (Global.GetSystem<SpringSystem>() as SpringSystem).Add(this);
            }
        }

        /// <summary>
        /// Stops this Spring.
        /// </summary>
        public void Stop()
        {
            this._currentState = SpringState.Stopped;
        }

        public ISpring Dampening(float dampening_)
        {
            this._dampening = dampening_;
            return this;
        }

        public ISpring Tension(float tension_)
        {
            this._tension = tension_;
            return this;
        }

        public ISpring To(params T[] targetProperties_)
        {
            this._targetProperties = targetProperties_;
            this.Start();
            return this;
        }

        /// <summary>
        /// Removes this Spring on the following game tick.
        /// </summary>
        public void Delete()
        {
            this._currentState = SpringState.Complete;
        }

        /// <summary>
        /// Defines Actions that will occur when this Spring begins.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        public ISpring OnStart(params Action[] tasksToAdd_)
        {
            addTasks(SpringState.Start, tasksToAdd_);
            return this;
        }

        /// <summary>
        /// Defines Actions that will occur when this Spring updates.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        public ISpring OnUpdate(params Action[] tasksToAdd_)
        {
            addTasks(SpringState.Update, tasksToAdd_);
            return this;
        }

        /// <summary>
        /// Defines Actions that will occur once this Spring has completed.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        public ISpring OnComplete(params Action[] tasksToAdd_)
        {
            addTasks(SpringState.Complete, tasksToAdd_);
            return this;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates the necessary velocities to be applied to all Spring properties each game tick.
        /// </summary>
        protected abstract void calculateForces();

        /// <summary>
        /// Applies force to properties each frame.
        /// </summary>
        /// <param name="deltaTime_">The time elapsed between last and current game tick.</param>
        protected abstract void applyForces(float deltaTime_);

        /// <summary>
        /// Determines if the minimum forces have been met to continue calculating Spring forces.
        /// </summary>
        /// <returns>Returns true if the minimum forces have been met.</returns>
        protected abstract bool minimumForcesMet();

        /// <summary>
        /// Determines if there is a need to apply force to this Spring to meet target values.
        /// </summary>
        /// <returns>Returns true if forces need to be applied</returns>
        protected abstract bool needToApplyForce();

        /// <summary>
        /// Snaps all Spring properties directly to their target values. 
        /// </summary>
        protected void snapSpringToTarget()
        {
            for (int index = 0; index < this._properties.Length; index++)
            {
                this._properties[index]() = this._targetProperties[index];
            }
        }

        /// <summary>
        /// Adds an array of new Actions to a SpringState.
        /// </summary>
        /// <param name="state_">The SpringState to add tasks for.</param>
        /// <param name="tasksToAdd_">The tasks to add.</param>
        protected void addTasks(SpringState state_, params Action[] tasksToAdd_)
        {
            _functions[state_].Value.Clear();
            foreach (Action task in tasksToAdd_)
            {
                _functions[state_].Value.Add(task);
            }
        }

        /// <summary>
        /// Handles all tasks for the specified SpringState.
        /// </summary>
        /// <param name="state_">The state to run tasks for.</param>
        protected void handleTasks(SpringState state_)
        {
            for (int index = 0; index < _functions[state_].Value.Count; index++)
            {
                this._functions[state_].Value[index]();
            }
        }
        #endregion
    }
}
