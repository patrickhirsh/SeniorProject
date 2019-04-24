using System;
using UnityEngine;
using System.Collections;

public class MobileOnly : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        gameObject.SetActive(false);
#endif
    }
}
