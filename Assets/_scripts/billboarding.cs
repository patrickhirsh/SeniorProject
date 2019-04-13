using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboarding : MonoBehaviour
{

    // Use this for initialization
    public Camera m_Camera;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward);
    }
}
