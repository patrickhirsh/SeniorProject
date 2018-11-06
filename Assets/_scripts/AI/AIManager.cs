using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    public class AIManager : MonoBehaviour
    {
        #region Singleton
        private static PathfindingManager _instance;
        public static PathfindingManager Instance => _instance ?? (_instance = Create());

        private static PathfindingManager Create()
        {
            GameObject singleton = FindObjectOfType<PathfindingManager>()?.gameObject;
            if (singleton == null) singleton = new GameObject { name = typeof(PathfindingManager).Name };
            singleton.AddComponent<PathfindingManager>();
            return singleton.GetComponent<PathfindingManager>();
        }
        #endregion


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

