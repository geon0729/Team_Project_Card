using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public TextMeshProUGUI orderText;
    public Button acceptButton;
    public Button rejectButton;

    private string[] iceCreamFlavors = { "바닐라", "초콜릿", "녹차", "딸기", "바나나" };
    private string[] toppings = { "시리얼", "스프링클" };

    private string[] orderTemplates = {
        "맛은 {0}으로 주시고, 토핑은 {1}으로 주세요",
        "{0} 맛 아이스크림에 {1} 토핑 부탁해요",
        "전 {1} 얹은 {0} 아이스크림을 좋아해요",
        "{0} 맛 아이스크림 하나에 {1} 추가해주세요",
        "{0}으로 주시고요, {1} 토핑도 넣어주세요",
        "야, {0}, {1}으로 줘 "
    };

    public int numberOfOrders = 3;
    private int currentOrderIndex = 0;
    private List<string> generatedOrders = new List<string>();

    void Start()
    {
        acceptButton.onClick.AddListener(AcceptOrder);
        rejectButton.onClick.AddListener(() => StartCoroutine(HandleRejectOrder()));

        GenerateInitialOrders();
        ShowNextOrder();
    }

    private void GenerateInitialOrders()
    {
        generatedOrders.Clear();

        for (int i = 0; i < numberOfOrders; i++)
        {
            generatedOrders.Add(GenerateRandomOrder());
        }
    }

    private string GenerateRandomOrder()
    {
        string flavor = iceCreamFlavors[Random.Range(0, iceCreamFlavors.Length)];
        string topping = toppings[Random.Range(0, toppings.Length)];
        string template = orderTemplates[Random.Range(0, orderTemplates.Length)];

        return string.Format(template, flavor, topping);
    }

    private void ShowNextOrder()
    {
        if (currentOrderIndex < generatedOrders.Count)
        {
            orderText.text = generatedOrders[currentOrderIndex];
        }
        else
        {
            orderText.text = "모든 주문이 끝났습니다!";
            acceptButton.interactable = false;
            rejectButton.interactable = false;
        }
    }

    private void AcceptOrder()
    {
        Debug.Log($"주문 수락됨: {generatedOrders[currentOrderIndex]}");

        // 여기서 제작 화면으로 이동하는 처리
        // 예: UI 전환 또는 다른 씬 이동 (지금은 로그만)
        orderText.text = "아이스크림 제작 창으로 이동 중...";
    }

    private IEnumerator HandleRejectOrder()
    {
        Debug.Log($"주문 거절됨: {generatedOrders[currentOrderIndex]}");

        // 버튼 비활성화
        acceptButton.interactable = false;
        rejectButton.interactable = false;

        // 기존 텍스트 지우기
        orderText.text = "";

        // 0.1초 기다려서 텍스트 깜빡임 줄이기 (선택사항)
        yield return new WaitForSeconds(0.1f);

        // 거절 메시지 표시
        orderText.text = "주문이 거절되었습니다.";

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 현재 주문을 새로 생성해서 교체
        generatedOrders[currentOrderIndex] = GenerateRandomOrder();

        // 새 주문 표시
        ShowNextOrder();

        // 버튼 다시 활성화
        acceptButton.interactable = true;
        rejectButton.interactable = true;
    }
}