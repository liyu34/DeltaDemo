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

    public void openGalaxy()
    {
        galaxyUI.SetActive(true);
        winPanel.SetActive(false);
        controlPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void enterRoom()
    {
        galaxyUI.SetActive(false);
        winPanel.SetActive(false);
        controlPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    public void leaveRoom()
    {
        controlPanel.SetActive(false);
        losePanel.SetActive(false);
    }
    
    public void winGame()
    {
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    public void loseGame()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

}
