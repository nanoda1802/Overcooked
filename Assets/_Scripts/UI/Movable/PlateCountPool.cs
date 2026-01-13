using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCountPool : ObjPool<PlateCount>
{
    protected override PlateCount CreateObj()
    {
        PlateCount obj = base.CreateObj();
        obj.Init();
        return obj;
    }
}
