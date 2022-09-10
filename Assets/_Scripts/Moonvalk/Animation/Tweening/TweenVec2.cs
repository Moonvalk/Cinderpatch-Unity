using UnityEngine;
using Moonvalk.Accessory;

namespace Moonvalk.Animation
{
    /// <summary>
    /// A Tween object which handles 2 point vector properties.
    /// </summary>
    public class TweenVec2 : BaseTween<Vector2>
    {
        /// <summary>
        /// Constructor for creating a new Tween.
        /// </summary>
        /// <param name="referenceValues_">Array of references to float values.</param>
        public TweenVec2(params Ref<Vector2>[] referenceValues_) : base(referenceValues_)
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
                this._properties[i]().x = this._easingFunctions[i](this._percentage, this._startValues[i].x, this._targetValues[i].x);
                this._properties[i]().y = this._easingFunctions[i](this._percentage, this._startValues[i].y, this._targetValues[i].y);
            }
        }
    }
}