using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using SF = UnityEngine.SerializeField;



public interface IPopUp
{
    
    public Sequence PopSeq { get; protected set; }

    public void PopUp();
    public void PopDown();
}

public class PopUpUIManager : MonoBehaviour
{
    private StageManager _stageManager;
    private CanvasManager _canvasManager;

    private PopUpPanel _bg;
    
    private PopUp2 _curPopUp;
    private Dictionary<PopUpType, PopUp2> _popUps;


    public void ActivatePopUp(PopUpType type, bool hasBgClicked = false)
    {
        if (hasBgClicked) _bg.OnClicked += DeactivatePopUp;
        _curPopUp = _popUps[type];
        _curPopUp.PopUp();
    }

    public void DeactivatePopUp()
    {
        _curPopUp?.PopDown();
        _curPopUp = null;
        _bg.OnClicked -= DeactivatePopUp;
    }

    public void ChangePopUp(PopUpType type)
    {
        DeactivatePopUp();
        ActivatePopUp(type);
    }
}
