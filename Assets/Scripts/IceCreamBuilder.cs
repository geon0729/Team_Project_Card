using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public void AddScoop(string flavor)
    {
        foreach (Transform child in scoopParent)
        {
            if (!child.name.StartsWith("ScoopPoint")) // ScoopPoint는 유지
                Destroy(child.gameObject);
        }

        GameObject scoop = Instantiate(scoopPrefab, scoopParent);
        scoop.transform.position = scoopSpawnPoints[0].position; // 항상 첫 번째 위치에 생성

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

        // 이전 토핑 제거
        if (currentToppingImage != null)
            Destroy(currentToppingImage);

        // 새로운 토핑 생성
        currentToppingImage = Instantiate(toppingPrefab, scoopParent); // scoopParent에 붙이기
        currentToppingImage.transform.position = toppingSpawnPoint.position;

        Image img = currentToppingImage.GetComponent<Image>();
        if (img != null)
            img.sprite = GetSpriteForTopping(topping);
    }

    public void SubmitIceCream()
    {
        IceCreamOrder order = orderManager.currentOrder;

        bool flavorMatch = selectedScoops.Contains(order.flavor);
        bool toppingMatch = selectedTopping == order.topping;

        bool isSatisfied = flavorMatch && toppingMatch;

        Debug.Log($"맛 일치: {flavorMatch}, 토핑 일치: {toppingMatch}, 만족 여부: {isSatisfied}");

        ClearCup();

        if (iceCreamUIPanel != null)
            iceCreamUIPanel.SetActive(false);
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

}
