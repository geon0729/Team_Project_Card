using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderManager : MonoBehaviour
{
    public TextMeshProUGUI orderText;

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

            string orderLine = string.Format(template, flavor, topping);

            fullOrder += orderLine;

            if (i < numberOfOrders - 1)
                fullOrder += "\n";
        }

        orderText.text = fullOrder;
    }
}