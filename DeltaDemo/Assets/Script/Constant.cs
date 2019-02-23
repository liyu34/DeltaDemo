using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public static class PlanetType
    {
        public static int count = 4;   // 类型总数
        public static int meteorolite = 0;   // 陨石
        public static int resource = 1;   // 资源点
        public static int blackhole = 2;   // 黑洞
        public static int ecostar = 3;   // 环星
    }

    public static float[,] maxDistance = new float[4, 4]
    {
        { 0.8f, 0.5f, 6f, 3.5f },
        { 0.5f, 0.2f, 6f, 3.2f },
        { 6f, 6f, 20f, 8f },
        { 3.5f, 3.2f, 8f, 9f }
    };

    public static float ecoStarRadius = 6.255f;
    public static float surroundAngleSpeed = 25;
    public static float stopSurroundDelayTime = 1;
}
