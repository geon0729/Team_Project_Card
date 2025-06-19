using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class IceCreamBuilder : MonoBehaviour
{
    [Header("토핑 관련")]
    public Sprite cerealSprite;
    public Sprite sprinkleSprite;
    public Sprite defaultToppingSprite;

    public GameObject toppingPrefab; // 이미지 프리팹
    public Transform toppingSpawnPoint; // 토핑 위치 기준
    private GameObject currentToppingImage; // 현재 생성된 토핑 이미지
    public List<string> selectedScoops = new List<string>();
    public string selectedTopping = "";

    private OrderManager orderManager;

    public GameObject scoopPrefab;
    public Transform[] scoopSpawnPoints;
    public Transform scoopParent;

    public GameObject speechBubble;
    public TextMeshProUGUI speechText;

    public TextMeshProUGUI iceCreamTimerText;


    [SerializeField] private GameObject iceCreamUIPanel;

    public Sprite vanillaSprite;
    public Sprite chocolateSprite;
    public Sprite bananaSprite;
    public Sprite greenteaSprite;
    public Sprite strawberrySprite;

    public Sprite defaultScoopSprite;

    void Start()
    {
        orderManager = FindObjectOfType<OrderManager>();
    }
    void Update()
    {
        if (orderManager != null && iceCreamTimerText != null)
        {
            // 남은 시간을 정수초 단위로 표시
            iceCreamTimerText.text = $"남은 시간: {Mathf.CeilToInt(orderManager.GetRemainingTime())}초";
        }
    }

    public void AddScoop(string flavor)
    {
        foreach (Transform child in scoopParent)
        {
            if (!child.name.StartsWith("ScoopPoint"))
                Destroy(child.gameObject);
        }

        GameObject scoop = Instantiate(scoopPrefab, scoopParent);
        scoop.transform.position = scoopSpawnPoints[0].position; // 항상 첫 번째 위치에 생성

        RectTransform targetRT = scoopSpawnPoints[0].GetComponent<RectTransform>();
        RectTransform scoopRT = scoop.GetComponent<RectTransform>();

        if (targetRT != null && scoopRT != null)
        {
            scoopRT.sizeDelta = targetRT.rect.size; 
            scoopRT.localScale = Vector3.one;   
            scoopRT.anchorMin = new Vector2(0.5f, 0.5f);
            scoopRT.anchorMax = new Vector2(0.5f, 0.5f);
            scoopRT.pivot = new Vector2(0.5f, 0.5f);
        }

        Image img = scoop.GetComponent<Image>();
        if (img != null)
            img.sprite = GetSpriteForFlavor(flavor);

        selectedScoops.Clear();        // 기존 선택 비우고
        selectedScoops.Add(flavor);
    }

    public void SetTopping(string topping)
    {
        selectedTopping = topping;
        Debug.Log("선택한 토핑: " + topping);

        
        if (currentToppingImage != null)
            Destroy(currentToppingImage);

        
        currentToppingImage = Instantiate(toppingPrefab, scoopParent); 
        currentToppingImage.transform.position = toppingSpawnPoint.position;

       
        RectTransform targetRT = toppingSpawnPoint.GetComponent<RectTransform>();
        RectTransform toppingRT = currentToppingImage.GetComponent<RectTransform>();

        if (targetRT != null && toppingRT != null)
        {
            toppingRT.sizeDelta = targetRT.rect.size;   
            toppingRT.localScale = Vector3.one;         
            toppingRT.anchorMin = new Vector2(0.5f, 0.5f); 
            toppingRT.anchorMax = new Vector2(0.5f, 0.5f);
            toppingRT.pivot = new Vector2(0.5f, 0.5f);
        }

        // 이미지 설정
        Image img = currentToppingImage.GetComponent<Image>();
        if (img != null)
            img.sprite = GetSpriteForTopping(topping);
    }

    public void SubmitIceCream()
    {
        IceCreamOrder order = orderManager.currentOrder;

        bool flavorMatch = selectedScoops.Contains(order.flavor);
        bool toppingMatch = selectedTopping == order.topping;

        string result;
        if (flavorMatch && toppingMatch)
            result = "good";
        else if (flavorMatch || toppingMatch)
            result = "normal";
        else
            result = "bad";

        if (result == "bad")
        {
            orderManager.IncreaseBadCount();
        }

        Debug.Log($"맛 일치: {flavorMatch}, 토핑 일치: {toppingMatch}, 결과: {result}");

        
        orderManager.SetReactionWaiting(true);

        ShowSpeechBubble(result);

        if (iceCreamUIPanel != null)
            iceCreamUIPanel.SetActive(false);

        ClearCup();
    }

    private void ClearCup()
    {
        foreach (Transform child in scoopParent)
        {
            if (!child.name.StartsWith("ScoopPoint"))
                Destroy(child.gameObject);
        }

        if (currentToppingImage != null)
            Destroy(currentToppingImage);

        selectedScoops.Clear();
        selectedTopping = "";
    }

    private Sprite GetSpriteForFlavor(string flavor)
    {
        switch (flavor)
        {
            case "바닐라": return vanillaSprite;
            case "초콜릿": return chocolateSprite;
            case "바나나": return bananaSprite;
            case "녹차": return greenteaSprite;
            case "딸기": return strawberrySprite;
            default: return defaultScoopSprite;
        }
    }
    private Sprite GetSpriteForTopping(string topping)
    {
        switch (topping)
        {
            case "시리얼": return cerealSprite;
            case "스프링클": return sprinkleSprite;
            default: return defaultToppingSprite;
        }
    }

    private void ShowSpeechBubble(string result)
    {
        if (speechBubble != null && speechText != null)
        {
            string message = "";

            switch (result)
            {
                case "good":
                    message = goodMessages[Random.Range(0, goodMessages.Length)];
                    break;
                case "normal":
                    message = normalMessages[Random.Range(0, normalMessages.Length)];
                    break;
                case "bad":
                    message = badMessages[Random.Range(0, badMessages.Length)];
                    break;
            }

            speechText.text = message;
            speechBubble.SetActive(true);

            // 일정 시간 후 숨기기
            StartCoroutine(HideSpeechBubbleAfterDelay(1.5f));
        }
    }

    private string[] goodMessages = {
    "정말 맛있어요!",
    "완벽해요!",
    "이거 제가 좋아하는 맛이에요!",
    "다음에도 이걸로 부탁해요!",
    "다음에 또 올게요!",
    "이건 진짜 최고예요!"
    };

    private string[] normalMessages = {
    "그럭저럭 괜찮네요.",
    "음… 나쁘진 않아요.",
    "조금 아쉬워요.",
    "괜찮지만 특별하진 않네요."
};
    private string[] badMessages = {
    "이건 좀 별로예요...",
    "내가 만들어도 이것보단 잘만들겠다!.",
    "이거 맞아요?.",
    "이건 아닌 것 같아요…"
};


    private IEnumerator HideSpeechBubbleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speechBubble.SetActive(false);

        speechBubble.SetActive(false);

        orderManager.SetReactionWaiting(false);
        orderManager.NextOrderAfterSubmit();
    }

}
