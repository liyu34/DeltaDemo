using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject planetRoot;
    public GameObject[] planetPrefabs;
    public int planetCount;
    public float spawnWait;
    public Vector3 spawnRange;

    public EarthController earthController;
    public PlanetMove planetMove;
    public BackgroundScroller bgRemote;
    public BackgroundScroller bgNear;

    private float earthVelocity
    {
        get => earthController.HorizontalVelocity;
    }

    void Start()
    {
        StartCoroutine(SpawnPlanets());
    }   

    void Update()
    {
        planetMove.speed = -earthVelocity;
    }

    IEnumerator SpawnPlanets()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            for (int i = 0; i < planetCount; i++)
            {
                GameObject hazard = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
                Vector3 spawnPosition = new Vector3(spawnRange.x, Random.Range(-spawnRange.y, spawnRange.y), spawnRange.z);
                Quaternion spawnRotation = Quaternion.identity;
                GameObject planet = Instantiate(hazard, spawnPosition, spawnRotation);
                planet.transform.parent = planetRoot.transform;
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(4);
        }
    }
}
