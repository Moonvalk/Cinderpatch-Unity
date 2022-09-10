using Moonvalk.Systems;
using Game;

namespace Moonvalk
{
    /// <summary>
    /// Global class for accessing Systems and game engine components.
    /// </summary>
    public static partial class Global
    {
        public static GameManager GetGameManager()
        {
            return GameManager.Instance;
        }
    }
}
