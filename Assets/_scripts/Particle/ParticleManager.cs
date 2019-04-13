using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
				public void GenerateFirework(Vector3 origin, Building.BuildingColors color)
				{
								Vector3 position = origin;
								var firework = Instantiate(Resources.Load("Fireworks/Firework"), position, new Quaternion(-.5f, 0, 0, .5f)) as GameObject;
								var explosion = firework.transform.Find("Explosion");

								var fireworkMain = firework.GetComponent<ParticleSystem>().main;
								var explosionMain = explosion.GetComponent<ParticleSystem>().main;
								var fireworkTrails = firework.GetComponent<ParticleSystem>().trails;

								fireworkMain.startColor = Game.ColorKey.GetColor(color);
								explosionMain.startColor = Game.ColorKey.GetColor(color);
								fireworkTrails.colorOverLifetime = Game.ColorKey.GetColor(color);

								Destroy(firework, 5);
				}
}
