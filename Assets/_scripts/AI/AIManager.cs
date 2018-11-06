using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    public class AIManager : MonoBehaviour
    {
        #region Singleton
        private static AIManager _instance;
        public static AIManager Instance => _instance ?? (_instance = Create());

        private static AIManager Create()
        {
            GameObject singleton = FindObjectOfType<AIManager>()?.gameObject;
            if (singleton == null) singleton = new GameObject { name = typeof(AIManager).Name };
            singleton.AddComponent<AIManager>();
            return singleton.GetComponent<AIManager>();
        }
        #endregion

        private float spawnInterval = 5f;

        public GameObject redCar;
        public GameObject greeCar;


        // Use this for initialization
        void Start()
        {
            StartCoroutine(spawnTimer());
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void spawnCar()
        {

        }


        private IEnumerator spawnTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}

