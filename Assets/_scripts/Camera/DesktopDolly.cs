using UnityEngine;
using System.Collections;
using RideShareLevel;
using UnityEngine.UIElements;

public class DesktopDolly : MonoBehaviour
{
    private Camera _camera;
    protected Camera MainCamera => _camera ? _camera : _camera = Camera.main;

    public float Angle = 90;
    public float Height = 45;
    public float RotationSpeed = 10;

    [Header("Orthographic Camera Settings")]
    public bool UseOrthographic = false;
    public float MinSize = 10;
    public float MaxSize = 50;
    public float ZoomSpeed = 10;

    private void Update()
    {
        if (!MainCamera) return;
        var levelTransform = LevelManager.Instance.CurrentLevel.Bubble.transform;
        var q = Quaternion.AngleAxis(Angle, Vector3.up);
        if (Physics.Linecast(levelTransform.position + q * Vector3.right * 1000, levelTransform.position, out var hitInfo, 1 << 10))
        {
            transform.position = hitInfo.point + Vector3.up * Height + Vector3.up * LevelManager.Instance.CurrentLevel.Bubble.transform.localPosition.y;
            MainCamera.transform.LookAt(levelTransform);
        }

        var hInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(hInput) > 0.01)
        {
            Angle -= hInput * Time.deltaTime * RotationSpeed;
        }


        if (UseOrthographic)
        {
            var vInput = Input.GetAxis("Vertical");
            if (Mathf.Abs(vInput) > 0.01)
            {
                var orthographicSize = MainCamera.orthographicSize;
                orthographicSize -= vInput * Time.deltaTime * ZoomSpeed;
                MainCamera.orthographicSize = Mathf.Clamp(orthographicSize, MinSize, MaxSize);
            }
        }
        else
        {
            var bubble = LevelManager.Instance.CurrentLevel.Bubble;

            var vInput = Input.GetAxis("Vertical");
            if (Mathf.Abs(vInput) > 0.01)
            {
                var radius = bubble.SphereCollider.radius;
                radius -= vInput * Time.deltaTime * ZoomSpeed;
                bubble.SphereCollider.radius = Mathf.Clamp(radius, bubble.MinRadius, bubble.MaxRadius);
            }
        }
    }

    private void OnDrawGizmos()
    {
        var levelTransform = LevelManager.Instance.CurrentLevel.Bubble.transform;
        var q = Quaternion.AngleAxis(Angle, Vector3.up);
        Gizmos.DrawLine(levelTransform.position + q * Vector3.right * 1000 + Vector3.up * Height, levelTransform.position);
    }
}
