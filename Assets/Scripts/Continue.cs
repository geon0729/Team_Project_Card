using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{

    public Button continueButton;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("SavedDay"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }

        continueButton.onClick.AddListener(OnContinueClicked);
    }

    void OnContinueClicked()
    {
        
        SceneManager.LoadScene("InGame");
    }

    
    void Update()
    {
        
    }
}
