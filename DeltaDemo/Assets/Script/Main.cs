using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int meteoroliteNum;
    public int resourceNum;
    public int blackholeNum;
    public int ecostarNum;

    public float spawnWait;
    public Vector3 spawnRange;

    public GameObject[] meteorolitePrefabs;
    public GameObject[] resourcePrefabs;
    public GameObject[] blackholePrefabs;
    public GameObject[] ecostarPrefabs;

    public GameObject planetRoot;
    public EarthController earthController;
    public PlanetMove planetMove;
    public BackgroundScroller bgRemote;
    public BackgroundScroller bgNear;

    private float bgRemoteRatio = 0.3f;
    private float bgNearRatio = 0.6f;

    private float earthVelocity
    {
        get => earthController.HorizontalVelocity;
    }

    private int totalNum;
    private int[] typeList;
    private bool[] hasSpawn;
    private int[] maxNum;
    private int[] count;

    void Start()
    {
        maxNum = new int[Constant.PlanetType.count];
        maxNum[Constant.PlanetType.meteorolite] = meteoroliteNum;
        maxNum[Constant.PlanetType.resource] = resourceNum;
        maxNum[Constant.PlanetType.blackhole] = blackholeNum;
        maxNum[Constant.PlanetType.ecostar] = ecostarNum;
        count = new int[Constant.PlanetType.count];

        totalNum = meteoroliteNum + resourceNum + blackholeNum + ecostarNum;
        typeList = new int[totalNum];
        int i = 0;
        for (; i < meteoroliteNum; i++)
        {
            typeList[i] = Constant.PlanetType.meteorolite;
        }
        for (; i < meteoroliteNum + resourceNum; i++)
        {
            typeList[i] = Constant.PlanetType.resource;
        }
        for (; i < meteoroliteNum + resourceNum + blackholeNum; i++)
        {
            typeList[i] = Constant.PlanetType.blackhole;
        }
        for (; i < totalNum; i++)
        {
            typeList[i] = Constant.PlanetType.ecostar;
        }
        hasSpawn = new bool[totalNum];
        SpawnPlanets();
    }

    void Update()
    {
        if (earthVelocity <= 0)
        {
            planetMove.speed = 0;
            bgRemote.scrollSpeed = 0;
            bgNear.scrollSpeed = 0;
        }
        else
        {
            planetMove.speed = earthVelocity;
            bgRemote.scrollSpeed = -earthVelocity * bgRemoteRatio;
            bgNear.scrollSpeed = -earthVelocity * bgNearRatio;
        }
    }

    private void SpawnPlanets()
    {
        int lastType = -1;
        Vector3 lastLocalPosition = Vector3.zero;
        for (int i = 0; i < totalNum; i++)
        {
            int index = Random.Range(0, totalNum);
            while (hasSpawn[index])
            {
                index = Random.Range(0, totalNum);
            }
            hasSpawn[index] = true;
            int type = typeList[index];

            GameObject prefab = meteorolitePrefabs[0];
            Vector3 spawnPosition = new Vector3(spawnRange.x, 0, spawnRange.z);
            Quaternion spawnRotation;
            GameObject planet;
            switch (type)
            {
                case (0):
                    prefab = meteorolitePrefabs[Random.Range(0, meteorolitePrefabs.Length)];
                    break;
                case (1):
                    prefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];
                    break;
                case (2):
                    prefab = blackholePrefabs[Random.Range(0, blackholePrefabs.Length)];
                    break;
                case (3):
                    prefab = ecostarPrefabs[Random.Range(0, ecostarPrefabs.Length)];
                    break;
            }
            
            if (lastType == -1)
            {
                spawnPosition = new Vector3(spawnRange.x, Random.Range(-spawnRange.y, spawnRange.y), spawnRange.z);
            }
            spawnRotation = Quaternion.identity;
            planet = Instantiate(prefab, spawnPosition, spawnRotation);
            planet.transform.parent = planetRoot.transform;

            if (lastType != -1)
            {
                float distOffset = Random.Range(0f, 10f) + Constant.maxDistance[type, lastType];
                float y = Random.Range(-spawnRange.y, spawnRange.y);
                float xOffset;
                if (Mathf.Abs(y - lastLocalPosition.y) >= distOffset)
                {
                    xOffset = Random.Range(0f, 2f);
                }
                else
                {
                    xOffset = Mathf.Sqrt(distOffset * distOffset - (y - lastLocalPosition.y) * (y - lastLocalPosition.y));
                }
                planet.transform.localPosition = new Vector3(lastLocalPosition.x + xOffset, y, lastLocalPosition.z);
            }

            lastType = type;
            lastLocalPosition = planet.transform.localPosition;
        }
    }
}
