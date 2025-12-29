using Cinemachine;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Eatery : MonoBehaviour
{
    [SF] private StageInfoData stageInfo;
    [SF] private StageSelectPopUp popUpUI;
    
    [SF] private CinemachineVirtualCamera vCam;
    [SF] private Transform marker;
    public Transform Marker => marker;

    public bool IsDummyEatery()
    {
        return stageInfo is null;
    }

    public void SetVCamPriority(int priority)
    {
        vCam.Priority = priority;
    }

    public Vector3 GetVCamPos()
    {
        return vCam.transform.position;
    }

    public void ActivatePopUpUI(OutStagePlayerController player)
    {
        popUpUI.Activate(player, stageInfo);
        // popUpUI.SetDisplayInfos(stageInfo);
        // popUpUI.SubscribeEvents(player);
        // popUpUI.gameObject.SetActive(true);
    }
    
    public void DeactivatePopUpUI(OutStagePlayerController player)
    {
        popUpUI.Deactivate(player);
    }
}
