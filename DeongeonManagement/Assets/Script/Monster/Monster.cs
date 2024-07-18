using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IPlacementable, I_BattleStat
{
    protected void Awake()
    {

    }
    protected void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sizeOffset = transform.localScale.x;
        //MonsterInit();
        //Initialize_Status();
    }


    #region IPlacementable
    public PlacementType PlacementType { get; set; }
    public PlacementState PlacementState { get; set; }
    public PlacementInfo PlacementInfo { get; set; }
    public GameObject GetObject()
    {
        return this.gameObject;
    }

    public string Name_Color { get { return $"{name_Tag_Start}{Name}{name_Tag_End}"; } }
    private string name_Tag_Start = "<color=#44ff44ff>";
    private string name_Tag_End = "</color>";

    public virtual string Detail_KR { get { return Data.detail; } }


    public virtual void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Data == null) return;

        if (FindAnyObjectByType<UI_Monster_Management>() == null)
        {
            StartCoroutine(ShowMonsterManagement());
        }
    }

    public virtual void MouseMoveEvent()
    {
        //if (Main.Instance.Management == false) return;
    }
    public virtual void MouseExitEvent()
    {
        //Cancle_QuickEvent();
    }



    IEnumerator ShowMonsterManagement()
    {
        var ui = Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management");
        yield return new WaitForEndOfFrame();

        ui.ShowDetail(this);
    }


    public virtual void MouseDownEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Data == null) return;


        Cancle_QuickEvent();
        Main.Instance.QuickPlacement = StartCoroutine(QuickPlacement());
    }
    public virtual void MouseUpEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Data == null) return;


        Cancle_QuickEvent();
    }


    void Cancle_QuickEvent()
    {
        if (Main.Instance.QuickPlacement != null)
        {
            StopCoroutine(Main.Instance.QuickPlacement);
            Main.Instance.QuickPlacement = null;
            Debug.Log("퀵배치 취소");
        }
    }

    IEnumerator QuickPlacement()
    {
        yield return new WaitForSecondsRealtime(0.6f);


        Debug.Log("퀵배치");
        //var ui = Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management");
        //ui.Type = UI_Monster_Management.UI_Type.Placement;
        //Main.Instance.CurrentFloor = PlacementInfo.Place_Floor;

        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        //ui.ShowDetail(this);



        //ui.Type = UI_Monster_Management.UI_Type.Placement;
        //yield return null;

        var ui = Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management");
        ui.isQuickMode = true;
        MonsterOutFloor();
        ui.QuickPlacement(MonsterID);

        Main.Instance.QuickPlacement = null;
    }


    #endregion



    #region SaveLoad

    public void Initialize_Load(Save_MonsterData _LoadData)
    {
        if (_LoadData == null) { Debug.Log($"세이브데이터 없음 : {name}"); return; }

        LV = _LoadData.LV;
        HP = _LoadData.HP;
        HP_Max = _LoadData.HP_MAX;

        ATK = _LoadData.ATK;
        DEF = _LoadData.DEF;
        AGI = _LoadData.AGI;
        LUK = _LoadData.LUK;

        hp_chance = _LoadData.HP_chance;
        atk_chance = _LoadData.ATK_chance;
        def_chance = _LoadData.DEF_chance;
        agi_chance = _LoadData.AGI_chance;
        luk_chance = _LoadData.LUK_chance;

        State = _LoadData.State;
        Mode = _LoadData.MoveMode;
        EvolutionState = _LoadData.Evolution;
        BattlePoint_Count = _LoadData.BattleCount;
        BattlePoint_Rank = _LoadData.BattlePoint;
    }

    #endregion



    #region For Management
    public enum MonsterState
    {
        Standby,
        Placement,
        Injury,
    }

    private MonsterState state;
    public MonsterState State { get { return state; } 
        set 
        {
            state = value;
            if (state == MonsterState.Injury)
            {
                Injury();
                if (Cor_Moving != null)
                {
                    StopCoroutine(Cor_Moving);
                    Cor_Moving = null;
                }
            }
        } 
    }


    #endregion








    public abstract SO_Monster Data { get; set; }
    public int MonsterID { get; set; }



    #region Collection

    public void AddCollectionPoint()
    {
        var collection = CollectionManager.Instance.Get_Collection(Data);
        if (collection != null)
        {
            collection.AddPoint();
        }
    }

    #endregion



    #region I_Battle Stat

    public int B_HP { get => HP_Final; }
    public int B_HP_Max { get => HP_Max; }
    public int B_ATK { get => ATK_Final; }
    public int B_DEF { get => DEF_Final; }
    public int B_AGI { get => AGI_Final; }
    public int B_LUK { get => LUK_Final; }

    #endregion





    #region Monster Status
    public string Name { get; protected set; }
    public int LV { get; protected set; }
    public int HP { get; set; }
    public int HP_Max { get; protected set; }

    public int ATK { get; protected set; }
    public int DEF { get; protected set; }
    public int AGI { get; protected set; }
    public int LUK { get; protected set; }

    public float hp_chance;
    public float atk_chance;
    public float def_chance;
    public float agi_chance;
    public float luk_chance;



    int HP_Final { get { return HP + Trait_HP; } }


    int ATK_Final { get { return ATK + AllStat_Bonus + Trait_ATK + ATK_Bonus; } }
    int DEF_Final { get { return DEF + AllStat_Bonus + Trait_DEF + DEF_Bonus; } }
    int AGI_Final { get { return AGI + AllStat_Bonus + Trait_AGI + AGI_Bonus; } }
    int LUK_Final { get { return LUK + AllStat_Bonus + Trait_LUK + LUK_Bonus; } }



    int ATK_Bonus { get { return 0; } }
    int DEF_Bonus { get { return 0; } }
    int AGI_Bonus { get { return 0; } }
    int LUK_Bonus { get { return 0; } }


    int AllStat_Bonus { get { return Orb_Bonus + Floor_Bonus + Trait_Friend; } }



    //? 전투의 오브 활성화 보너스
    int Orb_Bonus { get { return GameManager.Buff.CurrentBuff.Orb_red > 0 ? 5 : 0; } }
    //? 깊은 층에 배치할수록 스탯보너스
    int Floor_Bonus { get { return PlacementInfo != null ? PlacementInfo.Place_Floor.FloorIndex * 1 : 0; } }

    //? 같은층의 몬스터 수에 따른 보너스
    int Trait_Friend
    {
        get
        {
            if (State == MonsterState.Placement)
            {
                return TraitCheck(TraitGroup.Friend) ? PlacementInfo.Place_Floor.GetFloorObjectList(Define.TileType.Monster).Count - 1 : 0;
            }
            return 0;
        }
    }



    #region IStat & ITrait
    int Trait_HP { get => TraitList.Count > 0 ? Apply_HP() : 0; }
    int Trait_HP_Max { get => TraitList.Count > 0 ? Apply_HP_Max() : 0; }
    int Trait_ATK { get => TraitList.Count > 0 ? Apply_ATK() : 0; }
    int Trait_DEF { get => TraitList.Count > 0 ? Apply_DEF() : 0; }
    int Trait_AGI { get => TraitList.Count > 0 ? Apply_AGI() : 0; }
    int Trait_LUK { get => TraitList.Count > 0 ? Apply_LUK() : 0; }





    public List<ITrait> TraitList = new List<ITrait>();

    public void AddTrait(ITrait trait) //? 동일한 특성 불가능
    {
        foreach (var item in TraitList)
        {
            if (trait.ID == item.ID)
            {
                return;
            }
        }
        TraitList.Add(trait);
    }
    public void AddTrait(TraitGroup traitID)
    {
        string className = $"Trait+{traitID.ToString()}";
        ITrait trait = Util.GetClassToString<ITrait>(className);
        AddTrait(trait);
    }

    public bool TraitCheck(TraitGroup searchTrait)
    {
        var trait = Util.GetTypeToString($"Trait+{searchTrait.ToString()}");
        foreach (var item in TraitList)
        {
            if (item.GetType() == trait) return true;
        }
        return false;
    }
    public void DoSomething(TraitGroup searchTrait)
    {
        var trait = Util.GetTypeToString($"Trait+{searchTrait.ToString()}");
        foreach (var item in TraitList)
        {
            if (item.GetType() == trait)
            {
                item.DoSomething();
            }
        }
    }
    public int GetSomething<T>(TraitGroup searchTrait, T current)
    {
        var trait = Util.GetTypeToString($"Trait+{searchTrait.ToString()}");
        foreach (var item in TraitList)
        {
            if (item.GetType() == trait)
            {
                int tValue = item.GetSomething(current);
                return tValue;
            }
        }
        return 0;
    }





    int Apply_HP()
    {
        int applyHp = 0;

        foreach (var trait in TraitList)
        {
            applyHp += trait.ApplyHP(HP);
        }
        return applyHp;
    }
    int Apply_HP_Max()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            applyValue += trait.ApplyHP_Max(HP_Max);
        }
        return applyValue;
    }
    int Apply_ATK()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            applyValue += trait.ApplyATK(ATK);
        }
        return applyValue;
    }
    int Apply_DEF()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            applyValue += trait.ApplyDEF(DEF);
        }
        return applyValue;
    }
    int Apply_AGI()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            applyValue += trait.ApplyAGI(AGI);
        }
        return applyValue;
    }
    int Apply_LUK()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            applyValue += trait.ApplyLUK(LUK);
        }
        return applyValue;
    }







    public List<int> SaveTraitList()
    {
        List<int> saveList = new List<int>();
        foreach (var item in TraitList)
        {
            saveList.Add((int)item.ID);
        }

        return saveList;
    }

    public void LoadTraitList(List<int> loadData)
    {
        foreach (var item in loadData)
        {
            AddTrait((TraitGroup)item);
        }
    }



    #endregion






    public abstract void MonsterInit();
    //public abstract void Trait_Original();
    public virtual void MonsterInit_Evolution() //? 나중에 abstract로 변경하면 댐
    { 

    }
    public void Initialize_Status()
    {
        if (Data == null) { Debug.Log($"데이터 없음 : {name}"); return; }

        Name = Data.labelName;
        LV = Data.startLv;

        HP = Data.hp;
        HP_Max = Data.hp;

        ATK = Data.atk;
        DEF = Data.def;
        AGI = Data.agi;
        LUK = Data.luk;

        hp_chance = Data.up_hp;
        atk_chance = Data.up_atk;
        def_chance = Data.up_def;
        agi_chance = Data.up_agi;
        luk_chance = Data.up_luk;
    }


    #endregion



    public virtual void TurnStart()
    {
        switch (State)
        {
            case MonsterState.Standby:
                break;

            case MonsterState.Placement:
                MoveSelf();
                HP = HP_Max;
                break;

            case MonsterState.Injury:
                if (TraitCheck(TraitGroup.Reconfigure))
                {
                    HP = HP_Max;
                    State = MonsterState.Standby;
                }
                break;
        }
    }

    public virtual void TurnOver()
    {

    }

    public virtual void MoveSelf()
    {
        //Debug.Log("몬스터 무브애니메이션 다시 시작");
        Cor_Moving = StartCoroutine(MoveCor());
    }

    #region MonsterMove
    Animator anim;

    protected Coroutine Cor_Moving { get; set; }
    Coroutine Cor_moveAnimation;

    public enum MoveType
    {
        Fixed,

        Wander,

        Attack,
    }

    public MoveType Mode { get; set; } = MoveType.Fixed;
    public void SetMoveType(MoveType _moveType)
    {
        Mode = _moveType;
    }


    protected IEnumerator MoveCor()
    {
        yield return new WaitForSeconds(2);

        while (Main.Instance.Management == false && State == MonsterState.Placement)
        {
            float ranDelay = Random.Range(1.5f, 2.5f);
            switch (Mode)
            {
                case MoveType.Fixed:
                    break;

                case MoveType.Wander:
                    Moving(ranDelay);
                    break;

                case MoveType.Attack:
                    Moving_Attack(ranDelay);
                    break;
            }
            yield return UserData.Instance.Wait_GamePlay;
            yield return new WaitForSeconds(ranDelay);
        }
    }

    protected BasementTile GetRandomTile()
    {
        BasementTile newTile;

        int dir = Random.Range(0, 5);
        switch (dir)
        {
            case 0:
                newTile = PlacementInfo.Place_Floor.GetTileUp(this, PlacementInfo.Place_Tile);
                break;

            case 1:
                newTile = PlacementInfo.Place_Floor.GetTileDown(this, PlacementInfo.Place_Tile);
                break;

            case 2:
                newTile = PlacementInfo.Place_Floor.GetTileLeft(this, PlacementInfo.Place_Tile);
                break;

            case 3:
                newTile = PlacementInfo.Place_Floor.GetTileRight(this, PlacementInfo.Place_Tile);
                break;

            default:
                newTile = null;
                break;
        }

        return newTile;
    }

    protected void Moving(float _delay)
    {
        BasementTile tile = GetRandomTile();
        if (tile != null)
        {
            var eventType = tile.TryPlacement(this);

            switch (eventType)
            {
                case Define.PlaceEvent.Placement:
                    if (Cor_moveAnimation != null)
                    {
                        StopCoroutine(Cor_moveAnimation);
                    }
                    Cor_moveAnimation = StartCoroutine(MoveUpdate_Monster(tile, _delay));
                    GameManager.Placement.PlacementMove(this, new PlacementInfo(PlacementInfo.Place_Floor, tile));
                    break;

                case Define.PlaceEvent.Battle:
                    var npc = tile.Original as NPC;
                    if (npc.Cor_Encounter == null && npc.HP > 0 && this.HP > 0)
                    {
                        npc.Cor_Encounter = StartCoroutine(npc.Encounter_ByMonster(this));
                        //Debug.Log($"몬스터 선빵때리기");
                    }
                    break;

                default:
                    //Debug.Log($"{eventType.ToString()} : 아무이벤트 없음");
                    break;
            }
        }
    }

    protected void Moving_Attack(float _delay)
    {
        BasementTile tile = null;
        float prev_dist = 1000;
        
        // Floor에게 전체 npc를 받아와서 가장 가까운 npc를 찾아서 tile을 해당 npc의 tile로 갱신
        var npcList = PlacementInfo.Place_Floor.GetFloorObjectList(Define.TileType.NPC);
        if (npcList.Count == 0)
        {
            //Debug.Log("층에 npc가 없으므로 이동하지 않음");
            return;
        }

        foreach (var item in npcList)
        {
            float current_dist = (item.worldPosition - PlacementInfo.Place_Tile.worldPosition).magnitude;
            if (current_dist < prev_dist )
            {
                tile = item;
                prev_dist = current_dist;
            }
        }

        // 찾은 타일과의 패스파인딩. npc가 있었다면 가장 가까운 npc가 타겟 / 빈공간 or npc tile로만 길찾기 함
        bool pathFind = false;
        List<BasementTile> path = PlacementInfo.Place_Floor.PathFinding_Monster(PlacementInfo.Place_Tile, tile, out pathFind);

        if (pathFind)
        {
            //Debug.Log("길찾기 성공 and 이동");
            //Debug.Log("몬스터 위치 : " + PlacementInfo.Place_Tile.worldPosition);
            //Debug.Log("이동할 위치 : " + path[1].worldPosition);
            tile = path[1];

            var eventType = tile.TryPlacement(this);

            switch (eventType)
            {
                case Define.PlaceEvent.Placement:
                    if (Cor_moveAnimation != null)
                    {
                        StopCoroutine(Cor_moveAnimation);
                    }
                    Cor_moveAnimation = StartCoroutine(MoveUpdate_Monster(tile, _delay));
                    GameManager.Placement.PlacementMove(this, new PlacementInfo(PlacementInfo.Place_Floor, tile));
                    break;

                case Define.PlaceEvent.Battle:
                    var npc = tile.Original as NPC;
                    if (npc.Cor_Encounter == null && npc.HP > 0 && this.HP > 0)
                    {
                        npc.Cor_Encounter = StartCoroutine(npc.Encounter_ByMonster(this));
                        //Debug.Log($"몬스터 선빵때리기");
                    }
                    break;

                default:
                    //Debug.Log($"{eventType.ToString()} : 아무이벤트 없음");
                    break;
            }
        }
    }



    IEnumerator MoveUpdate_Monster(BasementTile endPos, float duration)
    {
        var startPos = PlacementInfo.Place_Tile;
        Vector3 dir = endPos.worldPosition - startPos.worldPosition;
        SetDirection(dir);

        float dis = Vector3.Distance(startPos.worldPosition, endPos.worldPosition);

        float moveValue = dis / duration;
        float timer = 0;

        anim.Play(Define.ANIM_Running);
        while (timer < (duration * 0.95f))
        {
            //yield return null;
            yield return UserData.Instance.Wait_GamePlay;

            timer += Time.deltaTime;
            transform.position += dir.normalized * moveValue * Time.deltaTime;
        }
        anim.Play(Define.ANIM_Idle);

        transform.position = endPos.worldPosition;
        Cor_moveAnimation = null;
    }

    [SerializeField]
    private float sizeOffset;
    void SetDirection(Vector3 dir)
    {
        if (dir.x > 0)
        {
            //? 무브 오른쪽
            //transform.localRotation = Quaternion.Euler(0, 180, 0);
            transform.localScale = Vector3.one * sizeOffset;
        }
        else if (dir.x < 0)
        {
            //? 왼쪽
            //transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(-sizeOffset, sizeOffset, sizeOffset);
        }
        //else if (dir.y > 0)
        //{
        //    //? 위
        //    _monster.Anim_State = NPC.moveState.back;
        //}
        //else if (dir.y < 0)
        //{
        //    //? 아래
        //    _monster.Anim_State = NPC.moveState.front;
        //}
    }



    #endregion

    int BattleCount { get; set; } = 0;
    Coroutine Cor_Battle { get; set; }

    public Coroutine Battle(NPC npc)
    {
        if (this.HP > 0 && npc.HP > 0)
        {
            Cor_Battle = StartCoroutine(BattleWait(npc));
            return Cor_Battle;
        }
        else
        {
            Debug.Log($"{Name} 가 배틀 불가능");
            return null;
        }
    }

    IEnumerator BattleWait(NPC npc)
    {
        //BattleStateCor = StartCoroutine(BattleStateBusy());

        BattleCount++;
        SetState_BattleCount(BattleCount);
        if (Cor_Moving != null)
        {
            StopCoroutine(Cor_Moving);
        }
        LookAtTarget(npc.PlacementInfo.Place_Tile);

        var npcType = npc.GetType();
        npc.ActionPoint -= Data.battleAp;

        int battleMP = npc.Rank * 1;

        if (npcType == typeof(Adventurer) || npcType == typeof(Elf) || npcType == typeof(Wizard))
        {
            battleMP = npc.Rank * 4;
        }

        if (npcType == typeof(EventNPC))
        {
            battleMP = npc.Rank * 2;
        }

        if (npcType == typeof(QuestHunter))
        {
            battleMP = npc.Rank * 1;
        }

        int manaClamp = Mathf.Clamp(battleMP, 0, npc.Mana);
        if (manaClamp < 0)
        {
            manaClamp = 0;
        }
        npc.Mana -= manaClamp;

        //? 전투가 동시에 일어날 때, 죽고 사는 전투가 이전의 Nothing 공방보다 먼저 끝나는 경우가 존재함. 이 때 PlaceInfo가 사라지거나 하는 문제떄문에 먼저 저장
        string floorName = PlacementInfo.Place_Floor.LabelName;


        UI_EventBox.AddEventText($"★{floorName} - " +
            $"{npc.Name_Color} vs {Name_Color} {UserData.Instance.LocaleText("Battle_Start")}");


        BattleField.BattleResult result = 0;
        yield return BattleManager.Instance.ShowBattleField(npc, this, out result);

        BattleEvent(result, npc);
        switch (result)
        {
            case BattleField.BattleResult.Nothing:
                UI_EventBox.AddEventText($"★{floorName} - " +
                    $"{npc.Name_Color} vs {Name_Color} {UserData.Instance.LocaleText("Battle_End")}");
                GetBattlePoint(npc.Rank);
                break;

            case BattleField.BattleResult.Monster_Die:
                UI_EventBox.AddEventText($"★{floorName} - {Name_Color} {UserData.Instance.LocaleText("Battle_Lose")}");
                MonsterOutFloor();
                break;

            case BattleField.BattleResult.NPC_Die:
                UI_EventBox.AddEventText($"★{floorName} - {Name_Color} {UserData.Instance.LocaleText("Battle_Win")}");
                GetBattlePoint(npc.Rank * 2);

                if (TraitCheck(TraitGroup.Predation))
                {
                    HP_Max += 1;
                    HP += 1;
                }
                break;
        }

        BattleCount--;
        SetState_BattleCount(BattleCount);
        if (BattleCount == 0 && this.HP > 0)
        {
            MoveSelf();
        }

        if (manaClamp > 0)
        {
            Main.Instance.CurrentDay.AddMana(manaClamp);
            Main.Instance.ShowDM(manaClamp, Main.TextType.mana, transform);
        }
        

        //if (BattleStateCor != null && BattleCount == 0) //? 만약 전투가 Interval보다 빨리 끝나고 그게 마지막 전투였을 경우 빠르게 Standby
        //{
        //    StopCoroutine(BattleStateCor);
        //    PlacementState = PlacementState.Standby;
        //    BattleStateCor = null;
        //}
    }

    //Coroutine BattleStateCor;
    //IEnumerator BattleStateBusy()
    //{
    //    PlacementState = PlacementState.Busy;
    //    yield return UserData.Instance.Wait_GamePlay;
    //    yield return new WaitForSeconds(Data.battleInterval);
    //    PlacementState = PlacementState.Standby;
    //    BattleStateCor = null;
    //}


    void SetState_BattleCount(int battleCount)
    {
        if (battleCount >= Data.maxBattleCount)
        {
            PlacementState = PlacementState.Busy;
            GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
        }
        else
        {
            PlacementState = PlacementState.Standby;
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }


    void LookAtTarget(BasementTile _target)
    {
        if (Cor_moveAnimation != null)
        {
            StopCoroutine(Cor_moveAnimation);
        }
        transform.position = PlacementInfo.Place_Tile.worldPosition;

        var startPos = PlacementInfo.Place_Tile;
        Vector3 dir = _target.worldPosition - startPos.worldPosition;
        SetDirection(dir);

        anim.Play(Define.ANIM_Ready);
    }

    public void MonsterOutFloor()
    {
        var player = this as Player;
        if (player != null)
        {
            Debug.Log("플레이어 Die");
            GameManager.Placement.PlacementClear(this);
            return;
        }

        PlacementInfo.Place_Floor.MaxMonsterSize++;
        State = HP <= 0 ? MonsterState.Injury : MonsterState.Standby;
        GameManager.Placement.PlacementClear(this);
    }




    #region Battle
    public enum LevelUpEventType
    {
        Training,
        Battle,
    }
    public virtual void LevelUpEvent(LevelUpEventType levelUpType)
    {

    }
    public virtual void BattleEvent(BattleField.BattleResult result, NPC npc)
    {

    }


    public enum Evolution
    {
        None,
        Ready,
        Progress,
        Complete,
        Exclude,
    }
    public Evolution EvolutionState { get; set; }

    public int BattlePoint_Rank { get; set; }
    public int BattlePoint_Count { get; set; }


    public void GetBattlePoint(int _npcRank)
    {
        BattlePoint_Rank += _npcRank;
        BattlePoint_Count++;
        //Debug.Log($"{Name_KR}// 랭크포인트:{BattleCount_Rank} // 전투횟수:{BattleCount_Quantity}");

        if (BattlePoint_Count >= 5 || BattlePoint_Rank >= Mathf.Clamp(LV * 2, 4, 30) )
        {
            Debug.Log($"{Name_Color}.Lv{LV}가 레벨업");
            BattleLevelUp();
            BattlePoint_Rank = 0;
            BattlePoint_Count = 0;
        }
    }


    public void BattleLevelUp()
    {
        if (GetType() == typeof(Player)) return;

        LevelUpEvent(LevelUpEventType.Battle);

        if (LV >= Data.maxLv) return;
        GameManager.Monster.AddLevelUpEvent(this);
    }

    void Injury()
    {
        GameManager.Monster.RemoveLevelUpEvent(this);
        BattlePoint_Rank = 0;
        BattlePoint_Count = 0;
        GameManager.Monster.InjuryMonster++;
    }


    #endregion




    public void Recover(int mana)
    {
        //? 회복
        if (Main.Instance.Player_Mana >= mana)
        {
            Main.Instance.CurrentDay.SubtractMana(mana);
            HP = HP_Max;
            State = MonsterState.Standby;
            //Debug.Log("회복성공");
        }
        else
        {
            var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
            ui.Message = UserData.Instance.LocaleText("Message_No_Mana");
            //Debug.Log("마나부족");
        }
    }

    public void Training()
    {
        if (Main.Instance.Player_AP <= 0)
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            ui.Message = UserData.Instance.LocaleText("Message_No_AP");
            return;
        }
        if (Data.maxLv <= LV)
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            ui.Message = UserData.Instance.LocaleText("Message_MaxLv");
            return;
        }

        Main.Instance.Player_AP--;
        Debug.Log($"{Name_Color} 훈련진행");
        LevelUpEvent(LevelUpEventType.Training);
        LevelUp(true); ;
    }

    public void LevelUp(bool _showPopup)
    {
        if (Data.maxLv <= LV)
        {
            Debug.Log("최대레벨임");
            return;
        }

        if (_showPopup)
        {
            var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
            ui.TargetMonster(this);
        }

        LV++;

        HP_Max += TryStatUp(Data.up_hp, ref hp_chance);
        HP = HP_Max;

        ATK += TryStatUp(Data.up_atk, ref atk_chance);
        DEF += TryStatUp(Data.up_def, ref def_chance);
        AGI += TryStatUp(Data.up_agi, ref agi_chance);
        LUK += TryStatUp(Data.up_luk, ref luk_chance);
    }
    public void StatUp()
    {
        HP_Max += TryStatUp(Data.up_hp, ref hp_chance);
        ATK += TryStatUp(Data.up_atk, ref atk_chance);
        DEF += TryStatUp(Data.up_def, ref def_chance);
        AGI += TryStatUp(Data.up_agi, ref agi_chance);
        LUK += TryStatUp(Data.up_luk, ref luk_chance);
    }

    int TryStatUp(float origin, ref float probability)
    {
        int value = 0;

        //? 1보다 크면 일단 확정적으로 올려주고
        while (probability >= 1)
        {
            value++;
            probability--;
        }

        //? 1보다 작은값은 확률굴림
        if (probability > Random.value)
        {
            value++;
            probability = origin;
        }
        else
        {
            probability += origin;
        }

        return value;
    }




}


