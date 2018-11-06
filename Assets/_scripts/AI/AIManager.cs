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
        public GameObject greenCar;
        public GameObject spawnPoint;

        // Use this for initialization
        void Start()
        {
            spawnPoint = GameObject.Find("Spawn");
            redCar = GameObject.Find("redCar");
            greenCar = GameObject.Find("greenCar");
            StartCoroutine(spawnTimer());
        }

        // Update is called once per frame
        void Update()
        {

        }

        
        private void spawnCar(GameObject car)
        {
            GameObject newCar = Instantiate(car, car.transform.parent);
            newCar.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        }


        private GameObject getRandomCarType()
        {
            float type = UnityEngine.Random.Range(-1, 1);

            if (type >= 0)
                return redCar;
            else
                return greenCar;
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

