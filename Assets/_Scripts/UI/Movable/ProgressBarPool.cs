using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarPool : ObjPool<ProgressBar>
{
    protected override ProgressBar CreateObj()
    {
        ProgressBar obj = base.CreateObj();
        obj.Init();
        return obj;
    }

    protected override void OnGot(ProgressBar obj)
    {
        obj.InitFillAmount();
        base.OnGot(obj);
    }
}
