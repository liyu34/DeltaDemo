using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePanelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        int reason = Main.instance.failReason;
        Text t = GetComponentInChildren<Text>();
        if (reason == 0)
        {
            t.text = "您的星球已迷失...";
        }
        else if (reason == 1)
        {
            t.text = "星球能量已耗尽";
        }
        else
        {
            t.text = "无人生还";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Submit"))
        {
            Main.instance.LoseGame();
        }
    }
}
