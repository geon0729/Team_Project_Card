using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class Customer : MonoBehaviour
{
    // 손님 대사 표시용
    public TextMeshProUGUI dialogueText;
    public GameObject orderPanel;    // 주문 UI 전체
    public Button acceptButton;      // 수락 버튼
    public Button rejectButton;      // 거절 버튼
    public Image characterImageUI;

    private int currentDay = 1;           // 현재 날짜
    private int customersPerDay = 5;      // 오늘 받을 손님 수
    private int customersServed = 0;      // 오늘 받은 손님 수

    private IceCreamOrder currentOrder;

    public IceCreamOrder[] orders;   // 손님 주문 목록
    private string dialogue;

    private List<IceCreamOrder> todayCustomerList = new List<IceCreamOrder>();
    private int customerIndex = 0;

    [SerializeField] private GameObject daySummaryPanel; // 요약 패널
    [SerializeField] private TextMeshProUGUI goodCountText;
    [SerializeField] private TextMeshProUGUI normalCountText;
    [SerializeField] private TextMeshProUGUI badCountText;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Button retryButton;

    private int goodCount = 0;
    private int normalCount = 0;
    private int badCount = 0;

    void Start()
    {
        StartNewDay(); // 첫날 시작

    }

    void StartNewDay()
    {
        // 일차 텍스트 업데이트
        if (dayText != null)
            dayText.text = $"{currentDay}일차";

        // 손님 초기화 등 기존 로직
        todayCustomerList = orders.OrderBy(x => Random.value).Take(customersPerDay).ToList();
        customerIndex = 0;
        ShowNextCustomer();
    }

    [SerializeField] private TMP_Text dayText;



    void ShowNextCustomer()
    {

        if (customerIndex >= todayCustomerList.Count)
            return;

        currentOrder = todayCustomerList[customerIndex];
        customerIndex++;


        characterImageUI.sprite = currentOrder.characterImage;
        characterImageUI.gameObject.SetActive(true);
        characterImageUI.color = Color.white;
        StartCoroutine(FadeInImage(characterImageUI, 0.5f));

        dialogueText.text = currentOrder.dialogue;
        orderPanel.SetActive(true);

        // 버튼 이벤트 초기화
        acceptButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();

        // 버튼 기능 연결
        acceptButton.onClick.AddListener(AcceptOrder);
        rejectButton.onClick.AddListener(RejectOrder);
    }



    void AcceptOrder()
    {
        orderPanel.SetActive(false);
        Debug.Log("주문 수락: " + currentOrder.customerName);

        // 아이스크림 만들기 UI 켜기
        iceCreamUIPanel.SetActive(true);

        // IceCreamBuilder에 현재 손님 시스템 전달
        iceCreamBuilder.SetCustomerSystem(this);


    }

    void RejectOrder()
    {
        orderPanel.SetActive(false);
        Debug.Log("주문 거절: " + currentOrder.customerName);

        StartCoroutine(FadeOutImage(characterImageUI, 0.5f));

        customersServed++; //직접 증가시킴

        if (customersServed >= customersPerDay)
        {
            Debug.Log("하루 종료!");
            ShowDaySummary(); //패널 보여주기
            currentDay++;
            customersPerDay = 5 + (currentDay - 1);
            customersServed = 0;

            Invoke(nameof(StartNewDay), 2f);
        }
        else
        {
            Invoke(nameof(ShowNextCustomer), 2f);
        }
    }

    void StartIceCreamMaking()
    {
        // 예시로 3초 뒤에 아이스크림이 완성됐다고 가정
        StartCoroutine(FinishMakingIceCreamAfterDelay(3f));
    }

    IEnumerator FinishMakingIceCreamAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("아이스크림 제작 완료!");
        CheckNextCustomer(); // 제작이 끝난 뒤에 다음 손님
    }

    void CheckNextCustomer()
    {
        customersServed++;

        if (customersServed >= customersPerDay)
        {
            Debug.Log("하루 종료!");

            ShowDaySummary(); //하루 결과만 보여주고 버튼 누를 때까지 대기!
        }
        else
        {
            Invoke(nameof(ShowNextCustomer), 2f);
        }
    }



    IEnumerator FadeOutImage(Image img, float duration)
    {
        float elapsed = 0f;
        Color originalColor = img.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            img.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        img.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        img.gameObject.SetActive(false);
    }

    IEnumerator FadeInImage(Image img, float duration)
    {
        float elapsed = 0f;
        Color originalColor = img.color;

        // 시작은 투명
        img.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        img.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            img.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        img.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }

    public void OnIceCreamFinished(bool isSatisfied, IceCreamOrder order, List<string> playerScoops)
    {
        //손님 요구 맛 복사본 만들기 (중복 확인 위해)
        List<string> requiredFlavors = order.scoopFlavors
            .Select(f => f.Trim().ToLower())
            .ToList();

        List<string> playerFlavors = playerScoops
            .Select(f => f.Trim().ToLower())
            .ToList();

        int matchCount = 0;

        foreach (string flavor in playerFlavors)
        {
            if (requiredFlavors.Contains(flavor))
            {
                matchCount++;
                requiredFlavors.Remove(flavor); //중복 방지를 위해 하나씩 제거
            }
        }

        //평가
        if (matchCount >= 3)
            goodCount++;
        else if (matchCount == 2)
            normalCount++;
        else
            badCount++;

        //디버깅
        Debug.Log("손님 주문: " + string.Join(", ", order.scoopFlavors));
        Debug.Log("플레이어 제출: " + string.Join(", ", playerScoops));
        Debug.Log("일치 수: " + matchCount);
        Debug.Log("손님 평가 결과: " + (matchCount >= 3 ? "좋음" : matchCount == 2 ? "보통" : "나쁨"));

        //연출
        StartCoroutine(FadeOutImage(characterImageUI, 0.5f));
        Invoke(nameof(CheckNextCustomer), 2f);
    }
    public IceCreamOrder GetCurrentOrder()
    {
        return currentOrder;
    }

    
    [SerializeField] private IceCreamBuilder iceCreamBuilder;
    [SerializeField] private GameObject iceCreamUIPanel; // 아이스크림 만들기 UI 패널

    void ShowDaySummary()
    {
        daySummaryPanel.SetActive(true);

        goodCountText.text = $"good: {goodCount}";
        normalCountText.text = $"normal: {normalCount}";
        badCountText.text = $"bad: {badCount}";

        if (goodCount >= 3)
        {
            nextDayButton.gameObject.SetActive(true);
            retryButton.gameObject.SetActive(false);
        }
        else
        {
            nextDayButton.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(true);
        }

        nextDayButton.onClick.RemoveAllListeners();
        nextDayButton.onClick.AddListener(() =>
        {
            daySummaryPanel.SetActive(false);
            currentDay++;
            customersPerDay = 5 + (currentDay - 1);
            customersServed = 0;
            goodCount = normalCount = badCount = 0;
            StartNewDay();
        });

        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(() =>
        {
            daySummaryPanel.SetActive(false);
            customersServed = 0;
            goodCount = normalCount = badCount = 0;
            StartNewDay();
        });
    }


    







    // Update is called once per frame
    void Update()
    {
        
    }
}
