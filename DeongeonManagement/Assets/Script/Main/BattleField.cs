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

        if (openingList.Count > 0)
        {
            for (int i = 0; i < openingList.Count; i++)
            {
                yield return UserData.Instance.Wait_GamePlay;
                yield return new WaitForSeconds(0.2f);

                if (openingList[i].target is Monster)
                {
                    switch (openingList[i].effectType)
                    {
                        case EffectType.Damaged:
                            InstantAction(openingList[i].value, pos_Right, DamageMeshType.Damage);
                            ChangeHPValue(0, openingList[i].value);
                            break;

                        case EffectType.Heal:
                            InstantAction(openingList[i].value, pos_Right, DamageMeshType.Heal);
                            ChangeHPValue(0, -openingList[i].value);
                            break;
                    }
                }
                else if (openingList[i].target is NPC)
                {
                    switch (openingList[i].effectType)
                    {
                        case EffectType.Damaged:
                            InstantAction(openingList[i].value, pos_Left, DamageMeshType.Damage);
                            ChangeHPValue(openingList[i].value, 0);
                            break;

                        case EffectType.Heal:
                            InstantAction(openingList[i].value, pos_Left, DamageMeshType.Heal);
                            ChangeHPValue(-openingList[i].value, 0);
                            break;
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }
        }



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
                    //AddAction(roundList[i].damage, pos_Left, ani_npc);
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction(item.Item1, pos_Left, ani_npc, item.Item2);
                    }

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
                    //AddAction(roundList[i].damage, pos_Left);
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction(item.Item1, pos_Left, item.Item2);
                    }
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
                    //AddAction(roundList[i].damage, pos_Right, ani_monster);
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction(item.Item1, pos_Right, ani_monster, item.Item2);
                    }

                    yield return new WaitUntil(() => ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle);
                    yield return new WaitForSeconds(0.5f);
                    isOver = true;
                    break;
                }
                else
                {
                    //AddAction(roundList[i].damage, pos_Right);
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction(item.Item1, pos_Right, item.Item2);
                    }
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
    public DamageNumber specialMesh;
    public DamageNumber criticalMesh;
    public DamageNumber healMesh;

    public enum DamageMeshType
    {
        Damage,
        Special,
        Critical,
        Heal,
    }

    public void InstantAction(int _dam, Transform parent, DamageMeshType meshType = DamageMeshType.Damage)
    {
        var pos = parent.GetChild(0);
        switch (meshType)
        {
            case DamageMeshType.Damage:
                if (_dam == 0)
                {
                    DamageNumber dn = damageMesh.Spawn(pos.transform.position, "miss");
                }
                else
                {
                    DamageNumber dn = damageMesh.Spawn(pos.transform.position, _dam);
                }
                break;

            case DamageMeshType.Special:
                DamageNumber dn1 = specialMesh.Spawn(pos.transform.position, _dam);
                break;

            case DamageMeshType.Critical:
                DamageNumber dn2 = criticalMesh.Spawn(pos.transform.position, _dam);
                break;

            case DamageMeshType.Heal:
                DamageNumber dn3 = healMesh.Spawn(pos.transform.position, _dam);
                break;
        }
    }

    public void AddAction(int _dam, Transform parent, DamageMeshType meshType = DamageMeshType.Damage)
    {
        Call_Damage += () =>
        {
            var pos = parent.GetChild(0);
            switch (meshType)
            {
                case DamageMeshType.Damage:
                    if (_dam == 0)
                    {
                        DamageNumber damageNumber = damageMesh.Spawn(pos.transform.position, "miss");
                    }
                    else
                    {
                        DamageNumber damageNumber = damageMesh.Spawn(pos.transform.position, _dam);
                    }
                    break;

                case DamageMeshType.Special:
                    DamageNumber damageNumber1 = specialMesh.Spawn(pos.transform.position, _dam);
                    break;

                case DamageMeshType.Critical:
                    DamageNumber damageNumber2 = criticalMesh.Spawn(pos.transform.position, _dam);
                    break;

                case DamageMeshType.Heal:
                    DamageNumber damageNumber3 = healMesh.Spawn(pos.transform.position, _dam);
                    break;
            }
        };
    }
    public void AddAction(int _dam, Transform parent, Animator deadAnim, DamageMeshType meshType = DamageMeshType.Damage)
    {
        AddAction(_dam, parent, meshType);
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

    List<OpeningTrait> openingList = new List<OpeningTrait>();

    public class OpeningTrait
    {
        public EffectType effectType;

        public I_BattleStat target;

        public StatType statType;

        public int value;

        public OpeningTrait(EffectType _effect, I_BattleStat _target, StatType _stat, int _value)
        {
            effectType = _effect;
            target = _target;
            statType = _stat;
            value = _value;
        }
    }

    public enum StatType
    {
        HP = 0,
        HP_MAX = 1,
        ATK = 2,
        DEF = 3,
        AGI = 4,
        LUK = 5,
    }
    public enum EffectType
    {
        Damaged,
        Heal,
    }



    public class Round
    {
        public PlacementType attacker;

        public int damage;
        public List<(int, DamageMeshType)> damage_show;

        public BattleResult roundResult;


        public Round(PlacementType _attacker, int _damage, List<(int, DamageMeshType)> _damageList, BattleResult _result)
        {
            attacker = _attacker;

            damage = _damage;
            damage_show = _damageList;

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

        if (monster.TraitCheck(TraitGroup.Vitality))
        {
            int bonusHP = monster.GetSomething(TraitGroup.Vitality, monster.B_HP_Max);
            int applyHP = monster.B_HP + bonusHP;

            int realValue = applyHP > monster.B_HP_Max ? (monster.B_HP_Max - monster.B_HP) : bonusHP;
            monster.HP += realValue;
            openingList.Add(new OpeningTrait(EffectType.Heal, monster, StatType.HP, realValue));
        }

        if (monster.TraitCheck(TraitGroup.Overwhelm))
        {
            int realValue = Mathf.RoundToInt(npc.HP * 0.2f);
            npc.HP -= realValue;
            openingList.Add(new OpeningTrait(EffectType.Damaged, npc, StatType.HP, realValue));
        }



        int agi = npc.B_AGI - monster.B_AGI;
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
        List<(int, DamageMeshType)> damageList = new List<(int, DamageMeshType)>();

        int normalAttackDamage = 1;
        if (TryDodge(monster.B_LUK, npc.LUK))
        {
            //Debug.Log("회피함");
            normalAttackDamage = 0;
        }
        else
        {
            int atkRange = (int)UnityEngine.Random.Range(monster.B_ATK * 0.8f, monster.B_ATK * 1.2f);

            normalAttackDamage = Mathf.Clamp((atkRange - npc.DEF), 1, atkRange);
        }


        damageList.Add((normalAttackDamage, DamageMeshType.Damage));

        //? 님블 = 0.5배 추가공격
        if (monster.TraitCheck(TraitGroup.Nimble))
        {
            int bonusAttack = normalAttackDamage / 2;
            damageList.Add((bonusAttack, DamageMeshType.Damage));
        }


        if (monster.TraitCheck(TraitGroup.IronSkin))
        {
            int trueDamage = monster.GetSomething(TraitGroup.IronSkin, monster.B_DEF);
            damageList.Add((trueDamage, DamageMeshType.Special));
        }


        int damage_Sum = 0;
        foreach (var item in damageList)
        {
            damage_Sum += item.Item1;
        }


        npc.HP -= damage_Sum;
        if (npc.HP <= 0)
        {
            result = BattleResult.NPC_Die;
            roundList.Add(new Round(PlacementType.Monster, damage_Sum, damageList, result));
            return true;
        }

        roundList.Add(new Round(PlacementType.Monster, damage_Sum, damageList, BattleResult.Nothing));
        return false;
    }
    bool NPCAttack()
    {
        List<(int, DamageMeshType)> damageList = new List<(int, DamageMeshType)>();

        int damage = 1;
        if (TryDodge(npc.B_LUK, monster.B_LUK))
        {
            //Debug.Log("회피함");
            damage = 0;
        }
        else
        {
            int atkRange = (int)UnityEngine.Random.Range(npc.B_ATK * 0.8f, npc.B_ATK * 1.2f);
            damage = Mathf.Clamp((atkRange - monster.B_DEF), 1, atkRange);
        }

        damageList.Add((damage, DamageMeshType.Damage));

        monster.HP -= damage;
        if (monster.HP <= 0)
        {
            monster.HP = 0;
            result = BattleResult.Monster_Die;
            roundList.Add(new Round(PlacementType.NPC, damage, damageList, result));
            return true;
        }

        roundList.Add(new Round(PlacementType.NPC, damage, damageList, BattleResult.Nothing));
        return false;
    }


    bool TryDodge(int attacker, int defender) //? 회피확률. 최소 5%에 LUK가 1차이날수록 5%씩 증가, 최대 90%
    {

        int chance = Mathf.Clamp((defender - attacker), 1, 18);
        int dice = UnityEngine.Random.Range(0, 20);

        return chance > dice ? true : false;
    }



}
