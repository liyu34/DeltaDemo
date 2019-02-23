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

    public static int[,] maxDistance = new int[4, 4]
    {
        { 10, 10, 10, 10 },
        { 10, 10, 10, 10 },
        { 10, 10, 10, 10 },
        { 10, 10, 10, 10 }
    };
}
