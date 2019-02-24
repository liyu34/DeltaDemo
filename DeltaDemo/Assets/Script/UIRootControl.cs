using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRootControl : MonoBehaviour
{
    public GameObject galaxyUI;
    public GameObject winPanel;
    public GameObject controlPanel;
    public GameObject losePanel;

    public static UIRootControl instance { get; private set; }

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OpenGalaxy()
    {
        galaxyUI.SetActive(true);
        winPanel.SetActive(false);
        controlPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void EnterRoom()
    {
        galaxyUI.SetActive(false);
        winPanel.SetActive(false);
        controlPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    public void LeaveRoom()
    {
        controlPanel.SetActive(false);
        losePanel.SetActive(false);
    }
    
    public void WinGame()
    {
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    public void LoseGame()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

}
