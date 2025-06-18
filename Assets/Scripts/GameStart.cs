using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
    }

    void OnStartClicked()
    {
        PlayerPrefs.DeleteKey("SavedDay");
        PlayerPrefs.Save();

        SceneManager.LoadScene("InGame");
    }
}
