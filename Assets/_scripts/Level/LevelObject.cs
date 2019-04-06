using UnityEngine;

namespace RideShareLevel
{
    public abstract class LevelObject : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private Level Level;

        internal Level CurrentLevel
        {
            get
            {
                if (Level != null)
                {
                    return Level;
                }
                return Level = GetComponentInParent<Level>();
            }
        }

        internal EntityController EntityController => CurrentLevel.EntityController;

#if UNITY_EDITOR
        public void SetLevel(Level level)
        {
            UnityEditor.Undo.RecordObject(this, $"Set Level for {name}");
            Level = GetComponentInParent<Level>();
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif

    }
}