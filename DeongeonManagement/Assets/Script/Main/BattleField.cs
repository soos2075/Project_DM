using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.U2D.Animation;
using UnityEngine.Rendering;

public class BattleField : MonoBehaviour
{
    public int floorIndex;
    public int slotIndex;


    public int sort = 10;
    void Start()
    {
        ////BattlePlay();
        //sprite_BG.sortingOrder = sort;
        //sprite_border.sortingOrder = sort;

        ////Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        //hpBar_npc.sortingOrder = sort + 1;
        //hpBar_monster.sortingOrder = sort + 1;

        GetComponentInChildren<SortingGroup>().sortingOrder = sort;
    }

    readonly float hpFillOffset = 0.83f;

    public SpriteRenderer sprite_BG;
    public SpriteRenderer sprite_border;
    public SpriteRenderer sprite_Icon;

    public Transform pos_Left;
    public Transform pos_Right;

    //public Transform pos_L_text;
    //public Transform pos_R_text;

    //public Transform pos_L_Effect;
    //public Transform pos_R_Effect;

    //public Transform pos_L_Projectile;
    //public Transform pos_R_Projectile;



    GameObject obj_Left;
    GameObject obj_Right;

    Animator ani_npc;
    Animator ani_monster;

    NPC npc;
    Monster monster;

    public Transform hpBar_Left;
    public Transform hpBar_Right;

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

        float leftValue = Mathf.Clamp01((float)leftCurrent / (float)leftHPMax) * hpFillOffset;
        hpBar_Left.transform.localScale = new Vector3(leftValue, 1, 1);

        float rightValue = Mathf.Clamp01((float)rightCurrent / (float)rightHPMax) * hpFillOffset;
        hpBar_Right.transform.localScale = new Vector3(rightValue, 1, 1);
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
        if (openingList.Count > 0)
        {
            for (int i = 0; i < openingList.Count; i++)
            {
                yield return UserData.Instance.Wait_GamePlay;
                yield return new WaitForSeconds(0.6f);

                if (openingList[i].target is Monster)
                {
                    //? 효과이펙트
                    Instant_StartEffect(pos_Right, openingList[i].effectType);

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
                    //? 효과이펙트
                    Instant_StartEffect(pos_Left, openingList[i].effectType);

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
                yield return new WaitForSeconds(1.0f);
            }
        }

        for (int i = 0; i < roundList.Count; i++)
        {
            yield return UserData.Instance.Wait_GamePlay;
            yield return new WaitForSeconds(1.0f);

            if (roundList[i].attacker == PlacementType.Monster)
            {
                //AddFlashWhite(obj_Left.GetComponentInChildren<SpriteRenderer>());
                AddHPBar(roundList[i].damage, -roundList[i].heal);
                ani_monster.CrossFade(Define.ANIM_Attack, 0.1f);
                yield return new WaitForSeconds(0.1f); //? crossFade 시간 동안은 hash값이 바뀌지 않으므로 그만큼은 기다려줘야함

                if (roundList[i].roundResult == BattleResult.NPC_Die)
                {
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction_Defeat(item.Item1, pos_Left, ani_npc, item.Item2);
                    }
                    foreach (var item in roundList[i].heal_show)
                    {
                        AddAction(item.Item1, pos_Right, item.Item2);
                    }

                    yield return new WaitUntil(() => 
                        ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Ready ||
                        ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle);
                    Main.Instance.ShowDM_MSG("Win!", transform.position + (Vector3.up), new Color32(0, 255, 136, 255), 1);
                    yield return new WaitForSeconds(0.6f);
                    break;
                }
                else
                {
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction(item.Item1, pos_Left, item.Item2);
                    }
                    foreach (var item in roundList[i].heal_show)
                    {
                        AddAction(item.Item1, pos_Right, item.Item2);
                    }
                }
                //Debug.Log(ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash + $"##");
                yield return new WaitUntil(() => 
                    ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Ready ||
                    ani_monster.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle);
                yield return new WaitForSeconds(0.6f);
            }

            if (roundList[i].attacker == PlacementType.NPC)
            {
                //AddFlashWhite(obj_Right.GetComponentInChildren<SpriteRenderer>());
                AddHPBar(-roundList[i].heal, roundList[i].damage);
                NPC_AttackAnim();
                yield return new WaitForSeconds(0.1f); //? crossFade 시간 동안은 hash값이 바뀌지 않으므로 그만큼은 기다려줘야함

                if (roundList[i].roundResult == BattleResult.Monster_Die)
                {
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction_Defeat(item.Item1, pos_Right, ani_monster, item.Item2);
                    }
                    foreach (var item in roundList[i].heal_show)
                    {
                        AddAction(item.Item1, pos_Left, item.Item2);
                    }

                    yield return new WaitUntil(() => 
                        ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle ||
                        ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Ready);
                    Main.Instance.ShowDM_MSG("Lose...", transform.position + (Vector3.up), new Color32(255, 150, 150, 255), 1);
                    yield return new WaitForSeconds(0.6f);
                    break;
                }
                else
                {
                    foreach (var item in roundList[i].damage_show)
                    {
                        AddAction(item.Item1, pos_Right, item.Item2);
                    }
                    foreach (var item in roundList[i].heal_show)
                    {
                        AddAction(item.Item1, pos_Left, item.Item2);
                    }
                }

                yield return new WaitUntil(() => 
                    ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Idle ||
                    ani_npc.GetCurrentAnimatorStateInfo(0).shortNameHash == Define.ANIM_Ready);
                yield return new WaitForSeconds(0.6f);
            }
        }


        yield return new WaitForSeconds(1.0f);
        yield return UserData.Instance.Wait_GamePlay;
        Debug.Log("재생종료");
    }

    #region ForAnimationVoid

    //void Anim_Attack_NPC()
    //{
    //    var pos_Damage = pos_Left.GetChild(0);
    //    var pos_Projectile = pos_Left.GetChild(1);
    //    var pos_Effect = pos_Right.GetChild(2);
    //}

    //void Anim_Attack_Monster()
    //{
    //    var pos_Damage = pos_Right.GetChild(0);
    //    var pos_Projectile = pos_Right.GetChild(1);
    //    var pos_Effect = pos_Left.GetChild(2);
    //}




    void NPC_AttackAnim()
    {
        //? 일단은 마법사나 궁수나 전부 attack로 통일해보자. 법사랑 원거리 공격하는애들 이상하면 그때 바꾸기
        //? 마법사는 ㄱㅊ고 궁수만 문제니까 활쏘는애들 어차피 Elf밖에없는데 얘네만 고치자

        if (npc.name.Contains("Elf"))
        {
            ani_npc.CrossFade(Define.ANIM_Shot, 0.1f);
        }
        else
        {
            ani_npc.CrossFade(Define.ANIM_Attack, 0.1f);
        }

        //switch (npc.AttackOption.AttackAnim)
        //{
        //    case NPC.AttackType.Normal:
        //        ani_npc.CrossFade(Define.ANIM_Attack, 0.1f);
        //        break;
        //    case NPC.AttackType.Bow:
        //        ani_npc.CrossFade(Define.ANIM_Shot, 0.1f);
        //        break;
        //    case NPC.AttackType.Magic:
        //        ani_npc.CrossFade(Define.ANIM_Jab, 0.1f);
        //        break;
        //}
    }

    //public void Projectile_Launch()
    //{
    //    var projectile = Managers.Resource.Instantiate("Battle/Projectile", pos_Left);
    //    projectile.transform.position = pos_Left.transform.position;
    //    projectile.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 2;

    //    projectile.GetComponentInChildren<SpriteResolver>().SetCategoryAndLabel(npc.AttackOption.projectile_Category, npc.AttackOption.projectile_Label);

    //    StartCoroutine(Shotting(projectile));
    //}
    //IEnumerator Shotting(GameObject _projectile)
    //{
    //    for (int i = 0; i < 5; i++)
    //    {
    //        float interval = 0.05f;
    //        _projectile.transform.position += Vector3.right * interval * 4;
    //        yield return new WaitForSeconds(interval);
    //    }
    //    //float timer = 0;
    //    //while (timer < 0.5f)
    //    //{
    //    //    yield return null;
    //    //    timer += Time.deltaTime;

    //    //    _projectile.transform.Translate(Vector3.right * Time.deltaTime * 4);
    //    //}
    //    _projectile.SetActive(false);
    //    Call_Mash();
    //}
    //public void Projectile_Launch_Right()
    //{
    //    var projectile = Managers.Resource.Instantiate("Battle/Projectile", pos_Right);
    //    projectile.transform.position = pos_Right.transform.position;
    //    projectile.GetComponentInChildren<SpriteRenderer>().flipX = true;
    //    projectile.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 2;

    //    var attackOp = monster as I_Projectile;
    //    projectile.GetComponentInChildren<SpriteResolver>().SetCategoryAndLabel(attackOp.AttackOption.projectile_Category, attackOp.AttackOption.projectile_Label);

    //    StartCoroutine(Shotting_Right(projectile));
    //}
    //IEnumerator Shotting_Right(GameObject _projectile)
    //{
    //    for (int i = 0; i < 5; i++)
    //    {
    //        float interval = 0.05f;
    //        _projectile.transform.position += Vector3.left * interval * 4;
    //        yield return new WaitForSeconds(interval);
    //    }
    //    _projectile.SetActive(false);
    //    Call_Mash();
    //}



    public void Event_Attack(GameObject obj)
    {
        I_AttackEffect attacker;
        I_AttackEffect defender;

        //Transform pos_Damage;
        Transform pos_Projectile;
        Transform pos_Effect;

        if (obj == obj_Left)
        {
            attacker = npc;
            defender = monster;

            //pos_Damage = pos_Left.GetChild(0);
            pos_Projectile = pos_Left.GetChild(1);
            pos_Effect = pos_Right.GetChild(2);

            SoundManager.Instance.PlaySound("SFX/Battle_npc");
            switch (attacker.AttackOption.attack_Type)
            {
                case AttackType.Normal:
                    StartCoroutine(FlashWhite(obj_Right.GetComponentInChildren<SpriteRenderer>()));
                    Event_ShowDamageNumber();
                    break;

                case AttackType.Projectile:
                    Projectile_Launch(obj_Left, obj_Right, attacker.AttackOption.effectName, pos_Projectile);
                    break;

                case AttackType.Skill:
                    Projectile_Launch(obj_Left, obj_Right, attacker.AttackOption.effectName, pos_Effect);
                    break;
            }
        }
        else if (obj == obj_Right)
        {
            attacker = monster;
            defender = npc;

            //pos_Damage = pos_Right.GetChild(0);
            pos_Projectile = pos_Right.GetChild(1);
            pos_Effect = pos_Left.GetChild(2);

            SoundManager.Instance.PlaySound("SFX/Battle_monster");
            switch (attacker.AttackOption.attack_Type)
            {
                case AttackType.Normal:
                    StartCoroutine(FlashWhite(obj_Left.GetComponentInChildren<SpriteRenderer>()));
                    Event_ShowDamageNumber();
                    break;

                case AttackType.Projectile:
                    Projectile_Launch(obj_Right, obj_Left, attacker.AttackOption.effectName, pos_Projectile);
                    break;

                case AttackType.Skill:
                    Projectile_Launch(obj_Right, obj_Left, attacker.AttackOption.effectName, pos_Effect);
                    break;
            }
        }
    }

    void Projectile_Launch(GameObject atk, GameObject def, string effectName, Transform parents)
    {
        Instant_Effect($"Attack/{effectName}", parents, atk, def);
        //var projectile = Managers.Resource.Instantiate($"Effect/Attack/{effectName}", parents);
        //projectile.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 2;
        //projectile.GetOrAddComponent<AnimationCall>().Init_Battle(this, atk, def);
    }



    public void Event_FlashWhite(GameObject defender)
    {
        StartCoroutine(FlashWhite(defender.GetComponentInChildren<SpriteRenderer>()));
    }


    public void Event_ShowDamageNumber()
    {
        Call_Damage?.Invoke();
        Call_Damage = null;
    }


    //public void Call_Mash()
    //{
    //    Call_Damage?.Invoke();
    //}

    Action Call_Damage;
    public DamageNumber damageMesh;
    public DamageNumber specialMesh;
    public DamageNumber criticalMesh;
    public DamageNumber healMesh;
    public DamageNumber missMesh;

    public enum DamageMeshType
    {
        Damage,
        Special,
        Critical,
        Heal,
    }

    public void InstantAction(int _dam, Transform parent, DamageMeshType meshType = DamageMeshType.Damage)
    {
        var pos_Damage = parent.GetChild(0);
        switch (meshType)
        {
            case DamageMeshType.Damage:
                DamageNumber dn = damageMesh.Spawn(pos_Damage.transform.position, _dam);
                break;

            case DamageMeshType.Special:
                DamageNumber dn1 = specialMesh.Spawn(pos_Damage.transform.position, _dam);
                break;

            case DamageMeshType.Critical:
                DamageNumber dn2 = criticalMesh.Spawn(pos_Damage.transform.position, _dam);
                break;

            case DamageMeshType.Heal:
                DamageNumber dn3 = healMesh.Spawn(pos_Damage.transform.position, _dam);
                break;
        }
    }

    void Instant_StartEffect(Transform parent, EffectType type)
    {
        var pos_Effect = parent.GetChild(2);

        switch (type)
        {
            case EffectType.Damaged:
                Instant_Effect("Etc/Hit_1", pos_Effect);
                break;

            case EffectType.Heal:
                Instant_Effect("Etc/Heal", pos_Effect);
                break;

            case EffectType.Up_Status:
                Instant_Effect("Etc/Effect_Up", pos_Effect);
                break;

            case EffectType.Down_Status:
                Instant_Effect("Etc/Effect_Down", pos_Effect);
                break;
        }
    }


    void Instant_Effect(string path, Transform parent)
    {
        var effect = Managers.Resource.Instantiate($"Effect/{path}", parent);
        effect.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 2;
    }
    void Instant_Effect(string path, Transform parent, GameObject atk, GameObject def)
    {
        var effect = Managers.Resource.Instantiate($"Effect/{path}", parent);
        effect.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 2;
        effect.GetOrAddComponent<AnimationCall>().Init_Battle(this, atk, def);
    }





    public void AddAction(int _dam, Transform parent, DamageMeshType meshType = DamageMeshType.Damage)
    {
        Call_Damage += () =>
        {
            var pos_Damage = parent.GetChild(0);
            switch (meshType)
            {
                case DamageMeshType.Damage:
                    if (_dam == 0)
                    {
                        DamageNumber damageNumber = missMesh.Spawn(pos_Damage.transform.position, "miss");
                    }
                    else
                    {
                        DamageNumber damageNumber = damageMesh.Spawn(pos_Damage.transform.position, _dam);
                    }
                    break;

                case DamageMeshType.Special:
                    if (_dam == 0)
                    {
                        DamageNumber damageNumber1 = missMesh.Spawn(pos_Damage.transform.position, "miss");
                    }
                    else
                    {
                        DamageNumber damageNumber1 = specialMesh.Spawn(pos_Damage.transform.position, _dam);
                    }
                    break;

                case DamageMeshType.Critical:
                    if (_dam == 0)
                    {
                        DamageNumber damageNumber2 = missMesh.Spawn(pos_Damage.transform.position, "miss");
                    }
                    else
                    {
                        DamageNumber damageNumber2 = criticalMesh.Spawn(pos_Damage.transform.position, _dam);
                    }

                    break;

                case DamageMeshType.Heal:
                    DamageNumber damageNumber3 = healMesh.Spawn(pos_Damage.transform.position, _dam);
                    break;
            }
        };
    }
    public void AddAction_Defeat(int _dam, Transform parent, Animator deadAnim, DamageMeshType meshType = DamageMeshType.Damage)
    {
        AddAction(_dam, parent, meshType);
        Call_Damage += () => deadAnim.Play(Define.ANIM_Dead);
    }
    ////? 얘는 가장 처음 발동해야되서 초기화까지..? 근데 얘도 바꿔야겠네
    //public void AddFlashWhite(SpriteRenderer renderer)
    //{
    //    Call_Damage = () => StartCoroutine(FlashWhite(renderer));
    //}
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

        All = 6,
    }
    public enum EffectType
    {
        Damaged,
        Heal,
        Up_Status,
        Down_Status,
    }



    public class Round
    {
        public PlacementType attacker;

        public int damage;
        public List<(int, DamageMeshType)> damage_show;

        public int heal;
        public List<(int, DamageMeshType)> heal_show;


        public BattleResult roundResult;


        public Round(BattleResult _result, PlacementType _attacker, 
            int _damage, List<(int, DamageMeshType)> _damageList, 
            int _heal, List<(int, DamageMeshType)> _healList)
        {
            roundResult = _result;
            attacker = _attacker;

            damage = _damage;
            damage_show = _damageList;

            heal = _heal;
            heal_show = _healList;
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
        Destroy(obj_Left.GetComponentInChildren<Anim_BattleStatus>().gameObject);
        obj_Left.transform.localPosition = Vector3.zero;
        obj_Left.transform.localScale = Vector3.one * 2;
        obj_Left.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 1;
        obj_Left.GetComponentInChildren<SpriteRenderer>().material = mat_npc;
        ani_npc = obj_Left.GetComponent<Animator>();
        this.npc = npc;

        obj_Right = Instantiate(monster.gameObject, pos_Right);
        Destroy(obj_Right.GetComponent<Monster>());
        Destroy(obj_Right.GetComponentInChildren<Anim_BattleStatus>().gameObject);
        obj_Right.transform.localPosition = Vector3.zero;
        obj_Right.transform.localScale = Vector3.one * 2;
        obj_Right.GetComponentInChildren<SpriteRenderer>().flipX = true;
        obj_Right.GetComponentInChildren<SpriteRenderer>().sortingOrder = sort + 1;
        obj_Right.GetComponentInChildren<SpriteRenderer>().material = mat_monster;
        ani_monster = obj_Right.GetComponent<Animator>();
        this.monster = monster;


        obj_Left.GetOrAddComponent<AnimationCall>().Init_Battle(this, obj_Left, obj_Right);
        obj_Right.GetOrAddComponent<AnimationCall>().Init_Battle(this, obj_Right, obj_Left);


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

        StartEffect(monster, npc);
        StartEffect(npc, monster);

        int agi = npc.B_AGI - monster.B_AGI;
        if (agi > 5)
        {
            //? 플레이어 선공 / 3방때림
            if (NPCAttack(1)) return roundList;
            if (MonsterAttack(1)) return roundList;
            if (NPCAttack(2)) return roundList;
            if (MonsterAttack(2)) return roundList;
            if (NPCAttack(3)) return roundList;
        }
        else if (agi > 0)
        {
            //? 플레이어 선공 / 2방때림
            if (NPCAttack(1)) return roundList;
            if (MonsterAttack(1)) return roundList;
            if (NPCAttack(2)) return roundList;
            if (MonsterAttack(2)) return roundList;
        }
        else if (agi < -5)
        {
            //? 몬스터 선공 / 3방때림
            if (MonsterAttack(1)) return roundList;
            if (NPCAttack(1)) return roundList;
            if (MonsterAttack(2)) return roundList;
            if (NPCAttack(2)) return roundList;
            if (MonsterAttack(3)) return roundList;
        }
        else if (agi <= 0)
        {
            //? 몬스터 선공 / 2방때림
            if (MonsterAttack(1)) return roundList;
            if (NPCAttack(1)) return roundList;
            if (MonsterAttack(2)) return roundList;
            if (NPCAttack(2)) return roundList;
        }


        return roundList;
    }
    BattleResult result;


    bool MonsterAttack(int counter)
    {
        List<(int, DamageMeshType)> damageList = new List<(int, DamageMeshType)>();

        DamageMeshType normalAttackType = DamageMeshType.Damage;
        int attackValue = Calc_AttackValue(monster, npc, out normalAttackType);

        //? 기본공격
        damageList.Add((TryDodge(monster, npc, counter) ? 0 : Calc_Damaged(attackValue, npc.B_DEF), normalAttackType));
        //? 2회 공격
        if (monster.TraitCheck(TraitGroup.DoubleAttack))
        {
            damageList.Add((TryDodge(monster, npc, counter) ? 0 : Calc_Damaged(attackValue, npc.B_DEF), normalAttackType));
        }
        //? 3회 공격
        if (monster.TraitCheck(TraitGroup.TripleAttack))
        {
            damageList.Add((TryDodge(monster, npc, counter) ? 0 : Calc_Damaged(attackValue, npc.B_DEF), normalAttackType));
            damageList.Add((TryDodge(monster, npc, counter) ? 0 : Calc_Damaged(attackValue, npc.B_DEF), normalAttackType));
        }

        //? 특수공격
        damageList.AddRange(TryTrait(monster, npc, Mathf.Clamp((attackValue - npc.B_DEF), 1, attackValue)));



        //? 데미지 증감 특성 계산
        for (int i = 0; i < damageList.Count; i++)
        {
            var item = damageList[i];
            damageList[i] = (TryTrait_Final_Damaged(monster, npc, item.Item1), item.Item2);
        }


        int damage_Sum = 0;
        foreach (var item in damageList)
        {
            damage_Sum += item.Item1;
        }

        List<(int, DamageMeshType)> healList = new List<(int, DamageMeshType)>();
        healList.AddRange(TryTrait_Util(monster, npc, damage_Sum));

        int heal_Sum = 0;
        foreach (var item in healList)
        {
            heal_Sum += item.Item1;
        }


        npc.HP_Damaged += damage_Sum;
        if (npc.B_HP <= 0)
        {
            result = BattleResult.NPC_Die;
            roundList.Add(new Round(result, PlacementType.Monster, damage_Sum, damageList, heal_Sum, healList));
            return true;
        }

        roundList.Add(new Round(BattleResult.Nothing, PlacementType.Monster, damage_Sum, damageList, heal_Sum, healList));
        return false;
    }
    bool NPCAttack(int counter)
    {
        List<(int, DamageMeshType)> damageList = new List<(int, DamageMeshType)>();

        //int damage = TryDodge(npc, monster, counter) ? 0 : Calc_Damaged(npc, monster);

        //damageList.Add((damage, DamageMeshType.Damage));
        //damageList.AddRange(TryTrait(npc, monster, damage));
        //damageList.AddRange(TryTrait_Targeting(npc, monster, damage));


        DamageMeshType normalAttackType = DamageMeshType.Damage;
        int attackValue = Calc_AttackValue(npc, monster, out normalAttackType);

        //? 기본공격
        damageList.Add((TryDodge(npc, monster, counter) ? 0 : Calc_Damaged(attackValue, monster.B_DEF), normalAttackType));
        //? 2회 공격
        if (npc.TraitCheck(TraitGroup.DoubleAttack))
        {
            damageList.Add((TryDodge(npc, monster, counter) ? 0 : Calc_Damaged(attackValue, monster.B_DEF), normalAttackType));
        }
        //? 3회 공격
        if (npc.TraitCheck(TraitGroup.TripleAttack))
        {
            damageList.Add((TryDodge(npc, monster, counter) ? 0 : Calc_Damaged(attackValue, monster.B_DEF), normalAttackType));
            damageList.Add((TryDodge(npc, monster, counter) ? 0 : Calc_Damaged(attackValue, monster.B_DEF), normalAttackType));
        }

        //? 특수공격
        damageList.AddRange(TryTrait(npc, monster, Mathf.Clamp((attackValue - monster.B_DEF), 1, attackValue)));
        damageList.AddRange(TryTrait_Targeting(npc, monster, Mathf.Clamp((attackValue - monster.B_DEF), 1, attackValue)));



        //? 데미지 증감 특성 계산
        for (int i = 0; i < damageList.Count; i++)
        {
            var item = damageList[i];
            damageList[i] = (TryTrait_Final_Damaged(npc, monster, item.Item1), item.Item2);
        }

        int damage_Sum = 0;
        foreach (var item in damageList)
        {
            damage_Sum += item.Item1;
        }

        List<(int, DamageMeshType)> healList = new List<(int, DamageMeshType)>();
        healList.AddRange(TryTrait_Util(npc, monster, damage_Sum));

        int heal_Sum = 0;
        foreach (var item in healList)
        {
            heal_Sum += item.Item1;
        }


        monster.HP_Damaged += damage_Sum;
        if (monster.B_HP <= 0)
        {
            //monster.HP = 0;
            result = BattleResult.Monster_Die;
            roundList.Add(new Round(result, PlacementType.NPC, damage_Sum, damageList, heal_Sum, healList));
            return true;
        }

        roundList.Add(new Round(BattleResult.Nothing, PlacementType.NPC, damage_Sum, damageList, heal_Sum, healList));
        return false;
    }





    //? 시작효과
    void StartEffect<T1, T2>(T1 attacker, T2 defender) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    {
        //? 버프
        if (attacker.TraitCheck(TraitGroup.Grit))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Empower, 1);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.All, 0));
        }



        if (attacker.TraitCheck(TraitGroup.Fierce))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Sharp, 1);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.ATK, 0));
        }
        if (attacker.TraitCheck(TraitGroup.Fierce_V2))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Sharp, 2);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.ATK, 0));
        }

        if (attacker.TraitCheck(TraitGroup.ToughSkin))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Guard, 1);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.DEF, 0));
        }
        if (attacker.TraitCheck(TraitGroup.ToughSkin_V2))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Guard, 2);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.DEF, 0));
        }

        if (attacker.TraitCheck(TraitGroup.Wind))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Haste, 1);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.AGI, 0));
        }
        if (attacker.TraitCheck(TraitGroup.Wind_V2))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Haste, 2);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.AGI, 0));
        }
        if (attacker.TraitCheck(TraitGroup.Succubus))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Chance, 1);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.LUK, 0));
        }
        if (attacker.TraitCheck(TraitGroup.Succubus_V2))
        {
            attacker.CurrentBattleStatus.AddValue(BattleStatusLabel.Chance, 2);
            openingList.Add(new OpeningTrait(EffectType.Up_Status, attacker, StatType.LUK, 0));
        }




        //? 디버프
        if (attacker.TraitCheck(TraitGroup.Spore))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Wither, 1);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.ATK, 0));
        }
        if (attacker.TraitCheck(TraitGroup.Spore_V2))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Wither, 2);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.ATK, 0));
        }

        if (attacker.TraitCheck(TraitGroup.Acid))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Corrode, 1);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.DEF, 0));
        }
        if (attacker.TraitCheck(TraitGroup.Acid_V2))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Corrode, 2);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.DEF, 0));
        }

        if (attacker.TraitCheck(TraitGroup.ThornyVine))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Slow, 1);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.AGI, 0));
        }
        if (attacker.TraitCheck(TraitGroup.ThornyVine_V2))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Slow, 2);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.AGI, 0));
        }

        if (attacker.TraitCheck(TraitGroup.Golem))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Jinx, 1);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.LUK, 0));
        }
        if (attacker.TraitCheck(TraitGroup.Golem_V2))
        {
            defender.CurrentBattleStatus.AddValue(BattleStatusLabel.Jinx, 2);
            openingList.Add(new OpeningTrait(EffectType.Down_Status, defender, StatType.LUK, 0));
        }




        //? 기존꺼
        if (attacker.TraitCheck(TraitGroup.Vitality))
        {
            //int bonusHP = attacker.GetSomething(TraitGroup.Vitality, attacker.B_HP_Max);
            //int applyHP = attacker.B_HP + bonusHP;

            int bonusHP = Mathf.RoundToInt(attacker.B_HP_Max * 0.1f);
            int applyHP = attacker.HP_Damaged >= bonusHP ? bonusHP : attacker.HP_Damaged;
            attacker.HP_Damaged -= applyHP;
            openingList.Add(new OpeningTrait(EffectType.Heal, attacker, StatType.HP, applyHP));
        }
        if (attacker.TraitCheck(TraitGroup.Vitality_V2))
        {
            int bonusHP = Mathf.RoundToInt(attacker.B_HP_Max * 0.2f);
            int applyHP = attacker.HP_Damaged >= bonusHP ? bonusHP : attacker.HP_Damaged;
            attacker.HP_Damaged -= applyHP;
            openingList.Add(new OpeningTrait(EffectType.Heal, attacker, StatType.HP, applyHP));
        }


        if (attacker.TraitCheck(TraitGroup.Overwhelm))
        {
            int realValue = Mathf.RoundToInt(defender.B_HP * 0.1f);
            defender.HP_Damaged += realValue;
            openingList.Add(new OpeningTrait(EffectType.Damaged, defender, StatType.HP, realValue));
        }
        if (attacker.TraitCheck(TraitGroup.Overwhelm_V2))
        {
            int realValue = Mathf.RoundToInt(defender.B_HP * 0.2f);
            defender.HP_Damaged += realValue;
            openingList.Add(new OpeningTrait(EffectType.Damaged, defender, StatType.HP, realValue));
        }


        if (attacker.TraitCheck(TraitGroup.Reaper))
        {
            int realValue = Mathf.RoundToInt(attacker.B_HP_Max * 0.05f);
            attacker.HP_Damaged += realValue;
            openingList.Add(new OpeningTrait(EffectType.Damaged, attacker, StatType.HP, realValue));

            defender.HP_Damaged += realValue * 2;
            openingList.Add(new OpeningTrait(EffectType.Damaged, defender, StatType.HP, realValue * 2));
        }
    }





    //? 공격할 스탯 (방어 계산 전)
    int Calc_AttackValue<T1, T2>(T1 attacker, T2 defender, out DamageMeshType dmgType) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    {
        DamageMeshType type = DamageMeshType.Damage;
        int atkRange = attacker.B_ATK;

        //? 괴력 = ATK 의 30% / 60% 추가공격
        if (attacker.TraitCheck(TraitGroup.Powerful)) atkRange += Mathf.RoundToInt((attacker.B_ATK) * 0.3f);
        if (attacker.TraitCheck(TraitGroup.Powerful_V2)) atkRange += Mathf.RoundToInt((attacker.B_ATK) * 0.6f);

        //? 철스킨 = DEF 의 50% / 100% 추가공격
        if (attacker.TraitCheck(TraitGroup.IronSkin)) atkRange += attacker.B_DEF / 2;
        if (attacker.TraitCheck(TraitGroup.IronSkin_V2)) atkRange += attacker.B_DEF;

        //? 질풍 = AGI 의 50% / 100% 추가공격
        if (attacker.TraitCheck(TraitGroup.GaleForce)) atkRange += attacker.B_AGI / 2;
        if (attacker.TraitCheck(TraitGroup.GaleForce_V2)) atkRange += attacker.B_AGI;

        //? 행운 = LUK 의 50% / 100% 추가공격
        if (attacker.TraitCheck(TraitGroup.LuckyPunch)) atkRange += attacker.B_LUK / 2;
        if (attacker.TraitCheck(TraitGroup.LuckyPunch_V2)) atkRange += attacker.B_LUK;

        //? 필살 = 모든 스탯 합계 의 20% / 40% 추가공격
        if (attacker.TraitCheck(TraitGroup.Ultimate)) atkRange += Mathf.RoundToInt((attacker.B_ATK + attacker.B_DEF + attacker.B_AGI + attacker.B_LUK) * 0.2f);
        if (attacker.TraitCheck(TraitGroup.Ultimate_V2)) atkRange += Mathf.RoundToInt((attacker.B_ATK + attacker.B_DEF + attacker.B_AGI + attacker.B_LUK) * 0.4f);


        if (atkRange > attacker.B_ATK)
        {
            type = DamageMeshType.Critical;
        }
        dmgType = type;

        return atkRange;
    }

    //? 진짜 공격 = 데미지 스프레드 후 방어 적용
    int Calc_Damaged(int attackValue, int _DEF)
    {
        int lastATK = Mathf.RoundToInt(UnityEngine.Random.Range(attackValue * 0.8f, attackValue * 1.2f));
        int damage = Mathf.Clamp((lastATK - _DEF), 1, lastATK);
        return damage;
    }


    ////? 기본 공격
    //int Calc_Damaged<T1, T2>(T1 attacker, T2 defender) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    //{
    //    int damage = 1;
    //    int atkRange = attacker.B_ATK;

    //    //? 괴력 = ATK 의 30% / 60% 추가공격
    //    if (attacker.TraitCheck(TraitGroup.Powerful)) atkRange += Mathf.RoundToInt((attacker.B_ATK) * 0.3f);
    //    if (attacker.TraitCheck(TraitGroup.Powerful_V2)) atkRange += Mathf.RoundToInt((attacker.B_ATK) * 0.6f);

    //    //? 철스킨 = DEF 의 50% / 100% 추가공격
    //    if (attacker.TraitCheck(TraitGroup.IronSkin)) atkRange += attacker.B_DEF / 2;
    //    if (attacker.TraitCheck(TraitGroup.IronSkin_V2)) atkRange += attacker.B_DEF;

    //    //? 질풍 = AGI 의 50% / 100% 추가공격
    //    if (attacker.TraitCheck(TraitGroup.GaleForce)) atkRange += attacker.B_AGI / 2;
    //    if (attacker.TraitCheck(TraitGroup.GaleForce_V2)) atkRange += attacker.B_AGI;

    //    //? 행운 = LUK 의 50% / 100% 추가공격
    //    if (attacker.TraitCheck(TraitGroup.LuckyPunch)) atkRange += attacker.B_LUK / 2;
    //    if (attacker.TraitCheck(TraitGroup.LuckyPunch_V2)) atkRange += attacker.B_LUK;

    //    //? 필살 = 모든 스탯 합계 의 20% / 40% 추가공격
    //    if (attacker.TraitCheck(TraitGroup.Ultimate)) atkRange += Mathf.RoundToInt((attacker.B_ATK + attacker.B_DEF + attacker.B_AGI + attacker.B_LUK) * 0.2f);
    //    if (attacker.TraitCheck(TraitGroup.Ultimate_V2)) atkRange += Mathf.RoundToInt((attacker.B_ATK + attacker.B_DEF + attacker.B_AGI + attacker.B_LUK) * 0.4f);



    //    int lastATK = (int)UnityEngine.Random.Range(atkRange * 0.8f, atkRange * 1.2f);

    //    damage = Mathf.Clamp((lastATK - defender.B_DEF), 1, lastATK);

    //    return damage;
    //}


    //? 회피시스템
    bool TryDodge<T1, T2>(T1 attacker, T2 defender, int atkCount) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    {
        if (atkCount == 1 && attacker.TraitCheck(TraitGroup.EagleEye))
        {
            return false;
        }

        if (atkCount == 2 && defender.TraitCheck(TraitGroup.Blindness))
        {
            return true;
        }



        //? 회피확률. LUK가 1차이날수록 5%씩 증가, 최소 5%에  최대 95%
        int chance = Mathf.Clamp((defender.B_LUK - attacker.B_LUK), 1, 19);
        int dice = UnityEngine.Random.Range(0, 20);


        bool isDodge = chance > dice;

        return isDodge;

        //int damage = 1;
        //if (chance > dice)
        //{
        //    damage = 0;
        //}
        //else
        //{
        //    int atkRange = (int)UnityEngine.Random.Range(attacker.B_ATK * 0.8f, attacker.B_ATK * 1.2f);
        //    damage = Mathf.Clamp((atkRange - defender.B_DEF), 1, atkRange);
        //}

        //return damage;
    }

    //? 기본 공격 특성
    List<(int, DamageMeshType)> TryTrait<T1, T2>(T1 attacker, T2 defender, int damage) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    {
        List<(int, DamageMeshType)> addList = new List<(int, DamageMeshType)>();

        //? 님블 = 0.5배 추가공격
        if (attacker.TraitCheck(TraitGroup.Nimble))
        {
            int bonusAttack = damage / 2;
            addList.Add((bonusAttack, DamageMeshType.Damage));
        }
        if (attacker.TraitCheck(TraitGroup.Nimble_V2))
        {
            int bonusAttack = damage;
            addList.Add((bonusAttack, DamageMeshType.Damage));
        }

        //? 화상 = 0.2배 추가공격 3번
        if (attacker.TraitCheck(TraitGroup.Burn))
        {
            int bonusAttack = Mathf.RoundToInt(damage * 0.2f);
            addList.Add((bonusAttack, DamageMeshType.Damage));
            addList.Add((bonusAttack, DamageMeshType.Damage));
            addList.Add((bonusAttack, DamageMeshType.Damage));
        }


        //? 신성력	공격 시 상대 최대 HP의 8%만큼 추가데미지
        if (attacker.TraitCheck(TraitGroup.DivineForce))
        {
            int trueDamage = Mathf.RoundToInt(defender.B_HP_Max * 0.08f);
            addList.Add((trueDamage, DamageMeshType.Special));
        }

        //? 맹독	    공격 시 상대 현재 HP의 12%만큼 추가데미지
        if (attacker.TraitCheck(TraitGroup.Venom))
        {
            int trueDamage = Mathf.RoundToInt(defender.B_HP * 0.12f);
            addList.Add((trueDamage, DamageMeshType.Special));
        }

        return addList;
    }

    //? 유틸 효과 - 공격자한테 추가하는거
    List<(int, DamageMeshType)> TryTrait_Util<T1, T2>(T1 attacker, T2 defender, int damage) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    {
        List<(int, DamageMeshType)> addList = new List<(int, DamageMeshType)>();

        //? 정기흡수 = dmg 20% / 40% 체력흡수
        if (attacker.TraitCheck(TraitGroup.LifeDrain))
        {
            int bonusHP = Mathf.RoundToInt(damage * 0.2f);
            if (attacker is Monster mon)
            {
                mon.traitCounter.AddCustomValue(bonusHP);
            }

            int applyHP = attacker.HP_Damaged >= bonusHP ? bonusHP : attacker.HP_Damaged;
            attacker.HP_Damaged -= applyHP;
            addList.Add((applyHP, DamageMeshType.Heal));
        }
        if (attacker.TraitCheck(TraitGroup.LifeDrain_V2))
        {
            int bonusHP = Mathf.RoundToInt(damage * 0.4f);
            if (attacker is Monster mon)
            {
                mon.traitCounter.AddCustomValue(bonusHP);
            }

            int applyHP = attacker.HP_Damaged >= bonusHP ? bonusHP : attacker.HP_Damaged;
            attacker.HP_Damaged -= applyHP;
            addList.Add((applyHP, DamageMeshType.Heal));
        }

        return addList;
    }




    //? 특정 타겟 보너스
    List<(int, DamageMeshType)> TryTrait_Targeting<T1, T2>(T1 attacker, T2 defender, int damage) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    {
        List<(int, DamageMeshType)> addList = new List<(int, DamageMeshType)>();

        if (attacker.TraitCheck(TraitGroup.Hunting_Slime) && defender.GetType() == typeof(Slime))
        {
            int bonusAttack = damage / 2;
            addList.Add((bonusAttack, DamageMeshType.Damage));
        }

        if (attacker.TraitCheck(TraitGroup.Hunting_Golem) && defender.GetType() == typeof(EarthGolem))
        {
            int bonusAttack = damage / 2;
            addList.Add((bonusAttack, DamageMeshType.Damage));
        }



        return addList;
    }


    //? 최종 데미지 효과 - 데미지 계산 바로 전에 하는거
    int TryTrait_Final_Damaged<T1, T2>(T1 attacker, T2 defender, int damage) where T1 : I_TraitSystem, I_BattleStat where T2 : I_TraitSystem, I_BattleStat
    {
        if (defender.TraitCheck(TraitGroup.AbsoluteShield))
        {
            return damage > 0 ? 1 : 0;
        }
        return damage;
    }


}
