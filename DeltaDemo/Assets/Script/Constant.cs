using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public static class PlanetType
    {
        public static int count = 4;   // 类型总数
        public static int planet = 0;   // 行星
        public static int blackHole = 1;   // 黑洞
        public static int ecostar = 2;   // 环星
        public static int comet = 3;   // 彗星
    }

    public static int[,] maxDistance = new int[4, 4]
    {
        { 10, 10, 10, 10 },
        { 10, 10, 10, 10 },
        { 10, 10, 10, 10 },
        { 10, 10, 10, 10 }
    };
}
