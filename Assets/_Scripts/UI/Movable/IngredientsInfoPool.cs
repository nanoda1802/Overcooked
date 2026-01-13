using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientsInfoPool : ObjPool<IngredientsInfo>
{
    protected override IngredientsInfo CreateObj()
    {
        IngredientsInfo obj = base.CreateObj();
        obj.Init();
        return obj;
    }

    protected override void OnGot(IngredientsInfo obj)
    {
        obj.DeactivateImages();
        base.OnGot(obj);
    }

    protected override void OnReleased(IngredientsInfo obj)
    {
        obj.InitInfos();
        base.OnReleased(obj);
    }
}
