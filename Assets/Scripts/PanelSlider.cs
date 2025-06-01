using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSlider : MonoBehaviour
{
    public RectTransform kitchenPanel;     // 이동할 패널
    public Vector2 hiddenPosition;         // 시작 위치
    public Vector2 visiblePosition;        // 들어올 위치
    public float slideSpeed = 5f;

    private bool isSliding = false;

    private bool isSlidingOut = false;

    

    void Update()
    {
        if (isSliding)
        {
            kitchenPanel.anchoredPosition = Vector2.Lerp(
                kitchenPanel.anchoredPosition,
                visiblePosition,
                Time.deltaTime * slideSpeed
            );

            // 거의 도달하면 고정시키기
            if (Vector2.Distance(kitchenPanel.anchoredPosition, visiblePosition) < 0.1f)
            {
                kitchenPanel.anchoredPosition = visiblePosition;
                isSliding = false;
            }
        }
        if (isSlidingOut)
        {
            kitchenPanel.anchoredPosition = Vector2.Lerp(
                kitchenPanel.anchoredPosition,
                hiddenPosition,
                Time.deltaTime * slideSpeed
            );

            if (Vector2.Distance(kitchenPanel.anchoredPosition, hiddenPosition) < 0.1f)
            {
                kitchenPanel.anchoredPosition = hiddenPosition;
                isSlidingOut = false;

                // 원하면 패널 끄기
                // kitchenPanel.gameObject.SetActive(false);
            }
        }

    }

    public void SlideIn()
    {
        kitchenPanel.gameObject.SetActive(true); // 꺼져있었다면 켬
        isSliding = true;

        isSlidingOut = false; //반대 동작 중지
    }

    public void SlideOut()
    {
        isSlidingOut = true;
        isSliding = false;
    }
}
