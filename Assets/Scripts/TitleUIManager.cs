using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{

    public Button startNewGameButton;
    public Button continueButton;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("SavedDay"))
        {
            startNewGameButton.interactable = false;
            continueButton.interactable = true;
        }
        else
        {
            startNewGameButton.interactable = true;
            continueButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
