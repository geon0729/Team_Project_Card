using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IceCreamBuilder : MonoBehaviour
{

    public List<string> selectedScoops = new List<string>();
    public string selectedConeType = "";

    private Customer customerSystem;

    [Header("스쿱 관련 오브젝트")]
    public Transform scoopSpawnPoint; // 스쿱이 쌓이는 위치 기준점
    public GameObject[] scoopPrefabs; // 각 맛별 스쿱 프리팹 (초코, 바닐라 등)

    private int currentScoopCount = 0; // 쌓인 스쿱 개수

    

    public GameObject scoopPrefab; // UI 이미지 프리팹
    public Transform[] scoopSpawnPoints; // 컵 위 3개 위치
     
    public Transform scoopParent; // 캔버스 안, 스쿱들이 올라갈 UI 오브젝트

    private int scoopCount = 0;



    public void SetCone(string coneType)
    {
        selectedConeType = coneType;
        Debug.Log("선택한 콘: " + coneType);
    }

    public void AddScoop(string flavor)
    {
        if (scoopCount >= scoopSpawnPoints.Length)
        {
            Debug.LogWarning("스쿱 3개 이상 추가 불가");
            return;
        }

        GameObject scoop = Instantiate(scoopPrefab, scoopParent);
        scoop.transform.position = scoopSpawnPoints[scoopCount].position;

        Image img = scoop.GetComponent<Image>();
        if (img != null)
            img.sprite = GetSpriteForFlavor(flavor);

        selectedScoops.Add(flavor);
        scoopCount++;
    }

    public void SubmitIceCream()
    {
        IceCreamOrder order = customerSystem.GetCurrentOrder();

        bool coneMatch = order.coneType == selectedConeType; // 컵 vs 콘 비교 (지금은 컵만 쓰면 true로 고정 가능)
        int matchCount = order.scoopFlavors.Intersect(selectedScoops).Count();

        bool isSatisfied = matchCount >= 3;

        customerSystem.OnIceCreamFinished(
            isSatisfied,
            order,
            new List<string>(selectedScoops)
        );

        ClearCup(); // 컵 비우기

        //아이스크림 UI 패널 닫기
        if (iceCreamUIPanel != null)
            iceCreamUIPanel.SetActive(false);
    }

    [SerializeField] private GameObject iceCreamUIPanel;


    public void SetCustomerSystem(Customer cs)
    {
        customerSystem = cs;
    }

    private Sprite GetSpriteForFlavor(string flavor)
    {
        // 맛에 따라 다른 스프라이트 반환 (미리 세팅해둬야 함)
        switch (flavor.ToLower())
        {
            case "vanilla": return vanillaSprite;
            case "chocolate": return chocolateSprite;
            case "strawberry": return strawberrySprite;
            default: return defaultScoopSprite;
        }
    }

    private void ClearCup()
    {
        // 오직 스쿱들만 삭제
        List<Transform> toRemove = new List<Transform>();

        foreach (Transform child in scoopParent)
        {
            if (!child.name.StartsWith("ScoopPoint")) //스폰포인트 제외
                toRemove.Add(child);
        }

        foreach (Transform child in toRemove)
        {
            Destroy(child.gameObject);
        }

        selectedScoops.Clear();
        scoopCount = 0;
    }






    public Sprite vanillaSprite;
    public Sprite chocolateSprite;
    public Sprite strawberrySprite;
    public Sprite defaultScoopSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
