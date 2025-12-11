using Cinemachine;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Eatery : MonoBehaviour
{
    [SF] private CinemachineVirtualCamera vCam;
    [SF] private Transform marker;
    public Transform Marker => marker;

    public void SetVCamPriority(int priority)
    {
        vCam.Priority = priority;
    }
}
