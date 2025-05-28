using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCall : MonoBehaviour
{
    public BattleField field;
    public GameObject attacker;
    public GameObject defender;

    public bool isBattlePhase;
    public void Init_Battle(BattleField _field, GameObject _attacker, GameObject _defender)
    {
        field = _field;
        attacker = _attacker;
        defender = _defender;

        isBattlePhase = true;
    }


    //? ��� Attack �ִϸ��̼ǿ��� ȣ���� �̺�Ʈ �Լ� (NPC, ���� ���� ����)
    //? ��� Attack Type�� Normal�϶��� �ڵ�ó��, �װ� �ƴϸ� �������� ȭ��Ʈ�÷�������� ����
    public void EventCall_Attack()
    {
        if (isBattlePhase)
        {
            field.Event_Attack(gameObject);
        }
    }


    //? �÷���ȭ��Ʈ ȿ����
    public void EventCall_FlashWhite()
    {
        if (isBattlePhase)
        {
            field.Event_FlashWhite(defender);
        }
    }

    //? ���� �������� ��� �̺�Ʈ �Լ�
    public void EventCall_DamageNumber()
    {
        if (isBattlePhase)
        {
            field.Event_ShowDamageNumber();
        }
    }

    public void EventCall_FlashAndDamage()
    {
        if (isBattlePhase)
        {
            field.Event_FlashWhite(defender);
            field.Event_ShowDamageNumber();
        }
    }



    public void Self_Disable()
    {
        gameObject.SetActive(false);
    }



    //IEnumerator FlashWhite(SpriteRenderer renderer)
    //{
    //    float flashValue = 0;
    //    while (flashValue <= 0.25f)
    //    {
    //        yield return null;
    //        flashValue += Time.unscaledDeltaTime;
    //        renderer.material.SetFloat("_FlashIntensity", flashValue * 7f);
    //    }

    //    //Debug.Log("��¦");
    //    while (flashValue > 0)
    //    {
    //        yield return null;
    //        flashValue -= Time.unscaledDeltaTime;
    //        renderer.material.SetFloat("_FlashIntensity", flashValue * 7f);
    //    }
    //    renderer.material.SetFloat("_FlashIntensity", 0);
    //}




    //public void Call_Mash()
    //{
    //    //Debug.Log("anim_Cilp_Call");
    //    SoundManager.Instance.PlaySound("SFX/Battle_npc");

    //    if (GetComponentInParent<BattleField>())
    //    {
    //        GetComponentInParent<BattleField>().Call_Mash();
    //    }
    //}

    //public void Projectile_Jab()
    //{
    //    //Debug.Log("anim_Cilp_Call");
    //    SoundManager.Instance.PlaySound("SFX/Battle_npc");
    //    //GetComponentInParent<BattleField>().Projectile_Launch();
    //}
    //public void Projectile_Shot() //? �÷��̾ �긦 �Ἥ ���� �������
    //{
    //    if (name.Contains("Player"))
    //    {
    //        Call_Mash();
    //        return;
    //    }

    //    SoundManager.Instance.PlaySound("SFX/Battle_npc");
    //    //GetComponentInParent<BattleField>().Projectile_Launch();
    //}

    //public void Projectile_Shot_Right()
    //{
    //    //Debug.Log("anim_Cilp_Call");
    //    SoundManager.Instance.PlaySound("SFX/Battle_npc");
    //    //GetComponentInParent<BattleField>().Projectile_Launch_Right();
    //}


    //public void Monster_Call()
    //{
    //    SoundManager.Instance.PlaySound("SFX/Battle_monster");
    //    GetComponentInParent<BattleField>().Call_Mash();
    //}



}

public enum AttackType
{
    Normal,
    Projectile,
    Skill,
}
