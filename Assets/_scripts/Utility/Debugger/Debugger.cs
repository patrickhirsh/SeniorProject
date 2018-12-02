using UnityEngine;

namespace Utility
{
    public class Debugger : MonoBehaviour
    {
        public const string PROFILE_PATH = "Debugging/Debugging";
        private static DebuggingProfile _profile;
        public static DebuggingProfile Profile => _profile ?? (_profile = Resources.Load<DebuggingProfile>(PROFILE_PATH) ?? ScriptableObject.CreateInstance<DebuggingProfile>());
    }
}