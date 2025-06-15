using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI orderPanelText;
    public GameObject orderPanelUI;
    public Button acceptButton;
    public Button rejectButton;

    public GameObject customerVisual;
    public Image customerImageUI;
    public Sprite[] customerSprites;

    public static OrderManager Instance { get; private set; }

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
    private List<IceCreamOrder> generatedOrders = new List<IceCreamOrder>();
    private string lastOrderMessage;

    public IceCreamOrder currentOrder { get; private set; }

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
            string flavor = iceCreamFlavors[Random.Range(0, iceCreamFlavors.Length)];
            string topping = toppings[Random.Range(0, toppings.Length)];
            string template = orderTemplates[Random.Range(0, orderTemplates.Length)];
            string message = string.Format(template, flavor, topping);

            IceCreamOrder order = new IceCreamOrder(flavor, topping);
            generatedOrders.Add(order);

            if (i == 0)
                lastOrderMessage = message;
        }
    }

    private void ShowNextOrder()
    {
        if (currentOrderIndex < generatedOrders.Count)
        {
            currentOrder = generatedOrders[currentOrderIndex];

            string message = string.Format(orderTemplates[Random.Range(0, orderTemplates.Length)],
                                           currentOrder.flavor, currentOrder.topping);

            lastOrderMessage = message;
            orderText.text = message;

            
            customerVisual.SetActive(true);

            
            if (customerSprites.Length > 0 && customerImageUI != null)
            {
                customerImageUI.sprite = customerSprites[Random.Range(0, customerSprites.Length)];
            }
            acceptButton.interactable = true;
            rejectButton.interactable = true;
        }
        else
        {
            orderText.text = "모든 주문이 끝났습니다!";
            acceptButton.interactable = false;
            rejectButton.interactable = false;

            customerVisual.SetActive(false); 
        }
        

    }

    private void AcceptOrder()
    {
        Debug.Log($"주문 수락됨: {lastOrderMessage}");

        orderText.text = "아이스크림 제작 창으로 이동 중...";
        
    }

    private IEnumerator HandleRejectOrder()
    {
        Debug.Log($"주문 거절됨: {lastOrderMessage}");

        acceptButton.interactable = false;
        rejectButton.interactable = false;
        orderText.text = "";

        customerVisual.SetActive(false);

        yield return new WaitForSeconds(0.1f);
        orderText.text = "주문이 거절되었습니다.";
        yield return new WaitForSeconds(2f);

        string flavor = iceCreamFlavors[Random.Range(0, iceCreamFlavors.Length)];
        string topping = toppings[Random.Range(0, toppings.Length)];
        string message = string.Format(orderTemplates[Random.Range(0, orderTemplates.Length)], flavor, topping);

        IceCreamOrder newOrder = new IceCreamOrder(flavor, topping);
        generatedOrders[currentOrderIndex] = newOrder;
        lastOrderMessage = message;

        ShowNextOrder();

        acceptButton.interactable = true;
        rejectButton.interactable = true;
    }

    public void ShowOrderPanel()
    {
        if (string.IsNullOrEmpty(lastOrderMessage))
        {
            Debug.LogWarning("주문 메시지가 없습니다!");
            return;
        }

        orderPanelText.text = lastOrderMessage;
        orderPanelUI.SetActive(true);
    }

    public string GetLastOrderMessage()
    {
        return lastOrderMessage;
    }

    public void NextOrderAfterSubmit()
    {
        currentOrderIndex++;

        if (currentOrderIndex < numberOfOrders)
        {
            ShowNextOrder();
        }
        else
        {
            orderText.text = "모든 주문이 끝났습니다!";
            acceptButton.interactable = false;
            rejectButton.interactable = false;
        }
    }

    public void GenerateNewOrder()
    {
        string flavor = iceCreamFlavors[Random.Range(0, iceCreamFlavors.Length)];
        string topping = toppings[Random.Range(0, toppings.Length)];
        string message = string.Format(orderTemplates[Random.Range(0, orderTemplates.Length)], flavor, topping);

        IceCreamOrder newOrder = new IceCreamOrder(flavor, topping);
        currentOrder = newOrder;
        lastOrderMessage = message;

        orderText.text = message;

        if (customerSprites.Length > 0 && customerImageUI != null)
        {
            customerImageUI.sprite = customerSprites[Random.Range(0, customerSprites.Length)];
        }

        Debug.Log("새로운 주문 생성됨: " + message);
    }
}