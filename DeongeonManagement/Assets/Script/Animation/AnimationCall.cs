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

    public void Projectile_Jab()
    {
        //Debug.Log("anim_Cilp_Call");
        SoundManager.Instance.PlaySound("SFX/Battle_npc");
        GetComponentInParent<BattleField>().Projectile_Launch();
    }
    public void Projectile_Shot() //? 플레이어가 얘를 써서 따로 해줘야함
    {
        if (name.Contains("Player"))
        {
            Call_Mash();
            return;
        }

        SoundManager.Instance.PlaySound("SFX/Battle_npc");
        GetComponentInParent<BattleField>().Projectile_Launch();
    }

    public void Projectile_Shot_Right()
    {
        //Debug.Log("anim_Cilp_Call");
        SoundManager.Instance.PlaySound("SFX/Battle_npc");
        GetComponentInParent<BattleField>().Projectile_Launch_Right();
    }


    public void Monster_Call()
    {
        SoundManager.Instance.PlaySound("SFX/Battle_monster");
        GetComponentInParent<BattleField>().Call_Mash();
    }



}
