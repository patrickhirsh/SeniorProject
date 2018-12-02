using UnityEngine;

public static class MethodExtensionForMonoBehaviourTransform
{
    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    public static T GetOrAddComponent<T>(this Component child) where T : Component
    {
        var result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    public static T GetOrAddComponent<T>(this MonoBehaviour child) where T : Component
    {
        var result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }
}