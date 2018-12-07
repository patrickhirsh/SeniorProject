using UnityEngine;

namespace UserInterface
{
    public class PointerCanvas : MonoBehaviour
    {
        public RectTransform ImageTransform;

        private void Start()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            ImageTransform.position = Input.mousePosition;
        }
    }
}
