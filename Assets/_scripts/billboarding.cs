using System.Collections;
using System.Collections.Generic;
using RideShareLevel;
using UnityEngine;

public class billboarding : LevelObject
{
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        if (!MainCamera) return;
        transform.LookAt(transform.position + MainCamera.transform.rotation * Vector3.forward);
    }
}
