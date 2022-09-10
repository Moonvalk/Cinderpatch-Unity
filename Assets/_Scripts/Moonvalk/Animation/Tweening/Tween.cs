using Moonvalk.Accessory;

namespace Moonvalk.Animation
{
    /// <summary>
    /// A basic Tween which handles singular float value properties.
    /// </summary>
    public class Tween : BaseTween<float>
    {
        /// <summary>
        /// Constructor for creating a new Tween.
        /// </summary>
        /// <param name="referenceValues_">Array of references to float values.</param>
        public Tween(params Ref<float>[] referenceValues_) : base(referenceValues_)
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
                this._properties[i]() = this._easingFunctions[i](this._percentage, this._startValues[i], this._targetValues[i]);
            }
        }
    }
}