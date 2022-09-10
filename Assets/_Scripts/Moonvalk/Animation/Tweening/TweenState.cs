
namespace Moonvalk.Animation
{
    /// <summary>
    /// Provides an index for each available MVTween state.
    /// </summary>
    public enum TweenState
    {
        /// <summary> This Tween is idle and waiting for instruction. </summary>
        Idle,
        /// <summary> This Tween has just begun. </summary>
        Start,
        /// <summary> This Tween is actively updating. </summary>
        Update,
        /// <summary> This Tween has just completed. </summary>
        Complete,
        /// <summary> This Tween is idle and is now considered stopped. </summary>
        Stopped,
    }
}
