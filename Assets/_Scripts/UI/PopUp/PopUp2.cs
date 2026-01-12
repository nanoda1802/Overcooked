using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum PopUpType
{
    Tutorial, StageCue, StagePause, StageResult
}

public class PopUp2 : MonoBehaviour
{
    private PopUpUIManager _popUpUIManager;
    public PopUpType Type { get; protected set; }
    protected Sequence popSeq;
    
    public event Action OnPopUp;
    public event Action OnPopDown;
    
    public virtual void Init(PopUpUIManager popUpUIManager)
    {
        _popUpUIManager = popUpUIManager;
    }

    public virtual void PopUp()
    {
        OnPopUp?.Invoke();
    }

    public virtual void PopDown()
    {
        OnPopDown?.Invoke();
    }
}
