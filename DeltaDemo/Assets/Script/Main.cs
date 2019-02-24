﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject planetRoot;
    public EarthController earthController;
    public PlanetMove planetMove;
    public BackgroundScroller bgRemote;
    public BackgroundScroller bgNear;

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

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIRootControl.instance.OpenGalaxy();
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

    public void WinGame()
    {
        galaxyActiveList[CurGalaxyNum] = true;
        CurGalaxyLevel++;
        UIRootControl.instance.OpenGalaxy();
    }

    public void LoseGame()
    {
        UIRootControl.instance.OpenGalaxy();
    }

    public void EnterRoom(int galaxyType)
    {
        isInRoom = true;
        curGalaxyNum = galaxyType;
        earthController.gameObject.SetActive(true);
        UIRootControl.instance.EnterRoom();
    }

    public void LeaveRoom()
    {
        isInRoom = false;
        earthController.gameObject.SetActive(false);
        UIRootControl.instance.LeaveRoom();
    }
}
