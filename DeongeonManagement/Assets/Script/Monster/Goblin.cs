using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    protected override void Initialize()
    {
        SetStatus("Goblin", 15, 1);
    }


}
