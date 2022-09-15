using Moonvalk.Utilities.Algorithms;
using Moonvalk.Accessory;
using UnityEngine;

namespace Moonvalk.Animation
{
    public class SpringVec3 : BaseSpring<Vector3>
    {
        /// <summary>
        /// Constructor for creating a new Tween.
        /// </summary>
        /// <param name="referenceValues_">Array of references to Vector3 values.</param>
        public SpringVec3(params Ref<Vector3>[] referenceValues_) : base(referenceValues_)
        {
        }

        /// <summary>
        /// Calculates the necessary velocities to be applied to all Spring properties each game tick.
        /// </summary>
        protected override void calculateForces()
        {
            for (int i = 0; i < this._properties.Length; i++)
            {
                float displacement = (this._targetProperties[i].x - this._properties[i]().x);
                this._currentForce[i].x = MotionAlgorithms.SimpleHarmonicMotion(this._tension, displacement, this._dampening, this._speed[i].x);
                displacement = (this._targetProperties[i].y - this._properties[i]().y);
                this._currentForce[i].y = MotionAlgorithms.SimpleHarmonicMotion(this._tension, displacement, this._dampening, this._speed[i].y);
                displacement = (this._targetProperties[i].z - this._properties[i]().z);
                this._currentForce[i].z = MotionAlgorithms.SimpleHarmonicMotion(this._tension, displacement, this._dampening, this._speed[i].z);
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
                this._speed[i].x += this._currentForce[i].x * deltaTime_;
                this._speed[i].y += this._currentForce[i].y * deltaTime_;
                this._speed[i].z += this._currentForce[i].z * deltaTime_;
                this._properties[i]().x += this._speed[i].x * deltaTime_;
                this._properties[i]().y += this._speed[i].y * deltaTime_;
                this._properties[i]().z += this._speed[i].z * deltaTime_;
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
                if (Mathf.Abs(this._currentForce[index].x) >= this._minimumForce || Mathf.Abs(this._speed[index].x) >= this._minimumForce ||
                    Mathf.Abs(this._currentForce[index].y) >= this._minimumForce || Mathf.Abs(this._speed[index].y) >= this._minimumForce ||
                    Mathf.Abs(this._currentForce[index].z) >= this._minimumForce || Mathf.Abs(this._speed[index].z) >= this._minimumForce)
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
