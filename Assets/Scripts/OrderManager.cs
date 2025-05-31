using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderManager : MonoBehaviour
{
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI orderPanelText;
    public GameObject orderPanelUI;


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

    public int numberOfOrders = 1;
    private string lastOrderMessage;

    public static OrderManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public IceCreamOrder currentOrder { get; private set; }

    void Start()
    {
        GenerateOrders();
    }

    private void GenerateOrders()
    {
        string fullOrder = "";


        for (int i = 0; i < numberOfOrders; i++)
        {
            string flavor = iceCreamFlavors[Random.Range(0, iceCreamFlavors.Length)];
            string topping = toppings[Random.Range(0, toppings.Length)];
            string template = orderTemplates[Random.Range(0, orderTemplates.Length)];
            currentOrder = new IceCreamOrder(flavor, topping);

            string orderLine = string.Format(template, flavor, topping);
            orderText.text = orderLine;
            lastOrderMessage = orderLine;

            fullOrder += orderLine;

            if (i < numberOfOrders - 1)
                fullOrder += "\n";


        }

        orderText.text = fullOrder;

    }

    public void ShowOrderPanel()
    {
        string msg = OrderManager.Instance.GetLastOrderMessage();

        if (string.IsNullOrEmpty(msg))
        {
            Debug.LogWarning("주문 메시지가 없습니다!");
            return;
        }

        orderPanelText.text = msg;
        orderPanelUI.SetActive(true);
    }

    public string GetLastOrderMessage()
    {
        return lastOrderMessage;
    }
}