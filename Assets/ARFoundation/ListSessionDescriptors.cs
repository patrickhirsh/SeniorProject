﻿using System.Collections.Generic;
using UnityEngine.Experimental;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.Mock.Example
{
    static class ListSessionDescriptors
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Sessions/List Available Sessions")]
        static void ListSessions()
        {
            var descriptors = new List<XRSessionSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(descriptors);
            foreach (var descriptor in descriptors)
            {
                Debug.LogFormat("Session: {0}", descriptor.id);
            }

            if (descriptors.Count == 0)
                Debug.Log("No sessions available.");
        }
#endif
    }
}
