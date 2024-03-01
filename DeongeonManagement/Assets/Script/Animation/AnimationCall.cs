using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCall : MonoBehaviour
{

    public void Call_Mash()
    {
        //Debug.Log("anim_Cilp_Call");
        SoundManager.Instance.PlaySound("SFX/Battle_npc");
        GetComponentInParent<BattleField>().Call_Mash();
    }


    public void Monster_Call()
    {
        SoundManager.Instance.PlaySound("SFX/Battle_monster");
        GetComponentInParent<BattleField>().Call_Mash();
    }

}
