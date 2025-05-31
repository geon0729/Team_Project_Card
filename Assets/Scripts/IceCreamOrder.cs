using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class IceCreamOrder
{
    public string flavor;
    public string topping;

    public IceCreamOrder(string flavor, string topping)
    {
        this.flavor = flavor;
        this.topping = topping;
    }
}
