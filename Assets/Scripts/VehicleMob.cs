using System;
using UnityEngine;

public class VehicleMob : Mob
{
    // 차량은 entryPoint와 endPoint를 짝지어둠
    // endPoint에 도착하면 pool에 복귀 (Mob)
    // 각 entryPoint는 일정 시간 마다 차량을 생성 (MobManager)
    // 생성된 차량은 짝에 맞는 endPoint로 향함 (Mob)
    // 각각 속도나 모델 같은 거 좀 조절해서 변주 (VehicleMob)
    // 만약 이동 도중 전방에 뭔가가 감지되면 멈춤 (행인, 플레이어, 다른 차량) (VehicleMob -> triggerEnter)
    // 감지 카운트를 하든, 감지 목록을 관리하든 혀 (VehicleMob)
    // 감지됐던 대상이 전방에서 사라지면 기록 제거 (VehicleMob -> triggerExit)
    // 감지되는 게 아예 없어지면 다시 이동 (VehicleMob)

    private int _detectedMobCount;

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Peds") || other.CompareTag("Player"))
    //     {
    //         _detectedMobCount += 1;
    //         Debug.Log("Trig Enter " + _detectedMobCount);
    //     }
    //     if (agent.isStopped) return;
    //     
    //     StartCoroutine(CoSmoothBreak());
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Peds") || other.CompareTag("Player"))
    //     {
    //         _detectedMobCount -= 1;
    //         Debug.Log("Trig Exit " + _detectedMobCount);
    //     }
    //     if (!agent.isStopped) return;
    //     if (_detectedMobCount >= 1) return;
    //
    //     StartCoroutine(CoSmoothAccelerate());
    // }
}
