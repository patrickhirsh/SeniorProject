using System;
using UnityEngine;
using System.Collections;

public class DesktopOnly : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_STANDALONE
        gameObject.SetActive(false);
#endif
    }
}
