using UnityEngine;
using Moonvalk.Accessory;

namespace Moonvalk.Animation
{
    /// <summary>
    /// A Tween object which handles 3 point vector properties.
    /// </summary>
    public class TweenVec3 : BaseTween<Vector3>
    {
        /// <summary>
        /// Constructor for creating a new Tween.
        /// </summary>
        /// <param name="referenceValues_">Array of references to float values.</param>
        public TweenVec3(params Ref<Vector3>[] referenceValues_) : base(referenceValues_)
        {
        }

        /// <summary>
        /// Method used to update all properties available to this object.
        /// </summary>
        protected override void updateProperties()
        {
            // Apply easing and set properties.
            for (int i = 0; i < this._properties.Length; i++)
            {
                Debug.Log("Running Vector3 Tween now...");
                this._properties[i]().x = this._easingFunctions[i](this._percentage, this._startValues[i].x, this._targetValues[i].x);
                this._properties[i]().y = this._easingFunctions[i](this._percentage, this._startValues[i].y, this._targetValues[i].y);
                this._properties[i]().z = this._easingFunctions[i](this._percentage, this._startValues[i].z, this._targetValues[i].z);
            }
        }
    }
}