using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRootControl : MonoBehaviour
{
    public GameObject galaxyUI;
    public GameObject winPanel;
    public GameObject controlPanel;
    public GameObject losePanel;
    public GameObject startPanel;

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

    public void StartGame()
    {
        galaxyUI.SetActive(false);
        winPanel.SetActive(false);
        controlPanel.SetActive(false);
        losePanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void OpenGalaxy()
    {
        galaxyUI.SetActive(true);
        winPanel.SetActive(false);
        controlPanel.SetActive(false);
        losePanel.SetActive(false);
        startPanel.SetActive(false);
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
