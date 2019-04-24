using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CameraBubble : MonoBehaviour
{
    public float MinRadius;
    public float MaxRadius;

    public SphereCollider SphereCollider;

    private void Awake()
    {
        SphereCollider = GetComponent<SphereCollider>();
    }
}