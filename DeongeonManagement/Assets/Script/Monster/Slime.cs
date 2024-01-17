using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    protected override void Initialize()
    {
        SetStatus("Slime", 10, 1);
    }


}
