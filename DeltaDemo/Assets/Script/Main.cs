using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject planetRoot;
    public EarthController earthController;
    public PlanetMove planetMove;
    public BackgroundScroller bgRemote;
    public Material[] bgRemoteMaterials;
    public BackgroundScroller bgNear;
    public Material[] bgNearMaterials;
    public MeshRenderer bgMeshRenderer;
    public Material[] bgMaterials;
    public int failReason = 0;
    public bool isPaused
    {
        get
        {
            return _gamePaused;
        }
    }

    private bool _gamePaused = false;
    private bool isInRoom = false;
    private int curGalaxyNum = 0;
    private int curGalaxyLevel = 0;
    private bool[] galaxyActiveList = new bool[10];

    private float earthReturnSpeed = 3f;
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

    public static Main instance;
    public int CurGalaxyNum
    {
        get
        {
            return curGalaxyNum;
        }
        set
        {
            curGalaxyNum = Mathf.Clamp(value, 0, 9);
        }
    }
    public int CurGalaxyLevel
    {
        get
        {
            return curGalaxyLevel;
        }
        set
        {
            curGalaxyLevel = Mathf.Clamp(value, 0, 4);
        }
    }
    public bool[] GalaxyActiveList
    {
        get
        {
            return galaxyActiveList;
        }
    }

    public void PauseGame()
    {
        _gamePaused = true;
    }

    public void ResumeGame()
    {
        _gamePaused = false;
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIRootControl.instance.StartGame();
        earthController.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isInRoom)
        {
            float posX = earthController.GetEarthPosX();
            if (earthVelocity <= 0)
            {
                planetMove.speed = 0;
                bgRemote.scrollSpeed = 0;
                bgNear.scrollSpeed = 0;
            }
            else
            {
                if (posX > -10)
                {
                    earthController.SetEarthPosX(Mathf.Max(-10, posX - Time.deltaTime * earthReturnSpeed));
                    planetMove.speed = earthVelocity + earthReturnSpeed;
                    bgRemote.scrollSpeed = -earthVelocity * bgRemoteRatio + earthReturnSpeed;
                    bgNear.scrollSpeed = -earthVelocity * bgNearRatio + earthReturnSpeed;
                }
                else
                {
                    planetMove.speed = earthVelocity;
                    bgRemote.scrollSpeed = -earthVelocity * bgRemoteRatio;
                    bgNear.scrollSpeed = -earthVelocity * bgNearRatio;
                }
            }
        }
    }

    public void OpenGalaxy()
    {
        UIRootControl.instance.OpenGalaxy();
    }

    public void WinGame()
    {
        galaxyActiveList[CurGalaxyNum] = true;
        CurGalaxyLevel++;
        LeaveRoom();
        UIRootControl.instance.OpenGalaxy();
    }

    public void LoseGame()
    {
        LeaveRoom();
        UIRootControl.instance.OpenGalaxy();
        ResetGame();
    }

    public void ResetGame()
    {
        curGalaxyNum = 0;
        EarthModel.instance.ResetData();
        earthController.ResetData();
    }

    public void EnterRoom(int galaxyType)
    {
        int bgImageNum = Random.Range(0, 3);
        bgMeshRenderer.material = bgMaterials[bgImageNum];
        bgRemote.SetMaterial(bgRemoteMaterials[bgImageNum]);
        bgNear.SetMaterial(bgNearMaterials[bgImageNum]);
        GetComponent<AudioSource>().Play();
        isInRoom = true;
        curGalaxyNum = galaxyType;
        earthController.gameObject.SetActive(true);
        earthController.EndDistance = Constant.galaxyLevelEndDistance[galaxyType];
        earthController.TotalDistance = Constant.galaxyLevelEndDistance[galaxyType];
        UIRootControl.instance.EnterRoom();
    }

    public void LeaveRoom()
    {
        GetComponent<AudioSource>().Stop();
        isInRoom = false;
        earthController.gameObject.SetActive(false);
        UIRootControl.instance.LeaveRoom();
    }

    public float EarthProgress()
    {
        if (earthController)
        {
            return earthController.Progress;
        }
        return 0f;
    }
}
