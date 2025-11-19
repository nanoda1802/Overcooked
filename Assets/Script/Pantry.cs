using System;
using UnityEngine;
using SF =  UnityEngine.SerializeField;

public class Pantry : MonoBehaviour, IBox
{
    [SF] private ItemType type;
    [SF] private Transform cover;
    [SF] private GameObject[] items;
    
    private void Awake()
    {
        cover.GetChild((int)type).gameObject.SetActive(true);
    }

    public void Interact()
    {
        Debug.Log($"Interact with Pantry {type}");
        // 일단 Instantiate로... 추후 pool로 변경
        Instantiate(items[(int)type], cover.position, Quaternion.identity);
    }
}

public interface IBox
{
    public void Interact();
}