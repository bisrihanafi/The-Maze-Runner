using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamePlayerNow : MonoBehaviour
{
    const string playerNamePrefKey = "PlayerName";
    Text textValue;
    string defaultName;
    // Start is called before the first frame update
    void Start()
    {
        defaultName = string.Empty;
        textValue = this.GetComponent<Text>();
        if (textValue != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                textValue.text = defaultName;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (textValue != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                textValue.text = defaultName;
            }
        }
    }
}
