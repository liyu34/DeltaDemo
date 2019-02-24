using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRootControl : MonoBehaviour
{
    public GameObject galaxyUI;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetUIEnable(string uiName, bool isEnable)
    {
        switch (uiName)
        {
            case ("Galaxy"):
                galaxyUI.SetActive(isEnable);
                break;
        }
    }
}
