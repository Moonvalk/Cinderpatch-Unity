
namespace Moonvalk.Accessory
{
    /// <summary>
    /// Static class containing basic extended conversion functions.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Converts a boolean value to an integer.
        /// </summary>
        /// <param name="flag_">The input boolean flag to convert.</param>
        /// <returns>Returns 1 if true or 0 if false.</returns>
        public static int ToInt(bool flag_)
        {
            return (flag_ ? 1 : 0);
        }
    }
}
