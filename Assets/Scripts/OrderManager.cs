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

    public int currentDay = 1;
    private int maxDays = 5;

    private int[] customersPerDay = { 3, 5, 7, 9, 12 };
    private float[] timeLimits = { 60f, 60f, 60f, 70f, 85f };

    private float dayTimer;
    private bool isTiming = false;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI remainingCustomerText;

    public GameObject summaryPanel;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI summaryText;
    public Button nextDayButton;
    public Button retryButton;

    private int badCount = 0;

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

    private bool isWaitingForReaction = false;

    public bool IsWaitingForReaction => isWaitingForReaction;


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
        LoadProgress();

        acceptButton.onClick.AddListener(AcceptOrder);
        rejectButton.onClick.AddListener(() => StartCoroutine(HandleRejectOrder()));
        StartDay();
        summaryPanel.SetActive(false);
        retryButton.gameObject.SetActive(false);
        nextDayButton.interactable = true;
    }
    void Update()
    {
        if (isTiming)
        {
            dayTimer -= Time.deltaTime;

            if (dayTimer <= 0f)
            {
                dayTimer = 0f;
                isTiming = false;
                EndDay();
            }

            UpdateTimerUI();
        }
    }
    public void SetReactionWaiting(bool state)
    {
        isWaitingForReaction = state;
        acceptButton.interactable = !state;
        rejectButton.interactable = !state;
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
        acceptButton.interactable = false;
        rejectButton.interactable = false;

        StartCoroutine(ShowNextOrderWithDelay());

    }

    private void AcceptOrder()
    {
        Debug.Log($"주문 수락됨: {lastOrderMessage}");

        acceptButton.interactable = false;
        rejectButton.interactable = false;
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

        UpdateTimerUI(); // 남은 손님 수 갱신

        if (currentOrderIndex < numberOfOrders)
        {
            ShowNextOrder();
        }
        else
        {
            isTiming = false;
            EndDay();
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
    public void StartDay()
    {
        summaryPanel.SetActive(false);
        currentOrderIndex = 0;
        numberOfOrders = customersPerDay[currentDay - 1];
        dayTimer = timeLimits[currentDay - 1];
        isTiming = true;

        UpdateDayUI();
        GenerateInitialOrders();
        ShowNextOrder();
    }
    private void EndDay()
    {
        isTiming = false;
        acceptButton.interactable = false;
        rejectButton.interactable = false;

        summaryPanel.SetActive(true);

        bool isFail = false;

        // 실패 조건 체크
        if (badCount >= numberOfOrders / 2 || currentOrderIndex < numberOfOrders)
        {
            isFail = true;
        }

        if (isFail)
        {
            resultTitleText.text = $"<color=red>Day {currentDay} 실패</color>";
            retryButton.gameObject.SetActive(true);
            nextDayButton.interactable = false;

            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() =>
            {
                summaryPanel.SetActive(false);
                retryButton.gameObject.SetActive(false);
                badCount = 0;
                SaveProgress();
                StartDay(); // 같은 일차 다시 시작
            });
        }
        else
        {
            resultTitleText.text = $"<color=green>Day {currentDay} 완료</color>";
            nextDayButton.interactable = true;
            retryButton.gameObject.SetActive(false);

            nextDayButton.onClick.RemoveAllListeners();
            nextDayButton.onClick.AddListener(() =>
            {
                summaryPanel.SetActive(false);
                currentDay++;
                badCount = 0;
                SaveProgress();
                StartDay();
            });
        }

        // 결과 요약 텍스트
        summaryText.text = $"총 손님 수: {numberOfOrders}\n" +
                           $"완료한 손님: {currentOrderIndex}\n" +
                           $"Bad 평가 수: {badCount}\n" +
                           $"남은 시간: {Mathf.CeilToInt(dayTimer)}초";
    }
    void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"남은 시간: {Mathf.CeilToInt(dayTimer)}초";

        if (remainingCustomerText != null)
            remainingCustomerText.text = $"남은 손님: {numberOfOrders - currentOrderIndex}명";
    }

    void UpdateDayUI()
    {
        if (dayText != null)
            dayText.text = $"Day {currentDay}";
    }
    public void IncreaseBadCount()
    {
        badCount++;
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("SavedDay", currentDay);
        PlayerPrefs.Save();
        Debug.Log("게임 저장됨: Day " + currentDay);
    }

    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey("SavedDay"))
        {
            currentDay = PlayerPrefs.GetInt("SavedDay");
            Debug.Log("저장된 게임 불러옴: Day " + currentDay);
        }
        else
        {
            currentDay = 1; // 저장된 게 없으면 1일차부터
            Debug.Log("저장된 데이터 없음. Day 1부터 시작.");
        }
    }
    private IEnumerator ShowNextOrderWithDelay()
    {
        orderText.text = "";

        yield return new WaitForSeconds(0.05f);

        if (currentOrderIndex < generatedOrders.Count)
        {
            currentOrder = generatedOrders[currentOrderIndex];
            lastOrderMessage = string.Format(
                orderTemplates[Random.Range(0, orderTemplates.Length)],
                currentOrder.flavor, currentOrder.topping);

            orderText.text = lastOrderMessage;

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
}