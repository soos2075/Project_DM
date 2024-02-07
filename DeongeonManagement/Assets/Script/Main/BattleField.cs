using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleField : MonoBehaviour
{

    void Start()
    {
        //BattlePlay();
        sprite_BG = GetComponentInChildren<SpriteRenderer>();
    }


    SpriteRenderer sprite_BG;

    public Transform pos_Left;
    public Transform pos_Right;

    GameObject obj_Left;
    GameObject obj_Right;

    Animator ani_npc;
    Animator ani_monster;

    NPC npc;
    Monster monster;

    public Coroutine BattlePlay()
    {
        if (roundList == null || roundList.Count == 0)
        {
            Debug.Log("싸울 데이터가 없음");
            return null;
        }
        else
        {
            return StartCoroutine(BattleAnimation());
        }
    }

    IEnumerator BattleAnimation()
    {
        for (int i = 0; i < roundList.Count; i++)
        {
            if (roundList[i].attacker == Define.PlacementType.Monster)
            {
                ani_monster.CrossFade(Define.ANIM_attack, 0.1f);
                yield return new WaitForSecondsRealtime(0.1f); //? crossFade 시간 동안은 hash값이 바뀌지 않으므로 그만큼은 기다려줘야함
                //Debug.Log(ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash + $"##");

                AddAction(roundList[i].damage, pos_Left);
                yield return new WaitUntil(() => ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_idle);
                yield return new WaitForSecondsRealtime(0.2f);
            }

            if (roundList[i].attacker == Define.PlacementType.NPC)
            {
                ani_npc.CrossFade(Define.ANIM_attack, 0.1f);
                yield return new WaitForSecondsRealtime(0.1f);

                AddAction(roundList[i].damage, pos_Right);
                yield return new WaitUntil(() => ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_idle);
                yield return new WaitForSecondsRealtime(0.2f);
            }
        }

        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log("재생종료");

    }

    #region ForAnimationVoid

    public void Call_Mash()
    {
        Call_Damage?.Invoke();
    }

    Action Call_Damage;
    public void AddAction(int _dam, Transform parent)
    {
        Call_Damage = () =>
        {
            var pos = parent.GetChild(0);
            pos.localPosition = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(0.25f, 0.5f), 0);

            var mesh = Managers.Resource.Instantiate("Battle/DamageMesh", pos);
            mesh.transform.position = pos.transform.position;

            mesh.GetComponent<TextMeshPro>().sortingOrder = (sprite_BG.sortingOrder + 1);
            mesh.GetComponent<TextMeshPro>().text = _dam.ToString();
        };
    }


    #endregion




    public class Round
    {
        public Define.PlacementType attacker;
        public int damage;

        public Round(Define.PlacementType _attacker, int _damage)
        {
            attacker = _attacker;
            damage = _damage;
        }
    }

    public enum BattleResult
    {
        Nothing,
        Monster_Die,
        NPC_Die,
    }

    public BattleResult Battle(NPC npc, Monster monster)
    {
        obj_Left = Managers.Resource.Instantiate($"Battle/{npc.name}_Avatar", pos_Left);
        obj_Left.GetComponent<SpriteRenderer>().sortingOrder = 11;
        ani_npc = obj_Left.GetComponent<Animator>();
        this.npc = npc;


        obj_Right = Managers.Resource.Instantiate($"Battle/{monster.name}_Avatar", pos_Right);
        obj_Right.GetComponentInChildren<SpriteRenderer>().sortingOrder = 11;
        ani_monster = obj_Right.GetComponentInChildren<Animator>();
        this.monster = monster;


        if (!npc || !monster)
        {
            Debug.Log("전투할 데이터가 없음");
            return BattleResult.Nothing;
        }

        BattleStart();
        return result;
    }

    List<Round> roundList;
    List<Round> BattleStart()
    {
        roundList = new List<Round>();
        int agi = npc.AGI - monster.AGI;

        if (agi > 5)
        {
            //? 플레이어 선공 / 3방때림
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
        }
        if (agi > 0)
        {
            //? 플레이어 선공 / 2방때림
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
        }

        if (agi < -5)
        {
            //? 몬스터 선공 / 3방때림
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
        }

        if (agi <= 0)
        {
            //? 몬스터 선공 / 2방때림
            if(MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
        }


        return roundList;
    }
    BattleResult result;


    bool MonsterAttack()
    {
        int damage = 1;
        if (TryDodge(monster.LUK, npc.LUK))
        {
            Debug.Log("회피함");
            damage = 0;
        }
        else
        {
            damage = Mathf.Clamp((monster.ATK - npc.DEF), 1, monster.ATK);
        }
        
        roundList.Add(new Round(Define.PlacementType.Monster, damage));

        npc.HP -= damage;
        if (npc.HP <= 0)
        {
            result = BattleResult.NPC_Die;
            return true;
        }
        return false;
    }
    bool NPCAttack()
    {
        int damage = 1;
        if (TryDodge(npc.LUK, monster.LUK))
        {
            Debug.Log("회피함");
            damage = 0;
        }
        else
        {
            damage = Mathf.Clamp((npc.ATK - monster.DEF), 1, npc.ATK);
        }

        roundList.Add(new Round(Define.PlacementType.NPC, damage));

        monster.HP -= damage;
        if (monster.HP <= 0)
        {
            result = BattleResult.Monster_Die;
            return true;
        }
        return false;
    }


    bool TryDodge(int attacker, int defender) //? 회피확률. 최소 10%에 1차이날수록 10%씩 증가, 최대 90%
    {
        int chance = Mathf.Clamp((defender - attacker), 1, 9);
        int dice = UnityEngine.Random.Range(0, 10);

        return chance > dice ? true : false;
    }



}
