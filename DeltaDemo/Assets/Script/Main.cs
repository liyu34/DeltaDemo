using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int totalNum;
    public int planetNum;
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

    private int[] maxNum;
    private int[] count;

    void Start()
    {
        maxNum = new int[Constant.PlanetType.count];
        maxNum[Constant.PlanetType.meteorolite] = planetNum;
        maxNum[Constant.PlanetType.resource] = resourceNum;
        maxNum[Constant.PlanetType.blackhole] = blackholeNum;
        maxNum[Constant.PlanetType.ecostar] = ecostarNum;
        count = new int[Constant.PlanetType.count];
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
            int type = Random.Range(0, Constant.PlanetType.count);
            int thisCount = count[type];
            int thisMaxNum = maxNum[type];
            while (thisCount >= thisMaxNum)
            {
                type = Random.Range(0, Constant.PlanetType.count);
                thisCount = count[type];
                thisMaxNum = maxNum[type];
            }
            count[type]++;

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
                float distOffset = Random.Range(0f, 1f) + Constant.maxDistance[type, lastType];
                float y = Random.Range(-spawnRange.y, spawnRange.y);
                if (y - lastLocalPosition.y >= distOffset)
                {
                    y = lastLocalPosition.y + distOffset - 0.1f * distOffset;
                }
                else if (lastLocalPosition.y - y >= distOffset)
                {
                    y = lastLocalPosition.y - distOffset + 0.1f * distOffset;
                }

                float xOffset = Mathf.Sqrt(distOffset * distOffset - (y - lastLocalPosition.y) * (y - lastLocalPosition.y));
                planet.transform.localPosition = new Vector3(lastLocalPosition.x + xOffset, y, lastLocalPosition.z);
            }

            lastType = type;
            lastLocalPosition = planet.transform.localPosition;
        }
    }
}
