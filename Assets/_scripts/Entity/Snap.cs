using UnityEditor;
using UnityEngine;
using Utility;

namespace RideShareLevel
{
    [ExecuteInEditMode]
    public class Snap : MonoBehaviour
    {
        private void Update()
        {
            if (!Application.isPlaying && transform.hasChanged && !Input.GetMouseButton(0))
            {
                transform.SnapToGrid();
            }
        }

        public void SnapRotation()
        {

            var rotation = transform.eulerAngles;
            rotation.y = Mathf.Round(rotation.y / 90) * 90;
            transform.eulerAngles = rotation;
        }
    }
}