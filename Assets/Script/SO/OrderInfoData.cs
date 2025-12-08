using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "OrderData", menuName = "SO/Stage/OrderInfo")]
public class OrderInfoData : ScriptableObject
{
    [SF] private int maxOrderCount;
    [SF] private MenuData[] availableMenu;
    [SF] private float newOrderInterval;
    
    public int MaxOrderCount => maxOrderCount;
    public MenuData[] AvailableMenu => availableMenu;
    public float NewOrderInterval => newOrderInterval;
}
