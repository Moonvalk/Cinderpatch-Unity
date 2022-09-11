
namespace Moonvalk.Utilities.Algorithms
{
    /// <summary>
    /// Algorithms for determining physical motion.
    /// </summary>
    public static class MotionAlgorithms
    {
        /// <summary>
        /// Uses the provided settings to supply a force that creates simple harmonic motion.
        /// </summary>
        /// <param name="tension_"></param>
        /// <param name="offset_"></param>
        /// <param name="dampening_"></param>
        /// <param name="velocity_"></param>
        /// <returns></returns>
        public static float SimpleHarmonicMotion(float tension_, float offset_, float dampening_, float velocity_)
        {
            return (tension_ * offset_) - (dampening_ * velocity_);
        }
    }
}
