using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public static class PlanetType
    {
        public static int count = 5;   // 类型总数
        public static int meteorolite = 0;   // 陨石
        public static int resource = 1;   // 资源点
        public static int blackhole = 2;   // 黑洞
        public static int ecostar = 3;   // 环星
        public static int satellite = 4;   // 卫星
    }

    public static float[,] maxDistance = new float[5, 5]
    {
        { 0.8f, 0.5f, 6f, 3.5f, 3f },
        { 0.5f, 0.2f, 6f, 3.2f, 3f },
        { 6f, 6f, 20f, 8f, 6f },
        { 3.5f, 3.2f, 8f, 9f, 7f },
        { 3f, 3f, 6f, 7f, 8f}
    };

    public static int[] galaxyLevelFirstNum = new int[5] { 0, 1, 4, 6, 9 };

    public static int[] galaxyLevelEndDistance = new int[10] { 315, 415, 415, 420, 515, 515, 550, 555, 555, 600 };

    public static string[] galaxyName = new string[10] { "银河系", "水母座", "小恶魔座", "史莱姆座", "冰淇淋座", "天使座-前段", "流沙座", "天使座-后段", "草履虫座", "水晶座"};

    public static float ecoStarRadius = 5.008f;
    public static float surroundAngleSpeed = 55;
    public static float stopSurroundDelayTime = 1;
    public static float afterStopKeepVelocityDelay = 1;
    public static float afterStopHorizontalVelocity = 6;
    public static float maxHorizontalVelocity = 4.5f;
    public static float overMaxHVelocityDecreaseDelta = 1f;
    public static float initialHorizontalVelocity = 3f;
}
