using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class IceCreamBuilder : MonoBehaviour
{
    [Header("���� ����")]
    public Sprite cerealSprite;
    public Sprite sprinkleSprite;
    public Sprite defaultToppingSprite;

    public GameObject toppingPrefab; // �̹��� ������
    public Transform toppingSpawnPoint; // ���� ��ġ ����
    private GameObject currentToppingImage; // ���� ������ ���� �̹���
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
            // ���� �ð��� ������ ������ ǥ��
            iceCreamTimerText.text = $"���� �ð�: {Mathf.CeilToInt(orderManager.GetRemainingTime())}��";
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
        scoop.transform.position = scoopSpawnPoints[0].position; // �׻� ù ��° ��ġ�� ����

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

        selectedScoops.Clear();        // ���� ���� ����
        selectedScoops.Add(flavor);
    }

    public void SetTopping(string topping)
    {
        selectedTopping = topping;
        Debug.Log("������ ����: " + topping);

        
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

        // �̹��� ����
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

        Debug.Log($"�� ��ġ: {flavorMatch}, ���� ��ġ: {toppingMatch}, ���: {result}");

        
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
            case "�ٴҶ�": return vanillaSprite;
            case "���ݸ�": return chocolateSprite;
            case "�ٳ���": return bananaSprite;
            case "����": return greenteaSprite;
            case "����": return strawberrySprite;
            default: return defaultScoopSprite;
        }
    }
    private Sprite GetSpriteForTopping(string topping)
    {
        switch (topping)
        {
            case "�ø���": return cerealSprite;
            case "������Ŭ": return sprinkleSprite;
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

            // ���� �ð� �� �����
            StartCoroutine(HideSpeechBubbleAfterDelay(1.5f));
        }
    }

    private string[] goodMessages = {
    "���� ���־��!",
    "�Ϻ��ؿ�!",
    "�̰� ���� �����ϴ� ���̿���!",
    "�������� �̰ɷ� ��Ź�ؿ�!",
    "������ �� �ðԿ�!",
    "�̰� ��¥ �ְ���!"
    };

    private string[] normalMessages = {
    "�׷����� �����׿�.",
    "���� ������ �ʾƿ�.",
    "���� �ƽ�����.",
    "�������� Ư������ �ʳ׿�."
};
    private string[] badMessages = {
    "�̰� �� ���ο���...",
    "���� ���� �̰ͺ��� �߸���ڴ�!.",
    "�̰� �¾ƿ�?.",
    "�̰� �ƴ� �� ���ƿ䡦"
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
