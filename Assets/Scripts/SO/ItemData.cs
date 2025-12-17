using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public enum ItemType { Bun, Cabbage, Cheese, Patty, Tomato, Plate }
public enum ItemStatus { Undone, WellDone, Overdone }

[CreateAssetMenu(fileName = "ItemData", menuName = "SO/Item")]
public class ItemData : ScriptableObject
{
    [Header("[ Type ]")]
    [SF] private ItemType itemType;
    public ItemType ItemType => itemType;
    
    [Header("[ Doneness ]")]
    [SF] private ItemStatus initialDoneness;
    [SF] private ItemStatus maxDoneness;
    [SF] private float maxProgress;
    [SF] private Material[] mats;
    public ItemStatus InitialDoneness => initialDoneness;
    public ItemStatus MaxDoneness => maxDoneness;
    public float MaxProgress => maxProgress;
    public Material[] Mats => mats;
    
    [Header("[ Throw Values ]")]
    [SF, Range(1f, 20f)] private float throwForce;
    [SF] [Range(0f,1f)] private float throwDamp;
    [SF] [Range(1f,30f)] private float maxThrowDistance;
    public float ThrowForce => throwForce;
    public float ThrowDamp => throwDamp;
    public float MaxThrowDistance => maxThrowDistance;

    [Header("[Plate Only]")]
    [SF] [Range(0f,1f)] private float ingredientOffsetY;
    [SF] private GameObject ingredientPrefab;
    public float IngredientOffsetY => ingredientOffsetY;
    public GameObject IngredientPrefab => ingredientPrefab;
    
    // 미리 준비해둔...
    public void InitValues(ItemType itemType, ItemStatus initialDoneness, ItemStatus maxDoneness, float maxProgress, Material[] mats, float throwDamp, float maxThrowDistance, float ingredientOffsetY = 0, GameObject ingredientPrefab = null)
    {
        this.itemType = itemType;
        this.initialDoneness = initialDoneness;
        this.maxDoneness = maxDoneness;
        this.maxProgress = maxProgress;
        this.mats = mats;
        this.throwDamp = throwDamp;
        this.maxThrowDistance = maxThrowDistance;
        this.ingredientOffsetY = ingredientOffsetY;
        this.ingredientPrefab = ingredientPrefab;
    }
}
