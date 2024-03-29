using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IPlacementable
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

        StartCoroutine(ShowMonsterManagement());
    }

    IEnumerator ShowMonsterManagement()
    {
        var ui = Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management");
        yield return new WaitForEndOfFrame();

        ui.ShowDetail(this);
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



    public abstract void MonsterInit();
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
        MoveSelf();
        HP = HP_Max;
    }
    public virtual void MoveSelf()
    {
        Debug.Log("몬스터 무브애니메이션 다시 시작");
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
                    if (npc.Cor_Encounter == null)
                    {
                        npc.Cor_Encounter = StartCoroutine(npc.Encounter_ByMonster(this));
                        Debug.Log($"몬스터 선빵때리기");
                    }
                    break;

                default:
                    Debug.Log($"{eventType.ToString()} : 아무이벤트 없음");
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
            Debug.Log("층에 npc가 없으므로 이동하지 않음");
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
                    if (npc.Cor_Encounter == null)
                    {
                        npc.Cor_Encounter = StartCoroutine(npc.Encounter_ByMonster(this));
                        Debug.Log($"몬스터 선빵때리기");
                    }
                    break;

                default:
                    Debug.Log($"{eventType.ToString()} : 아무이벤트 없음");
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
            yield return null;
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
        if (this.HP > 0)
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
        BattleStateCor = StartCoroutine(BattleStateBusy());

        BattleCount++;

        if (Cor_Moving != null)
        {
            StopCoroutine(Cor_Moving);
        }
        LookAtTarget(npc.PlacementInfo.Place_Tile);

        var npcType = npc.GetType();
        npc.ActionPoint -= Data.battleAp;
        int battleMP = npc.Rank * 4;

        if (npcType == typeof(Adventurer) || npcType == typeof(Elf) || npcType == typeof(Wizard))
        {
            battleMP = npc.Rank * 10;
        }

        if (npcType == typeof(QuestHunter) || npcType == typeof(EventNPC))
        {
            battleMP = npc.Rank * 1;
        }

        npc.Mana -= battleMP;


        UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.LabelName} - " +
            $"{npc.Name_Color} vs {Name_Color} {UserData.Instance.GetLocaleText("Battle_Start")}");

        BattleField.BattleResult result = 0;
        yield return BattleManager.Instance.ShowBattleField(npc, this, out result);

        BattleEvent(result, npc);
        switch (result)
        {
            case BattleField.BattleResult.Nothing:
                UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.LabelName} - " +
                    $"{npc.Name_Color} vs {Name_Color} {UserData.Instance.GetLocaleText("Battle_End")}");
                GetBattlePoint(npc.Rank);
                break;

            case BattleField.BattleResult.Monster_Die:
                UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.LabelName} - {Name_Color} {UserData.Instance.GetLocaleText("Battle_Lose")}");
                MonsterOutFloor();
                break;

            case BattleField.BattleResult.NPC_Die:
                UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.LabelName} - {Name_Color} {UserData.Instance.GetLocaleText("Battle_Win")}");
                GetBattlePoint(npc.Rank * 2);
                break;
        }

        BattleCount--;
        if (BattleCount == 0)
        {
            MoveSelf();
        }

        Main.Instance.ShowDM(battleMP, Main.TextType.mana, transform);

        if (BattleStateCor != null) //? 만약 전투가 Interval보다 빨리 끝났을 경우.(바로 죽는다든지 인터벌이 ㅈㄴ길다든지)
        {
            StopCoroutine(BattleStateCor);
            PlacementState = PlacementState.Standby;
            BattleStateCor = null;
        }
    }

    Coroutine BattleStateCor;
    IEnumerator BattleStateBusy()
    {
        PlacementState = PlacementState.Busy;
        yield return new WaitForSeconds(Data.battleInterval);
        PlacementState = PlacementState.Standby;
        BattleStateCor = null;
    }

    void LookAtTarget(BasementTile _target)
    {
        if (Cor_moveAnimation != null)
        {
            StopCoroutine(Cor_moveAnimation);
            transform.position = PlacementInfo.Place_Tile.worldPosition;
        }

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
            Debug.Log("게임오버");
            GameManager.Placement.PlacementClear(this);
            return;
        }

        PlacementInfo.Place_Floor.MaxMonsterSize++;
        State = HP <= 0 ? MonsterState.Injury : MonsterState.Standby;
        GameManager.Placement.PlacementClear(this);
    }





    //protected IEnumerator SetBattleConfigure(NPC npc)
    //{
    //    UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 전투발생 : " +
    //        $"{npc.Name_KR} vs " +
    //        $"{Name_KR}");
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        GiveAndTakeOnce(this, npc);

    //        if (this.HP <= 0)
    //        {
    //            this.HP = 0;
    //            UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 {Name_KR} (이)가 전투에서 패배했습니다..");
    //            Debug.Log("몬스터 패배");
    //            MonsterOutFloor();
    //            break;
    //        }

    //        if (npc.HP <= 0)
    //        {
    //            UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 {Name_KR} (이)가 {npc.Name_KR} {npc.Name_Index} 에게 승리하였습니다!");
    //            Debug.Log("NPC 패배");
    //            break;
    //        }
    //    }
    //}

    //private void GiveAndTakeOnce(Monster monster, NPC npc)
    //{
    //    monster.HP -= Mathf.Clamp((npc.ATK - monster.DEF), 1, monster.HP);

    //    npc.HP -= Mathf.Clamp((monster.ATK - npc.DEF), 1, npc.HP);
    //    //Debug.Log($"배틀 상세 : {monster.name}의 남은 체력 : {monster.HP} / {npc.name}의 남은 체력 : {npc.HP}");
    //}


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
            ui.Message = UserData.Instance.GetLocaleText("Message_No_Mana");
            //Debug.Log("마나부족");
        }
    }

    public void Training()
    {
        if (Main.Instance.Player_AP <= 0)
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            ui.Message = UserData.Instance.GetLocaleText("Message_No_AP");
            return;
        }
        if (Data.maxLv <= LV)
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            ui.Message = UserData.Instance.GetLocaleText("Message_MaxLv");
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


