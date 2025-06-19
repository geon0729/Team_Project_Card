using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    public Button newGameButton;
    public Button continueButton;

    void Start()
    {
        UpdateButtonStates();
    }

    void Update()
    {
        // R 키로 저장 초기화
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("저장 데이터 초기화됨");
            UpdateButtonStates();
        }
    }

    void UpdateButtonStates()
    {
        bool hasSave = PlayerPrefs.HasKey("SavedDay");

        continueButton.interactable = hasSave;
        newGameButton.interactable = !hasSave;
    }
}

