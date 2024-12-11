using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IPlacementable, I_BattleStat, I_TraitSystem
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

    public string Name_Color { get { return $"{name_Tag_Start}{CallName}{name_Tag_End}"; } }
    private string name_Tag_Start = "<b>";//"<color=#44ff44ff>";
    private string name_Tag_End = "</b>";//"</color>";

    public virtual string Detail_KR { get { return Data.detail; } }


    public string CallName { get { return string.IsNullOrEmpty(CustomName) ? Name : CustomName; } }
    public string CustomName { get; set; }

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

        CustomName = _LoadData.CustomName;

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

        if (_LoadData.traitCounter != null)
        {
            traitCounter = _LoadData.traitCounter.DeepCopy();
            traitCounter.monster = this;
        }
        LoadTraitList(_LoadData.currentTraitList);

        //Debug.Log($"훈련카운트 : {traitCounter.TrainingCounter}");

        if (_LoadData.unitEvent != null)
        {
            UnitDialogueEvent = _LoadData.unitEvent.DeepCopy();
        }
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

    public int B_HP { get => HP_Final - HP_Damaged; }
    public int B_HP_Max { get => HPMax_Final; }
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

    //[HideInInspector]
    public float hp_chance;
    [HideInInspector]
    public float atk_chance;
    [HideInInspector]
    public float def_chance;
    [HideInInspector]
    public float agi_chance;
    [HideInInspector]
    public float luk_chance;


    //? 받은 데미지. 일차가 바뀔 때 초기화되면 됨. 데미지를 받을 때 +되고 회복할 때 -가 되면 됨. 0밑으로 내려갈 수 없음
    public int HP_Damaged { get; set; }


    public int HP_Final { get { return Mathf.RoundToInt((HP + Trait_HP + HP_Bonus) * HP_Hospital); } }
    public int HPMax_Final { get { return Mathf.RoundToInt((HP_Max + Trait_HP + HP_Bonus) * HP_Hospital); } }


    int HP_Bonus { get { return GameManager.Buff.HpBonus; } }



    public int ATK_Final { get { return Mathf.RoundToInt((ATK + AllStat_Bonus + Trait_ATK + ATK_Bonus) * Orb_Bonus); } }
    public int DEF_Final { get { return Mathf.RoundToInt((DEF + AllStat_Bonus + Trait_DEF + DEF_Bonus) * Orb_Bonus); } }
    public int AGI_Final { get { return Mathf.RoundToInt((AGI + AllStat_Bonus + Trait_AGI + AGI_Bonus) * Orb_Bonus); } }
    public int LUK_Final { get { return Mathf.RoundToInt((LUK + AllStat_Bonus + Trait_LUK + LUK_Bonus) * Orb_Bonus); } }



    int ATK_Bonus { get { return 0; } }
    int DEF_Bonus { get { return 0; } }
    int AGI_Bonus { get { return 0; } }
    int LUK_Bonus { get { return 0; } }


    int AllStat_Bonus { get { return 
                Floor_Bonus + 
                Trait_Friend + 
                Trait_Veteran +
                TrainingCenter +
                GameManager.Buff.StatBonus; } }



    //? Technical Bonus
    float HP_Hospital { get { return GameManager.Technical.Get_Technical<Hospital>() != null ? 1.3f : 1; } }
    int TrainingCenter { get { return GameManager.Technical.Get_Technical<TrainingCenter>() != null ? 1 : 0; } }

    //? 전투의 오브 활성화 보너스
    float Orb_Bonus { get { return GameManager.Buff.CurrentBuff.Orb_red > 0 ? 1.2f : 1; } }

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

    //? Trait_Veteran
    int Trait_Veteran
    {
        get
        {
            if (TraitCheck(TraitGroup.VeteranC)) { return 1; }
            if (TraitCheck(TraitGroup.VeteranB)) { return 2; }
            if (TraitCheck(TraitGroup.VeteranA)) { return 3; }
            return 0;
        }
    }


    #region Trait_SaveData

    public class TraitCounter
    {
        public int BattleCounter;
        public int TrainingCounter;
        public int InjuryCounter;
        public int KillCounter;

        public int PlacementDays;
        public int StandbyDays;

        public int Days;

        public int CustomValueCounter;


        [Newtonsoft.Json.JsonIgnore]
        public Monster monster;


        public TraitCounter DeepCopy()
        {
            var copy = (TraitCounter)this.MemberwiseClone();
            return copy;
        }



        void ChangeValue()
        {
            monster.ChangeValue_TraitCounter();
        }

        public void AddDays()
        {
            Days++;
            ChangeValue();
        }


        public void AddBattleCounter()
        {
            BattleCounter++;
            ChangeValue();
            if (BattleCounter >= 10 && monster.Data.TraitableList.Contains(TraitGroup.VeteranC))
            {
                monster.AddTrait_Runtime(TraitGroup.VeteranC);
            }
            if (BattleCounter >= 15 && monster.Data.TraitableList.Contains(TraitGroup.VeteranB))
            {
                monster.AddTrait_Runtime(TraitGroup.VeteranB);
            }
            if (BattleCounter >= 20 && monster.Data.TraitableList.Contains(TraitGroup.VeteranA))
            {
                monster.AddTrait_Runtime(TraitGroup.VeteranA);
            }
        }

        public void AddTrainingCounter()
        {
            TrainingCounter++;
            ChangeValue();
            if (TrainingCounter >= 5 && monster.Data.TraitableList.Contains(TraitGroup.EliteC))
            {
                monster.AddTrait_Runtime(TraitGroup.EliteC);
            }
            if (TrainingCounter >= 10 && monster.Data.TraitableList.Contains(TraitGroup.EliteB))
            {
                monster.AddTrait_Runtime(TraitGroup.EliteB);
            }
            if (TrainingCounter >= 15 && monster.Data.TraitableList.Contains(TraitGroup.EliteA))
            {
                monster.AddTrait_Runtime(TraitGroup.EliteA);
            }
        }

        public void AddInjuryCounter()
        {
            InjuryCounter++;
            ChangeValue();
            if (InjuryCounter >= 3 && monster.Data.TraitableList.Contains(TraitGroup.ShirkingC))
            {
                monster.AddTrait_Runtime(TraitGroup.ShirkingC);
            }
            if (InjuryCounter >= 4 && monster.Data.TraitableList.Contains(TraitGroup.ShirkingB))
            {
                monster.AddTrait_Runtime(TraitGroup.ShirkingB);
            }
            if (InjuryCounter >= 5 && monster.Data.TraitableList.Contains(TraitGroup.ShirkingA))
            {
                monster.AddTrait_Runtime(TraitGroup.ShirkingA);
            }
        }
        public void AddKillCounter()
        {
            KillCounter++;
            ChangeValue();
            if (KillCounter >= 10 && monster.Data.TraitableList.Contains(TraitGroup.RuthlessC))
            {
                monster.AddTrait_Runtime(TraitGroup.RuthlessC);
            }
            if (KillCounter >= 15 && monster.Data.TraitableList.Contains(TraitGroup.RuthlessB))
            {
                monster.AddTrait_Runtime(TraitGroup.RuthlessB);
            }
            if (KillCounter >= 20 && monster.Data.TraitableList.Contains(TraitGroup.RuthlessA))
            {
                monster.AddTrait_Runtime(TraitGroup.RuthlessA);
            }
        }
        public void AddPlacementDays()
        {
            StandbyDays = 0;
            PlacementDays++;
            ChangeValue();
            if (PlacementDays >= 4 && monster.Data.TraitableList.Contains(TraitGroup.SurvivabilityC))
            {
                monster.AddTrait_Runtime(TraitGroup.SurvivabilityC);
            }
            if (PlacementDays >= 7 && monster.Data.TraitableList.Contains(TraitGroup.SurvivabilityB))
            {
                monster.AddTrait_Runtime(TraitGroup.SurvivabilityB);
            }
            if (PlacementDays >= 10 && monster.Data.TraitableList.Contains(TraitGroup.SurvivabilityA))
            {
                monster.AddTrait_Runtime(TraitGroup.SurvivabilityA);
            }
            if (PlacementDays >= 15 && monster.Data.TraitableList.Contains(TraitGroup.SurvivabilityS))
            {
                monster.AddTrait_Runtime(TraitGroup.SurvivabilityS);
            }
        }
        public void AddStandbyDays()
        {
            PlacementDays = 0;
            StandbyDays++;
            ChangeValue();
            if (StandbyDays >= 4 && monster.Data.TraitableList.Contains(TraitGroup.DiscreetC))
            {
                monster.AddTrait_Runtime(TraitGroup.DiscreetC);
            }
            if (StandbyDays >= 6 && monster.Data.TraitableList.Contains(TraitGroup.DiscreetB))
            {
                monster.AddTrait_Runtime(TraitGroup.DiscreetB);
            }
            if (StandbyDays >= 8 && monster.Data.TraitableList.Contains(TraitGroup.DiscreetA))
            {
                monster.AddTrait_Runtime(TraitGroup.DiscreetA);
            }
        }

        public void AddCustomValue(int value, bool init = false)
        {
            if (init)
            {
                CustomValueCounter = 0;
            }
            CustomValueCounter += value;
            ChangeValue();
        }
    }

    public TraitCounter traitCounter { get; set; }


    public void Init_TraitCounter()
    {
        if (traitCounter == null)
        {
            traitCounter = new TraitCounter();
            traitCounter.monster = this;
        }
    }


    public virtual void ChangeValue_TraitCounter() //? TraitCounter가 변화할 때 마다 호출되는 함수
    {

    }
    #endregion



    #region IStat & ITrait
    int Trait_HP { get => TraitList.Count > 0 ? Apply_HP() : 0; }
    int Trait_HP_Max { get => TraitList.Count > 0 ? Apply_HP_Max() : 0; }
    int Trait_ATK { get => TraitList.Count > 0 ? Apply_ATK() : 0; }
    int Trait_DEF { get => TraitList.Count > 0 ? Apply_DEF() : 0; }
    int Trait_AGI { get => TraitList.Count > 0 ? Apply_AGI() : 0; }
    int Trait_LUK { get => TraitList.Count > 0 ? Apply_LUK() : 0; }





    public List<ITrait> TraitList = new List<ITrait>();

    public bool AddTrait(ITrait trait) //? 동일한 특성 불가능
    {
        foreach (var item in TraitList)
        {
            if (trait.ID == item.ID)
            {
                return false;
            }
        }
        TraitList.Add(trait);
        return true;
    }
    public void AddTrait(TraitGroup traitID)
    {
        string className = $"Trait+{traitID.ToString()}";
        ITrait trait = Util.GetClassToString<ITrait>(className);
        AddTrait(trait);
    }
    void AddTrait_Runtime(TraitGroup traitID)
    {
        string className = $"Trait+{traitID.ToString()}";
        ITrait trait = Util.GetClassToString<ITrait>(className);
        if (AddTrait(trait))
        {
            Main.Instance.CurrentDay.AddTrait(1);
        }
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
            if (item.GetType() == trait && item is ITrait_Value)
            {
                ITrait_Value value = item as ITrait_Value;
                value.DoSomething();
            }
        }
    }
    public int GetSomething<T>(TraitGroup searchTrait, T current)
    {
        var trait = Util.GetTypeToString($"Trait+{searchTrait.ToString()}");
        foreach (var item in TraitList)
        {
            if (item.GetType() == trait && item is ITrait_Value)
            {
                ITrait_Value value = item as ITrait_Value;
                int tValue = value.GetSomething(current);
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
            if (trait is ITrait_Value value)
            {
                applyHp += value.ApplyHP(HP);
            }
        }
        return applyHp;
    }
    int Apply_HP_Max()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            if (trait is ITrait_Value value)
            {
                applyValue += value.ApplyHP_Max(HP_Max);
            }
        }
        return applyValue;
    }
    int Apply_ATK()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            if (trait is ITrait_Value value)
            {
                applyValue += value.ApplyATK(ATK);
            }
        }
        return applyValue;
    }
    int Apply_DEF()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            if (trait is ITrait_Value value)
            {
                applyValue += value.ApplyDEF(DEF);
            }
        }
        return applyValue;
    }
    int Apply_AGI()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            if (trait is ITrait_Value value)
            {
                applyValue += value.ApplyAGI(AGI);
            }
        }
        return applyValue;
    }
    int Apply_LUK()
    {
        int applyValue = 0;
        foreach (var trait in TraitList)
        {
            if (trait is ITrait_Value value)
            {
                applyValue += value.ApplyLUK(LUK);
            }
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
        if (loadData == null) return;


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
    public virtual void EvolutionMonster_Init() //? 첫 생성시 진화몹을 생성할 때 호출
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

        //hp_chance = Data.up_hp;
        //atk_chance = Data.up_atk;
        //def_chance = Data.up_def;
        //agi_chance = Data.up_agi;
        //luk_chance = Data.up_luk;

        Init_TraitCounter();
    }

    public void Evolution_Status() //? 진화하면 스탯보너스, 레벨유지
    {
        Main.Instance.CurrentDay.AddEvolution(1);

        Name = Data.labelName;
        //LV = Data.startLv;

        HP += Data.hp;
        HP_Max += Data.hp;

        ATK += Data.atk;
        DEF += Data.def;
        AGI += Data.agi;
        LUK += Data.luk;

        //hp_chance = Data.up_hp;
        //atk_chance = Data.up_atk;
        //def_chance = Data.up_def;
        //agi_chance = Data.up_agi;
        //luk_chance = Data.up_luk;

        Init_TraitCounter();
    }



    #endregion



    public virtual void TurnStart()
    {
        traitCounter.AddDays();

        switch (State)
        {
            case MonsterState.Standby:
                traitCounter.AddStandbyDays();
                break;

            case MonsterState.Placement:
                MoveSelf();
                //HP = HP_Max;
                HP_Damaged = 0;

                traitCounter.AddPlacementDays();
                break;

            case MonsterState.Injury:
                if (TraitCheck(TraitGroup.Reconfigure))
                {
                    //HP = HP_Max;
                    HP_Damaged = 0;
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

    #region Animation

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
        float delay = Data.moveSpeed * 0.5f; //? 높을수록 느림. 1칸 이동하는데 걸리는 시간이라고 보면 댐
        float interval = Data.ActionInterval * 0.5f; //? 얘가 NPC로 치면 ActionDelay
        yield return new WaitForSeconds(1); //? 가장 처음 동작은 npc가 도망칠 시간은 줘야되서 1초정도 기다리기(일반액션은 상관없음)

        while (Main.Instance.Management == false && State == MonsterState.Placement)
        {
            //float delay = Random.Range(1.5f, 2.5f);
            switch (Mode)
            {
                case MoveType.Fixed:
                    break;

                case MoveType.Wander:
                    Moving(delay);
                    break;

                case MoveType.Attack:
                    Moving_Attack(delay);
                    break;
            }
            yield return UserData.Instance.Wait_GamePlay;
            yield return new WaitForSeconds(delay + interval);
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
                    if (npc.Cor_Encounter == null && npc.B_HP > 0 && this.B_HP > 0)
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
                    if (npc.Cor_Encounter == null && npc.B_HP > 0 && this.B_HP > 0)
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
        while (timer < (duration))// * 0.95f))
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
        if (this.B_HP > 0 && npc.B_HP > 0)
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
        traitCounter.AddBattleCounter();
        Main.Instance.CurrentDay.AddBattle(1);

        BattleCount++;
        SetState_BattleCount(BattleCount);
        if (Cor_Moving != null)
        {
            StopCoroutine(Cor_Moving);
        }
        LookAtTarget(npc.PlacementInfo.Place_Tile);


        int battleMP = npc.Rank * 5;
        foreach (var item in npc.Data.NPC_TraitList)
        {
            if (item == TraitGroup.Militant)
            {
                battleMP = npc.Rank * 8;
            }
            if (item == TraitGroup.Civilian)
            {
                battleMP = npc.Rank * 4;
            }
            if (item == TraitGroup.Trample)
            {
                battleMP = npc.Rank * 10;
            }
        }

        battleMP += GameManager.Buff.BattleBonus;

        int manaClamp = Mathf.Clamp(battleMP, 0, npc.Mana);
        if (manaClamp < 0)
        {
            manaClamp = 0;
        }
        //npc.Mana -= manaClamp;
        //npc.ActionPoint -= Data.battleAp;
        npc.Change_Mana(-manaClamp);
        npc.Change_ActionPoint(-Data.battleAp);

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

                if (npc.TraitCheck(TraitGroup.Instructor))
                {
                    GetBattlePoint(npc.Rank * 2);
                }
                else
                {
                    GetBattlePoint(npc.Rank);
                }
                break;

            case BattleField.BattleResult.Monster_Die:
                UI_EventBox.AddEventText($"★{floorName} - {Name_Color} {UserData.Instance.LocaleText("Battle_Lose")}");
                MonsterOutFloor();

                Main.Instance.CurrentDay.AddDefeatMonster(1);
                break;

            case BattleField.BattleResult.NPC_Die:
                UI_EventBox.AddEventText($"★{floorName} - {Name_Color} {UserData.Instance.LocaleText("Battle_Win")}");
                GetBattlePoint(npc.Rank * 2);

                traitCounter.AddKillCounter();
                Main.Instance.CurrentDay.AddVictory(1);

                if (TraitCheck(TraitGroup.Predation))
                {
                    HP_Max += 1;
                    HP += 1;
                }
                break;
        }

        BattleCount--;
        SetState_BattleCount(BattleCount);
        if (BattleCount == 0 && this.B_HP > 0)
        {
            MoveSelf();
        }

        if (manaClamp > 0)
        {
            Main.Instance.CurrentDay.AddMana(manaClamp, Main.DayResult.EventType.Monster);
            Main.Instance.ShowDM(manaClamp, Main.TextType.mana, transform);
        }

        yield return UserData.Instance.Wait_GamePlay;
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
            GetComponentInChildren<SpriteRenderer>(true).color = new Color(1, 1, 1, 0.25f);
        }
        else
        {
            PlacementState = PlacementState.Standby;
            GetComponentInChildren<SpriteRenderer>(true).color = Color.white;
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
        State = B_HP <= 0 ? MonsterState.Injury : MonsterState.Standby;
        GameManager.Placement.PlacementClear(this);
    }




    #region Battle
    public enum LevelUpEventType
    {
        Training,
        Battle,
        Normal,
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
        int exp = _npcRank;

        if (TraitCheck(TraitGroup.DiscreetC))
        {
            exp = Mathf.RoundToInt(exp * 1.2f);
        }
        if (TraitCheck(TraitGroup.DiscreetB))
        {
            exp = Mathf.RoundToInt(exp * 1.4f);
        }
        if (TraitCheck(TraitGroup.DiscreetA))
        {
            exp = Mathf.RoundToInt(exp * 1.6f);
        }

        exp += GameManager.Buff.ExpBonus;

        Main.Instance.ShowDM(exp, Main.TextType.exp, transform);

        BattlePoint_Rank += exp;
        BattlePoint_Count++;
        //Debug.Log($"{Name_KR}// 랭크포인트:{BattleCount_Rank} // 전투횟수:{BattleCount_Quantity}");

        if (BattlePoint_Count >= 5 || BattlePoint_Rank >= Mathf.Clamp(LV * 2, 5, 50))
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
        if (TraitCheck(TraitGroup.SurvivabilityS))
        {
            HP_Damaged = (B_HP - 1);
            //HP = 1;
            State = MonsterState.Standby;
            return;
        }


        GameManager.Monster.RemoveLevelUpEvent(this);
        BattlePoint_Rank = 0;
        BattlePoint_Count = 0;
        //GameManager.Monster.InjuryMonster++;

        traitCounter.AddInjuryCounter();
    }


    #endregion



    #region Management - 관리 UI에서 하는 작업들 (레벨업, 이벤트, 훈련 부상 회복 등등)

    //? 몬스터 대화 리스트 / 저장 및 로드 해야되고 이게 있으면 이벤트 활성화하면댐
    public UnitEvent UnitDialogueEvent { get; set; } = new UnitEvent();

    public class UnitEvent
    {
        public HashSet<int> CurrentEventList;
        public HashSet<int> ClearEventList;


        public UnitEvent()
        {
            CurrentEventList = new HashSet<int>();
            ClearEventList = new HashSet<int>();
        }

        public UnitEvent DeepCopy()
        {
            var newEvent = new UnitEvent();

            newEvent.CurrentEventList = new HashSet<int>(CurrentEventList);
            newEvent.ClearEventList = new HashSet<int>(ClearEventList);

            return newEvent;
        }

        public void AddEvent(int dialogueNumber, bool allow_Duplicate = false)
        {
            if (allow_Duplicate)
            {
                CurrentEventList.Add(dialogueNumber);
            }
            else
            {
                if (ClearEventList.Contains(dialogueNumber) == false)
                {
                    CurrentEventList.Add(dialogueNumber);
                }
            }
        }

        public void AddEvent(UnitDialogueEventLabel dialogueNumber, bool allow_Duplicate = false)
        {
            AddEvent((int)dialogueNumber, allow_Duplicate);
        }

        public void ClearEvent(int dialogueNumber)
        {
            CurrentEventList.Remove(dialogueNumber);
            ClearEventList.Add(dialogueNumber);
        }
        public void ClearEvent(UnitDialogueEventLabel dialogueNumber)
        {
            ClearEvent((int)dialogueNumber);
        }

        public bool ClearCheck(int dialogueNumber)
        {
            return ClearEventList.Contains(dialogueNumber);
        }

        public bool ExistCurrentEvent()
        {
            return CurrentEventList.Count > 0 ? true : false;
        }

        public int GetDialogue(bool Callback_Clear)
        {
            int number = int.MaxValue;

            foreach (var item in CurrentEventList)
            {
                if (item < number)
                {
                    number = item;
                }
            }

            if (Callback_Clear) //? 호출과 동시에 클리어할꺼면 이 값을 true로
            {
                ClearEvent(number);
            }

            return number;
        }
    }



    public void Recover(int mana)
    {
        //? 회복
        if (Main.Instance.Player_Mana >= mana)
        {
            Main.Instance.CurrentDay.SubtractMana(mana, Main.DayResult.EventType.Monster);
            //HP = HP_Max;
            HP_Damaged = 0;
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

        LevelUp(true);
        if (GameManager.Technical.Get_Technical<TrainingCenter>() != null)
        {
            LevelUp(false);
        }

        //LevelUpEvent(LevelUpEventType.Training);

        traitCounter.AddTrainingCounter();
        AddCollectionPoint();
    }

    public void Statue_Cat()
    {
        if (Data.maxLv <= LV)
        {
            return;
        }

        LevelUp(true);
        //LevelUpEvent(LevelUpEventType.Training);
    }
    public void Statue_Demon()
    {
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);

        int ran = Random.Range(0, 5);
        switch (ran)
        {
            case 0:
                HP_Max += 5;
                HP = HP_Max;
                break;

            case 1:
                ATK++;
                break;

            case 2:
                DEF++;
                break;

            case 3:
                AGI++;
                break;

            case 4:
                LUK++;
                break;
        }
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

        LevelUpEvent(LevelUpEventType.Normal);
        LV++;

        float hp_value = Data.up_hp;
        float atk_value = Data.up_atk;
        float def_value = Data.up_def;
        float agi_value = Data.up_agi;
        float luk_value = Data.up_luk;

        if (TraitCheck(TraitGroup.EliteC))
        {
            hp_value *= 1.3f;
            atk_value *= 1.15f;
            def_value *= 1.15f;
            agi_value *= 1.08f;
            luk_value *= 1.08f;
        }
        if (TraitCheck(TraitGroup.EliteB))
        {
            hp_value += 1.6f;
            atk_value *= 1.3f;
            def_value *= 1.3f;
            agi_value *= 1.15f;
            luk_value *= 1.15f;
        }
        if (TraitCheck(TraitGroup.EliteA))
        {
            hp_value *= 2.0f;
            atk_value *= 1.5f;
            def_value *= 1.5f;
            agi_value *= 1.25f;
            luk_value *= 1.25f;
        }


        HP_Max += TryStatUp(hp_value, ref hp_chance);
        HP = HP_Max;

        ATK += TryStatUp(atk_value, ref atk_chance);
        DEF += TryStatUp(def_value, ref def_chance);
        AGI += TryStatUp(agi_value, ref agi_chance);
        LUK += TryStatUp(luk_value, ref luk_chance);
    }
    //public void StatUp()
    //{
    //    HP_Max += TryStatUp(Data.up_hp, ref hp_chance);
    //    ATK += TryStatUp(Data.up_atk, ref atk_chance);
    //    DEF += TryStatUp(Data.up_def, ref def_chance);
    //    AGI += TryStatUp(Data.up_agi, ref agi_chance);
    //    LUK += TryStatUp(Data.up_luk, ref luk_chance);
    //}

    int TryStatUp(float origin, ref float probability)
    {
        int value = 0;


        probability += origin;

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
            probability = 0;
        }

        return value;
    }



    public void StatUP(StatEnum stat, int value, bool uiOpen)
    {
        if (uiOpen)
        {
            var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
            ui.TargetMonster(this);
        }

        switch (stat)
        {
            case StatEnum.HP:
                HP += value;
                HP_Max += value;
                break;

            case StatEnum.ATK:
                ATK += value;
                break;

            case StatEnum.DEF:
                DEF += value;
                break;

            case StatEnum.AGI:
                AGI += value;
                break;

            case StatEnum.LUK:
                LUK += value;
                break;

            case StatEnum.LV:
                LV += value;
                break;
        }
    }
    #endregion


}

public enum StatEnum
{
    HP,
    ATK,
    DEF,
    AGI,
    LUK,
    LV,
}

