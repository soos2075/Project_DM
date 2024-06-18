using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.U2D.Animation;

public class BattleField : MonoBehaviour
{
    public int sort = 10;
    void Start()
    {
        //BattlePlay();
        sprite_BG.sortingOrder = sort;
        sprite_border.sortingOrder = sort;

        //Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        hpBar_npc.sortingOrder = sort + 1;
        hpBar_monster.sortingOrder = sort + 1;

        GetComponentInChildren<SpriteMask>().frontSortingOrder = sort;
    }

    [Header("Field Sprite")]
    public Sprite field_1;
    public Sprite field_2;
    public Sprite field_3;

    [Space(10)]


    public SpriteRenderer sprite_BG;
    public SpriteRenderer sprite_border;

    public Transform pos_Left;
    public Transform pos_Right;

    GameObject obj_Left;
    GameObject obj_Right;

    Animator ani_npc;
    Animator ani_monster;

    NPC npc;
    Monster monster;

    public SpriteRenderer hpBar_npc;
    public SpriteRenderer hpBar_monster;

    int leftHPMax;
    int rightHPMax;
    int leftCurrent;
    int rightCurrent;
    public void SetHPBar(int _leftHP, int _leftHPMax, int _rightHP, int _rightHPMax)
    {
        leftCurrent = _leftHP;
        leftHPMax = _leftHPMax;
        rightCurrent = _rightHP;
        rightHPMax = _rightHPMax;

        ChangeHPValue(0, 0);
    }
    void ChangeHPValue(int _leftChange, int _rightChange)
    {
        leftCurrent -= _leftChange;
        rightCurrent -= _rightChange;

        float leftValue = Mathf.Clamp01((float)leftCurrent / (float)leftHPMax);
        hpBar_npc.transform.localScale = new Vector3(leftValue, 1, 1);

        float rightValue = Mathf.Clamp01((float)rightCurrent / (float)rightHPMax);
        hpBar_monster.transform.localScale = new Vector3(rightValue, 1, 1);
    }

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
        bool isOver = false;

        for (int i = 0; i < roundList.Count; i++)
        {
            yield return UserData.Instance.Wait_GamePlay;

            if (roundList[i].attacker == PlacementType.Monster)
            {
                AddFlashWhite(obj_Left.GetComponentInChildren<SpriteRenderer>());
                AddHPBar(roundList[i].damage, 0);
                ani_monster.CrossFade(Define.ANIM_Attack, 0.1f);
                yield return new WaitForSeconds(0.1f); //? crossFade 시간 동안은 hash값이 바뀌지 않으므로 그만큼은 기다려줘야함

                if (roundList[i].roundResult == BattleResult.NPC_Die)
                {
                    AddAction(roundList[i].damage, pos_Left, ani_npc);

                    yield return new WaitUntil(() => ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle);

                    yield return new WaitForSeconds(0.5f);
                    Main.Instance.ShowDM(npc.Rank * 2, Main.TextType.exp, pos_Right.GetChild(0), 1);

                    //yield return new WaitForSeconds(0.5f);
                    //Main.Instance.ShowDM(npc.KillGold, Main.TextType.gold, pos_Right.GetChild(0), 1);

                    yield return new WaitForSeconds(0.5f);
                    isOver = true;
                    break;
                }
                else
                {
                    AddAction(roundList[i].damage, pos_Left);
                }
                //Debug.Log(ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash + $"##");
                yield return new WaitUntil(() => ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle);
                yield return new WaitForSeconds(0.5f);
            }

            if (roundList[i].attacker == PlacementType.NPC)
            {
                AddFlashWhite(obj_Right.GetComponentInChildren<SpriteRenderer>());
                AddHPBar(0, roundList[i].damage);
                NPC_AttackAnim();
                yield return new WaitForSeconds(0.1f); //? crossFade 시간 동안은 hash값이 바뀌지 않으므로 그만큼은 기다려줘야함

                if (roundList[i].roundResult == BattleResult.Monster_Die)
                {
                    AddAction(roundList[i].damage, pos_Right, ani_monster);

                    yield return new WaitUntil(() => ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle);
                    yield return new WaitForSeconds(0.5f);
                    isOver = true;
                    break;
                }
                else
                {
                    AddAction(roundList[i].damage, pos_Right);
                }

                yield return new WaitUntil(() => ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle);
                yield return new WaitForSeconds(0.5f);
            }
        }

        if (!isOver)
        {
            yield return new WaitForSeconds(0.5f);
            Main.Instance.ShowDM(npc.Rank, Main.TextType.exp, pos_Right.GetChild(0), 1);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
        yield return UserData.Instance.Wait_GamePlay;
        Debug.Log("재생종료");
    }

    #region ForAnimationVoid

    void NPC_AttackAnim()
    {
        Type npcType = npc.GetType();
        if (npcType == typeof(Elf))
        {
            ani_npc.CrossFade(Define.ANIM_Shot, 0.1f);
        }
        else if(npcType == typeof(Wizard))
        {
            ani_npc.CrossFade(Define.ANIM_Jab, 0.1f);
        }
        else
        {
            ani_npc.CrossFade(Define.ANIM_Attack, 0.1f);
        }
    }

    public void Projectile_Launch()
    {
        var projectile = Managers.Resource.Instantiate("Battle/Projectile", pos_Left);
        projectile.transform.position = pos_Left.transform.position;
        projectile.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 2;

        Type npcType = npc.GetType();
        if (npcType == typeof(Elf))
        {
            projectile.GetComponentInChildren<SpriteResolver>().SetCategoryAndLabel("Elf", "ElfA");
        }
        else if (npcType == typeof(Wizard))
        {
            projectile.GetComponentInChildren<SpriteResolver>().SetCategoryAndLabel("Wizard", "WizardA");
        }

        StartCoroutine(Shotting(projectile));
    }
    IEnumerator Shotting(GameObject _projectile)
    {
        for (int i = 0; i < 5; i++)
        {
            float interval = 0.05f;
            _projectile.transform.position += Vector3.right * interval * 4;
            yield return new WaitForSeconds(interval);
        }
        //float timer = 0;
        //while (timer < 0.5f)
        //{
        //    yield return null;
        //    timer += Time.deltaTime;

        //    _projectile.transform.Translate(Vector3.right * Time.deltaTime * 4);
        //}
        _projectile.SetActive(false);
        Call_Mash();
    }


    public void Call_Mash()
    {
        Call_Damage?.Invoke();
    }

    Action Call_Damage;
    public DamageNumber damageMesh;
    public void AddAction(int _dam, Transform parent)
    {
        Call_Damage += () =>
        {
            var pos = parent.GetChild(0);
            if (_dam == 0)
            {
                DamageNumber damageNumber = damageMesh.Spawn(pos.transform.position, "miss");
            }
            else
            {
                DamageNumber damageNumber = damageMesh.Spawn(pos.transform.position, _dam);
            }
        };
    }
    public void AddAction(int _dam, Transform parent, Animator deadAnim)
    {
        AddAction(_dam, parent);
        Call_Damage += () => deadAnim.Play(Define.ANIM_Dead);
    }
    public void AddFlashWhite(SpriteRenderer renderer)
    {
        Call_Damage = () => StartCoroutine(FlashWhite(renderer));
    }
    void AddHPBar(int _left, int _right)
    {
        Call_Damage += () => ChangeHPValue(_left, _right);
    }

    IEnumerator FlashWhite(SpriteRenderer renderer)
    {
        float flashValue = 0;
        while (flashValue <= 0.25f)
        {
            yield return null;
            flashValue += Time.unscaledDeltaTime;
            renderer.material.SetFloat("_FlashIntensity", flashValue * 7f);
        }

        //Debug.Log("반짝");
        while (flashValue > 0)
        {
            yield return null;
            flashValue -= Time.unscaledDeltaTime;
            renderer.material.SetFloat("_FlashIntensity", flashValue * 7f);
        }
        renderer.material.SetFloat("_FlashIntensity", 0);
    }

    [Obsolete]
    public void _AddAction(int _dam, Transform parent) //? 데미지프로안쓰는버전
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
        public PlacementType attacker;
        public int damage;

        public BattleResult roundResult;


        public Round(PlacementType _attacker, int _damage, BattleResult _result)
        {
            attacker = _attacker;
            damage = _damage;
            roundResult = _result;
        }
    }

    public enum BattleResult
    {
        Nothing,
        Monster_Die,
        NPC_Die,
    }

    public BattleResult Battle(NPC npc, Monster monster, Material mat_npc, Material mat_monster)
    {
        obj_Left = Instantiate(npc.gameObject, pos_Left);
        Destroy(obj_Left.GetComponent<NPC>());
        obj_Left.transform.localPosition = Vector3.zero;
        obj_Left.transform.localScale = Vector3.one;
        obj_Left.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 1;
        obj_Left.GetComponentInChildren<SpriteRenderer>().material = mat_npc;
        ani_npc = obj_Left.GetComponent<Animator>();
        this.npc = npc;


        obj_Right = Instantiate(monster.gameObject, pos_Right);
        Destroy(obj_Right.GetComponent<Monster>());
        obj_Right.transform.localPosition = Vector3.zero;
        obj_Right.transform.localScale = new Vector3(-1, 1, 1);
        obj_Right.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 1;
        obj_Right.GetComponentInChildren<SpriteRenderer>().material = mat_monster;
        ani_monster = obj_Right.GetComponent<Animator>();
        this.monster = monster;


        //obj_Right = Managers.Resource.Instantiate($"Battle/{monster.name}_Avatar", pos_Right);
        //obj_Right.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort;
        //ani_monster = obj_Right.GetComponentInChildren<Animator>();



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
        int agi = npc.AGI - monster.AGI_Final;

        if (agi > 5)
        {
            //? 플레이어 선공 / 3방때림
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
        }
        else if (agi > 0)
        {
            //? 플레이어 선공 / 2방때림
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
        }
        else if (agi < -5)
        {
            //? 몬스터 선공 / 3방때림
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
            if (NPCAttack()) return roundList;
            if (MonsterAttack()) return roundList;
        }
        else if (agi <= 0)
        {
            //? 몬스터 선공 / 2방때림
            if (MonsterAttack()) return roundList;
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
        if (TryDodge(monster.LUK_Final, npc.LUK))
        {
            //Debug.Log("회피함");
            damage = 0;
        }
        else
        {
            int atkRange = (int)UnityEngine.Random.Range(monster.ATK_Final * 0.8f, monster.ATK_Final * 1.2f);

            damage = Mathf.Clamp((atkRange - npc.DEF), 1, atkRange);
        }
        

        npc.HP -= damage;
        if (npc.HP <= 0)
        {
            result = BattleResult.NPC_Die;
            roundList.Add(new Round(PlacementType.Monster, damage, result));
            return true;
        }

        roundList.Add(new Round(PlacementType.Monster, damage, BattleResult.Nothing));
        return false;
    }
    bool NPCAttack()
    {
        int damage = 1;
        if (TryDodge(npc.LUK, monster.LUK_Final))
        {
            //Debug.Log("회피함");
            damage = 0;
        }
        else
        {
            int atkRange = (int)UnityEngine.Random.Range(npc.ATK * 0.8f, npc.ATK * 1.2f);
            damage = Mathf.Clamp((atkRange - monster.DEF_Final), 1, atkRange);
        }

        monster.HP -= damage;
        if (monster.HP <= 0)
        {
            monster.HP = 0;
            result = BattleResult.Monster_Die;
            roundList.Add(new Round(PlacementType.NPC, damage, result));
            return true;
        }

        roundList.Add(new Round(PlacementType.NPC, damage, BattleResult.Nothing));
        return false;
    }


    bool TryDodge(int attacker, int defender) //? 회피확률. 최소 5%에 LUK가 1차이날수록 5%씩 증가, 최대 90%
    {

        int chance = Mathf.Clamp((defender - attacker), 1, 18);
        int dice = UnityEngine.Random.Range(0, 20);

        return chance > dice ? true : false;
    }



}
