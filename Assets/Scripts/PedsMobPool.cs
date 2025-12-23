public class PedsMobPool : ObjPool<PedsMob>
{
    protected override PedsMob Create()
    {
        PedsMob mob = base.Create();
        mob.SetReleaseEvent(m => Pool.Release(m as PedsMob));
        return mob;
    }

    protected override void OnGet(PedsMob obj)
    {
        // 풀에서 꺼낼 때 해줘야할 것
        // 시작점으로 이동시키고 방향 잘 바꿔주기 (시작점의 forward하면 되지 않을까?)
        // 도착점 지정해주기
        base.OnGet(obj); // 활성화
    }

    protected override void OnRelease(PedsMob obj)
    {
        base.OnRelease(obj); // 비활성화
        // 풀로 넣을 때 해줄 초기화
    }
}
