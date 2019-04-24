using UnityEngine;

namespace RideShareLevel
{
    public abstract class LevelObject : MonoBehaviour
    {
        private Camera _camera;
        protected Camera MainCamera => _camera ? _camera : _camera = Camera.main;

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

                return Level = LevelManager.Instance.CurrentLevel;
            }
        }

        internal EntityController EntityController => CurrentLevel.EntityController;
        internal PassengerController PassengerController => CurrentLevel.PassengerController;
        internal ScoreController ScoreController => CurrentLevel.ScoreController;

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