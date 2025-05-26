using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class IceCreamOrder
{
    public string customerName;      // 손님 이름
    public string dialogue;          // 손님 대사
    public string[] requiredFlavors; // 주문한 아이스크림 맛
    public Sprite characterImage;    //손님 이미지

    public string coneType;      // 예: "Cone" or "Cup"
    public string[] scoopFlavors;

}
