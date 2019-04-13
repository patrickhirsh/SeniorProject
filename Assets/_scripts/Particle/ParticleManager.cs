using Game;
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

        fireworkMain.startColor = ColorKey.GetBuildingColor(color);
        explosionMain.startColor = ColorKey.GetBuildingColor(color);
        fireworkTrails.colorOverLifetime = ColorKey.GetBuildingColor(color);

        Destroy(firework, 5);
    }
}
