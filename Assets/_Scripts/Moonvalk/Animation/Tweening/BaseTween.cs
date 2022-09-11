using System;
using System.Collections.Generic;
using Moonvalk.Accessory;
using Moonvalk.Utilities;

namespace Moonvalk.Animation
{
    /// <summary>
    /// Container representing a singular Tweening value.
    /// </summary>
    public abstract class BaseTween<T> : ITween
    {
        #region Data Fields
        /// <summary>
        /// A reference to the property value(s) that will be modified.
        /// </summary>
        protected Ref<T>[] _properties;

        /// <summary>
        /// The target value that will be reached.
        /// </summary>
        protected T[] _targetValues;

        /// <summary>
        /// The starting value.
        /// </summary>
        protected T[] _startValues;

        /// <summary>
        /// A timer used to delay Tweens before playing.
        /// </summary>
        protected MicroTimer _delayTimer;

        /// <summary>
        /// A duration in seconds that it will take for this Tween to elapse.
        /// </summary>
        protected float _duration = 1f;

        /// <summary>
        /// The percentage currently elapsed from 0f to 1f.
        /// </summary>
        protected float _percentage = 0f;

        /// <summary>
        /// An EasingFunction to be applied to this Tween.
        /// </summary>
        protected EasingFunction[] _easingFunctions;

        /// <summary>
        /// The current TweenState of this Tween object.
        /// </summary>
        protected TweenState _currentState = TweenState.Idle;

        /// <summary>
        /// A map of Actions that will occur while this Tween is in an active TweenState.
        /// </summary>
        protected Dictionary<TweenState, InitValue<List<Action>>> _functions;
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor for creating a new BaseTween.
        /// </summary>
        /// <param name="referenceValues_">Array of references to float values.</param>
        public BaseTween(params Ref<T>[] referenceValues_)
        {
            // Store reference to properties and build function maps.
            this._properties = referenceValues_;
            this._functions = new Dictionary<TweenState, InitValue<List<Action>>>();
            foreach (TweenState state in Enum.GetValues(typeof(TweenState)))
            {
                this._functions.Add(state, new InitValue<List<Action>>(() => { return new List<Action>(); }));
            }

            // Create new arrays for storing property start, end, and easing functions.
            this._startValues = new T[referenceValues_.Length];
            this._targetValues = new T[referenceValues_.Length];
            this._easingFunctions = new EasingFunction[referenceValues_.Length];
            for (int i = 0; i < this._easingFunctions.Length; i++)
            {
                this._easingFunctions[i] = Easing.Linear.None;
            }
        }
        #endregion

        #region Public Getters/Setters
        /// <summary>
        /// Gets the reference corresponding to the property value(s).
        /// </summary>
        /// <value>The function that returns reference to the properties being Tweened.</value>
        public Ref<T>[] Properties
        {
            get
            {
                return this._properties;
            }
        }

        /// <summary>
        /// Gets the current percentage.
        /// </summary>
        /// <value>The current percentage elapsed.</value>
        public float Percentage
        {
            get
            {
                return this._percentage;
            }
        }

        /// <summary>
        /// Gets the current state of this Tween.
        /// </summary>
        public TweenState State
        {
            get
            {
                return this._currentState;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts this Tween with the current settings.
        /// </summary>
        public BaseTween<T> Start()
        {
            this.updateStartValues();
            this._percentage = 0f;
            if (_delayTimer != null) {
                this._delayTimer.Start();
            }
            this._currentState = TweenState.Start;
            this.handleTasks(this._currentState);
            (Global.GetSystem<TweenSystem>() as TweenSystem).Add(this);
            return this;
        }

        /// <summary>
        /// Stops this Tween.
        /// </summary>
        public void Stop()
        {
            this._currentState = TweenState.Stopped;
        }

        /// <summary>
        /// Updates this Tween.
        /// </summary>
        /// <param name="deltaTime_">The duration of time between last and current game tick.</param>
        /// <returns>Returns true when this Tween is active and false when it is complete.</returns>
        public bool Update(float deltaTime_)
        {
            if (this._currentState == TweenState.Complete)
            {
                return false;
            }
            if (this._currentState == TweenState.Stopped || this._currentState == TweenState.Idle)
            {
                return true;
            }

            this._currentState = TweenState.Update;
            this.handleTasks(this._currentState);
            bool targetReached = this.animate(deltaTime_);
            if (targetReached)
            {
                this._currentState = TweenState.Complete;
                this.handleTasks(this._currentState);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the delay in seconds applied to this Tween.
        /// </summary>
        /// <param name="delaySeconds_">The delay duration in seconds.</param>
        /// <returns>Returns this Tween object.</returns>
        public BaseTween<T> Delay(float delaySeconds_)
        {
            if (this._delayTimer == null) {
                this._delayTimer = new MicroTimer();
            }
            this._delayTimer.Duration(delaySeconds_);
            return this;
        }
        
        /// <summary>
        /// Sets the duration in seconds that must elapse for this Tween to resolve.
        /// </summary>
        /// <param name="durationSeconds_">The duration of this Tween in seconds.</param>
        /// <returns>Returns this Tween object.</returns>
        public BaseTween<T> Duration(float durationSeconds_)
        {
            this._duration = durationSeconds_;
            return this;
        }

        /// <summary>
        /// Sets the target value(s) that this Tween will reach once complete.
        /// </summary>
        /// <param name="targetValues_">An array of target values for each property.</param>
        /// <returns>Returns this Tween object.</returns>
        public BaseTween<T> To(params T[] targetValues_)
        {
            for (int i = 0; i < targetValues_.Length; i++)
            {
                this._targetValues[i] = targetValues_[i];
            }
            return this;
        }

        /// <summary>
        /// Sets an Easing Function for each available property.
        /// </summary>
        /// <param name="functions_">An array of Easing Functions per property.</param>
        /// <returns>Returns this Tween object.</returns>
        public BaseTween<T> Ease(params EasingFunction[] functions_)
        {
            for (int i = 0; i < _easingFunctions.Length; i++)
            {
                EasingFunction nextFunc = (functions_.Length > i) ? functions_[i] : functions_[0];
                this._easingFunctions[i] = nextFunc;
            }
            return this;
        }

        /// <summary>
        /// Removes this MVTween on the following game tick.
        /// </summary>
        public void Delete()
        {
            this._currentState = TweenState.Complete;
        }

        /// <summary>
        /// Defines Actions that will occur when this Tween begins.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        public BaseTween<T> OnStart(params Action[] tasksToAdd_)
        {
            addTasks(TweenState.Start, tasksToAdd_);
            return this;
        }

        /// <summary>
        /// Defines Actions that will occur when this Tween updates.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        public BaseTween<T> OnUpdate(params Action[] tasksToAdd_)
        {
            addTasks(TweenState.Update, tasksToAdd_);
            return this;
        }

        /// <summary>
        /// Defines Actions that will occur once this Tween has completed.
        /// </summary>
        /// <param name="tasksToAdd_">Array of Actions to add.</param>
        /// <returns>Returns this Tween object.</returns>
        public BaseTween<T> OnComplete(params Action[] tasksToAdd_)
        {
            addTasks(TweenState.Complete, tasksToAdd_);
            return this;
        }

        /// <summary>
        /// Adds a Tween that will begin following the original MVTween's completion.
        /// </summary>
        /// <param name="triggeringTween_">The original MVTween.</param>
        /// <param name="tweenToFollow_">The MVTween to start on completion./param>
        /// <returns>Returns this Tween object.</returns>
        public static BaseTween<T> operator +(BaseTween<T> triggeringTween_, BaseTween<T> tweenToFollow_)
        {
            return triggeringTween_.OnComplete(() => { tweenToFollow_.Start(); });
        }

        /// <summary>
        /// Adds an Action that will begin following this MVTween's completion.
        /// </summary>
        /// <param name="triggeringTween_">The original MVTween.</param>
        /// <param name="taskForCompletion_">The Action to run on completion.</param>
        /// <returns>Returns this MVTween object.</returns>
        public static BaseTween<T> operator +(BaseTween<T> triggeringTween_, Action taskForCompletion_)
        {
            return triggeringTween_.OnComplete(taskForCompletion_);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method used to update all properties available to this object.
        /// </summary>
        protected abstract void updateProperties();

        /// <summary>
        /// Animates this Tween from start to target.
        /// </summary>
        /// <param name="deltaTime_">The delta between last and current game tick.</param>
        /// <returns>Returns true when complete or false when actively animating.</returns>
        protected bool animate(float deltaTime_)
        {
            bool isComplete = false;

            // Complete delay before animating.
            if (this._delayTimer != null && !this._delayTimer.IsComplete)
            {
                return isComplete;
            }

            // Begin animating by progressing percentage.
            float newPercentage = (this._percentage + (deltaTime_ / this._duration));
            if (newPercentage >= 1f)
            {
                this._percentage = 1f;
                isComplete = true;
            }
            else
            {
                this._percentage = newPercentage;
            }
            this.updateProperties();
            return isComplete;
        }

        /// <summary>
        /// Adds an array of new Actions to a MVTweenState.
        /// </summary>
        /// <param name="state_">The MVTweenState to add tasks for.</param>
        /// <param name="tasksToAdd_">The tasks to add.</param>
        protected void addTasks(TweenState state_, params Action[] tasksToAdd_)
        {
            foreach (Action task in tasksToAdd_)
            {
                _functions[state_].Value.Add(task);
            }
        }

        /// <summary>
        /// Updates all starting values set the reference property values.
        /// </summary>
        protected void updateStartValues()
        {
            for (int i = 0; i < this._properties.Length; i++)
            {
                this._startValues[i] = this._properties[i]();
            }
        }

        /// <summary>
        /// Handles all tasks for the specified TweenState.
        /// </summary>
        /// <param name="state_">The state to run tasks for.</param>
        protected void handleTasks(TweenState state_)
        {
            foreach (Action action in _functions[state_].Value)
            {
                action();
            }
        }
        #endregion
    }
}
