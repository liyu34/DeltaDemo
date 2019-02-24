using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GalaxyUIControl : MonoBehaviour
{
    public GameObject[] galaxyOnImage;

    public int meteoroliteNum;
    public int resourceNum;
    public int blackholeNum;
    public int ecostarNum;
    public int satelliteNum;

    public Vector3 spawnRange;
    public Vector3 spawnRangeForEcostar;

    public GameObject[] meteorolitePrefabs;
    public GameObject[] resourcePrefabs;
    public GameObject[] blackholePrefabs;
    public GameObject[] ecostarPrefabs;
    public GameObject[] satellitePrefabs;

    public GameObject planetRoot;

    private int totalNum;
    private int[] typeList;
    private bool[] hasSpawn;

    private bool[] activeList;
    private int activeLevel = 0;

    void OnEnable()
    {
        activeList = Main.instance.GalaxyActiveList;
        activeLevel = Main.instance.CurGalaxyLevel;

        for (int i = 0; i < 10; i++)
        {
            if (activeList[i])
            {
                galaxyOnImage[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        int firstNum = Constant.galaxyLevelFirstNum[activeLevel];
        int nextFirstNum = activeLevel == 4 ? 10 : Constant.galaxyLevelFirstNum[activeLevel + 1];

        for (int i = 0; i < firstNum; i++)
        {
            galaxyOnImage[i].GetComponent<Button>().interactable = false;
        }

        for (int i = firstNum; i < nextFirstNum; i++)
        {
            galaxyOnImage[i].GetComponent<Button>().interactable = true;
        }

        for (int i = nextFirstNum; i < 10; i++)
        {
            galaxyOnImage[i].GetComponent<Button>().interactable = false;
        }

        EventSystem.current.SetSelectedGameObject(galaxyOnImage[firstNum]);
        galaxyOnImage[firstNum].GetComponent<Button>().Select();
        galaxyOnImage[firstNum].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnSelectGalaxy(int galaxyType)
    {
        galaxyOnImage[galaxyType].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnUnselectGalaxy(int galaxyType)
    {
        if (!activeList[galaxyType])
        {
            galaxyOnImage[galaxyType].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void OnEnterGalaxy(int galaxyType)
    {
        bool canSelect = false;
        switch (galaxyType)
        {
            case 0:
                canSelect = true;
                break;
            case 1:
            case 2:
            case 3:
                canSelect = activeList[0];
                break;
            case 4:
            case 5:
                canSelect = activeList[0] && (activeList[1] || activeList[2] || activeList[3]);
                break;
            case 6:
            case 7:
            case 8:
                canSelect = activeList[0] && (activeList[1] || activeList[2] || activeList[3])
                            && (activeList[4] || activeList[5]);
                break;
            case 9:
                canSelect = activeList[0] && (activeList[1] || activeList[2] || activeList[3])
                            && (activeList[4] || activeList[5]) && (activeList[6] || activeList[7] || activeList[8]);
                break;
        }
        if (canSelect)
        {
            Main.instance.EnterRoom(galaxyType);
            InitSpawnParams();
            SpawnPlanets();
            transform.gameObject.SetActive(false);
        }
    }

    private void InitSpawnParams()
    {
        totalNum = meteoroliteNum + resourceNum + blackholeNum + ecostarNum + satelliteNum;
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
        for (; i < totalNum - satelliteNum; i++)
        {
            typeList[i] = Constant.PlanetType.ecostar;
        }
        for (; i < totalNum; i++)
        {
            typeList[i] = Constant.PlanetType.satellite;
        }
        hasSpawn = new bool[totalNum];
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
                case (4):
                    prefab = satellitePrefabs[Random.Range(0, satellitePrefabs.Length)];
                    break;
            }

            if (lastType == -1)
            {
                if (type == Constant.PlanetType.ecostar)
                {
                    spawnPosition = new Vector3(spawnRangeForEcostar.x, Random.Range(-spawnRangeForEcostar.y, spawnRangeForEcostar.y), spawnRangeForEcostar.z);
                }
                else
                {
                    spawnPosition = new Vector3(spawnRange.x, Random.Range(-spawnRange.y, spawnRange.y), spawnRange.z);
                }
            }
            spawnRotation = Quaternion.identity;
            planet = Instantiate(prefab, spawnPosition, spawnRotation);
            planet.transform.parent = planetRoot.transform;

            if (lastType != -1)
            {
                float distOffset = Random.Range(0f, 10f) + Constant.maxDistance[type, lastType];
                float y;
                if (type == Constant.PlanetType.ecostar)
                {
                    y = Random.Range(-spawnRangeForEcostar.y, spawnRangeForEcostar.y);
                }
                else
                {
                    y = Random.Range(-spawnRange.y, spawnRange.y);
                }
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
