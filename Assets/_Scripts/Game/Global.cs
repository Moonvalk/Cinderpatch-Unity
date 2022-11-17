using Moonvalk.Systems;
using Game;

namespace Moonvalk
{
    /// <summary>
    /// Global class for accessing Systems and game engine components.
    /// </summary>
    public static partial class Global
    {
        public static float MusicVolume = 0.6f;
        public static float SoundVolume = 1f;
        public static bool FullScreen = false;
        public static bool Paused = false;

        public static GameManager GetGameManager()
        {
            return GameManager.Instance;
        }
    }
}
