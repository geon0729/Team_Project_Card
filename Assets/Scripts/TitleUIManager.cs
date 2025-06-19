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
        // R Ű�� ���� �ʱ�ȭ
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("���� ������ �ʱ�ȭ��");
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

