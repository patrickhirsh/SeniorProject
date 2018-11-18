using UnityEngine;

namespace Utility
{
    public class Debugger : MonoBehaviour
    {
        #region Singleton
        private static Debugger _instance;
        public static Debugger Instance
        {
            get
            {
                if (Application.isPlaying) return _instance ?? (_instance = Create());
                return Create();
            }
        }

        private static Debugger Create()
        {
            GameObject singleton = FindObjectOfType<Debugger>()?.gameObject;
            if (singleton == null)
            {
                singleton = new GameObject { name = $"[{typeof(Debugger).Name}]" };
                singleton.AddComponent<Debugger>();
            }
            return singleton.GetComponent<Debugger>();
        }
        #endregion

        public string ProfilePath = "Debugging/Debugging.asset";
        public DebuggingProfile Profile => Resources.Load<DebuggingProfile>(ProfilePath) ?? ScriptableObject.CreateInstance<DebuggingProfile>();
    }
}