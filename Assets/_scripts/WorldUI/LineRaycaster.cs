using UnityEngine;

namespace _scripts
{
    [ExecuteInEditMode]
    public class LineRaycaster : MonoBehaviour
    {
        private float _y;
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            ComputeLine();
        }

        private void ComputeLine()
        {
            // Check if height has changed
            if (Mathf.Abs(_y - transform.position.y) > .05)
            {
                var position = transform.position;
                var hit = Physics.Raycast(position, Vector3.down, out var hitInfo, 50);
                _lineRenderer.SetPosition(1, transform.InverseTransformPoint(hitInfo.point));
                _y = position.y;
            }
        }
    }
}