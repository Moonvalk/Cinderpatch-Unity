using Moonvalk.Utilities.Algorithms;
using Moonvalk.Accessory;
using UnityEngine;

namespace Moonvalk.Animation
{
    public class Spring : BaseSpring<float>
    {
        /// <summary>
        /// Constructor for creating a new Tween.
        /// </summary>
        /// <param name="referenceValues_">Array of references to float values.</param>
        public Spring(params Ref<float>[] referenceValues_) : base(referenceValues_)
        {
        }

        /// <summary>
        /// Calculates the necessary velocities to be applied to all Spring properties each game tick.
        /// </summary>
        protected override void calculateForces()
        {
            for (int i = 0; i < this._properties.Length; i++)
            {
                float displacement = (this._targetProperties[i] - this._properties[i]());
                this._currentForce[i] = MotionAlgorithms.SimpleHarmonicMotion(this._tension, displacement, this._dampening, this._speed[i]);
            }
        }

        /// <summary>
        /// Applies force to properties each frame.
        /// </summary>
        /// <param name="deltaTime_">The time elapsed between last and current game tick.</param>
        protected override void applyForces(float deltaTime_)
        {
            for (int i = 0; i < this._properties.Length; i++)
            {
                this._speed[i] += this._currentForce[i] * deltaTime_;
                this._properties[i]() += this._speed[i] * deltaTime_;
            }
        }

        /// <summary>
        /// Determines if the minimum forces have been met to continue calculating Spring forces.
        /// </summary>
        /// <returns>Returns true if the minimum forces have been met.</returns>
        protected override bool minimumForcesMet()
        {
            for (int index = 0; index < _currentForce.Length; index++)
            {
                if (Mathf.Abs(this._currentForce[index]) >= this._minimumForce || Mathf.Abs(this._speed[index]) >= this._minimumForce)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if there is a need to apply force to this Spring to meet target values.
        /// </summary>
        /// <returns>Returns true if forces need to be applied</returns>
        protected override bool needToApplyForce()
        {
            for (int index = 0; index < this._properties.Length; index++)
            {
                if (this._properties[index]() != this._targetProperties[index])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
