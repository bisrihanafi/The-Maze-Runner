using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SettingGamePlay : MonoBehaviour
{
    
    // Start is called before the first frame update
    


    public void AmbilSetting()
    {
        //Dijadikan local variable karena menghindari error saat pertama aplikasi dijalankan akan memanggil object yang sebenarnya belum diaktifkan
        Toggle debugi = GameObject.Find("Debug").GetComponent<Toggle>();
        Toggle statis = GameObject.Find("Statis").GetComponent<Toggle>();
        Toggle dinamis = GameObject.Find("Dinamis").GetComponent<Toggle>();
        Dropdown diff = GameObject.Find("Diff").GetComponent<Dropdown>();
        Dropdown npc = GameObject.Find("NPC_Bot").GetComponent<Dropdown>();

        if (PlayerPrefs.GetInt("DebugerMode")==1)
        {
            debugi.isOn=true;
        }else if (PlayerPrefs.GetInt("DebugerMode") == 0)
        {
            debugi.isOn = false;
        }
        if (PlayerPrefs.GetInt("TypeLabirin") == 0)
        {
            statis.isOn = true;
            dinamis.isOn = false;
        }
        else if (PlayerPrefs.GetInt("TypeLabirin") == 1)
        {
           statis.isOn = false;
           dinamis.isOn = true;
           diff.value= PlayerPrefs.GetInt("TypeLabirinDiff");
           
        }
        npc.value = PlayerPrefs.GetInt("NPCOnMap")-1;
        
    }
    public void WriteString()
    {
        //Dijadikan local variable karena menghindari error saat pertama aplikasi dijalankan akan memanggil object yang sebenarnya belum diaktifkan
        Toggle statis = GameObject.Find("Statis").GetComponent<Toggle>();
        Toggle debugi = GameObject.Find("Debug").GetComponent<Toggle>();
        Dropdown npc = GameObject.Find("NPC_Bot").GetComponent<Dropdown>();
        Dropdown diff = GameObject.Find("Diff").GetComponent<Dropdown>();

        if (debugi.isOn)
        {
            PlayerPrefs.SetInt("DebugerMode", 1);
        }
        else {
            PlayerPrefs.SetInt("DebugerMode", 0);
        }
        if (statis.isOn)
        {
            PlayerPrefs.SetInt("TypeLabirin", 0);            
        }
        else {
            PlayerPrefs.SetInt("TypeLabirin", 1);
            if (diff.options[diff.value].text.Equals("Simpel")) 
            {
                PlayerPrefs.SetInt("TypeLabirinDiff", 0);
            }
            else if (diff.options[diff.value].text.Equals("Medium"))
            {
                PlayerPrefs.SetInt("TypeLabirinDiff", 1);
            }
            else if (diff.options[diff.value].text.Equals("Kompleks"))
            {
                PlayerPrefs.SetInt("TypeLabirinDiff", 2);
            }
        }

        if (npc.options[npc.value].text.Equals("1 NPC"))
        {
            PlayerPrefs.SetInt("NPCOnMap", 1);
        }
        else if (npc.options[npc.value].text.Equals("2 NPC"))
        {
            PlayerPrefs.SetInt("NPCOnMap", 2);
        }
        else if (npc.options[npc.value].text.Equals("3 NPC"))
        {
            PlayerPrefs.SetInt("NPCOnMap", 3);
        }
    }


}
