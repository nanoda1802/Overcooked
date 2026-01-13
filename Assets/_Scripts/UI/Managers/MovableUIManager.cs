using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class MovableUIManager : MonoBehaviour
{
    public ObjectPool<ProgressBar> ProgressBarPool { get; private set; }
    public ObjectPool<PlateCount> PlateCountPool { get; private set; }
    public ObjectPool<IngredientsInfo> IngredientsInfoPool { get; private set; }

    private void Awake() // [임시] StageManager -> CanvasManger 거쳐 Init되도록...
    {
        Init();
    }

    public void Init()
    {
        ProgressBarPool = GetComponent<ProgressBarPool>().InitPool();
        PlateCountPool = GetComponent<PlateCountPool>().InitPool();
        IngredientsInfoPool = GetComponent<IngredientsInfoPool>().InitPool();
    }
}
