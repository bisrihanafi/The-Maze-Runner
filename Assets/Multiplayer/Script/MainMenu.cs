using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject PanelMainMenu;
    public GameObject PanelSetting;
    // Start is called before the first frame update
    void Start()
    {
        PanelMainMenu.SetActive(true);
        PanelSetting.SetActive(false);
    }
    public void Pengaturan() {
        PanelMainMenu.SetActive(false);
        PanelSetting.SetActive(true);
    }
    public void Kembali() {
        PanelMainMenu.SetActive(true);
        PanelSetting.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ExitGame() {
        Application.Quit();
    }
}
