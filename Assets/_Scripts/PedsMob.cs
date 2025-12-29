public class PedsMob : Mob
{
    // 행인은 entryPoint와 endPoint 간 구분이 없음
    // 목적지인 point에 도착하면 pool에 복귀 (Mob)
    // pool은 일정 시간 마다 랜덤한 point에 행인을 생성 (MobManager)
    // 생성 포인트 제외한 나머지 중 임의의 point를 목적지로 설정 (MobManager)
    // 생성된 행인은 지정된 목적지로 이동 (Mob)
    // 행인은 가끔씩 자리에 서서 특정 동작을 하곤 함 (PedsMob)
}
