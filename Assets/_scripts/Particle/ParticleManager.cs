using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
				public void GenerateFirework(Vector3 origin)
				{
								Vector3 position = origin;
								var firework = Instantiate(Resources.Load("Fireworks/Firework"), position, new Quaternion(-.5f, 0, 0, .5f)) as GameObject;
								Destroy(firework, 5);
				}
}
