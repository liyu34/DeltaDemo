using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyUIControl : MonoBehaviour
{
    public GameObject[] galaxyOnImage;

    private bool[] isActiveList = new bool[10];
    private int curGalaxyType;

    public void OnSelectGalaxy(int galaxyType)
    {
        bool canSelect = false;
        switch (galaxyType)
        {
            case 0:
                break;
            case 1:
            case 2:
            case 3:
                canSelect = isActiveList[0];
                break;
            case 4:
            case 5:
                canSelect = isActiveList[0] && (isActiveList[1] || isActiveList[2] || isActiveList[3]);
                break;
            case 6:
            case 7:
            case 8:
                canSelect = isActiveList[0] && (isActiveList[1] || isActiveList[2] || isActiveList[3])
                            && (isActiveList[4] || isActiveList[5]);
                break;
            case 9:
                canSelect = isActiveList[0] && (isActiveList[1] || isActiveList[2] || isActiveList[3])
                            && (isActiveList[4] || isActiveList[5]) && (isActiveList[6] || isActiveList[7] || isActiveList[8]);
                break;
        }
        if (canSelect)
        {

        }
    }
}
