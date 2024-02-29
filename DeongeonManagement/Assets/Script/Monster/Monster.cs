using System.Collections;
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

    public string Name_KR { get { return $"{name_Tag_Start}{Name}{name_Tag_End}"; } }
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

    public void Initialize_SaveData(Save_MonsterData _SaveData)
    {
        if (_SaveData == null) { Debug.Log($"���̺굥���� ���� : {name}"); return; }

        LV = _SaveData.LV;
        HP = _SaveData.HP;
        HP_Max = _SaveData.HP_MAX;

        ATK = _SaveData.ATK;
        DEF = _SaveData.DEF;
        AGI = _SaveData.AGI;
        LUK = _SaveData.LUK;

        hp_chance = _SaveData.HP_chance;
        atk_chance = _SaveData.ATK_chance;
        def_chance = _SaveData.DEF_chance;
        agi_chance = _SaveData.AGI_chance;
        luk_chance = _SaveData.LUK_chance;

        State = _SaveData.State;
        Mode = _SaveData.MoveMode;
        EvolutionState = _SaveData.Evolution;
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








    public abstract MonsterData Data { get; set; }
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
        if (Data == null) { Debug.Log($"������ ���� : {name}"); return; }

        Name = Data.Name_KR;
        LV = Data.LV;

        HP = Data.HP;
        HP_Max = Data.HP;

        ATK = Data.ATK;
        DEF = Data.DEF;
        AGI = Data.AGI;
        LUK = Data.LUK;

        hp_chance = Data.HP_chance;
        atk_chance = Data.ATK_chance;
        def_chance = Data.DEF_chance;
        agi_chance = Data.AGI_chance;
        luk_chance = Data.LUK_chance;
    }


    #endregion



    public virtual void TurnStart()
    {
        MoveSelf();
    }
    public virtual void MoveSelf()
    {
        Debug.Log("���� ����ִϸ��̼� �ٽ� ����");
        Cor_Moving = StartCoroutine(MoveCor());
    }

    #region MonsterMove
    Animator anim;

    protected Coroutine Cor_Moving { get; set; }
    Coroutine Cor_moveAnimation;

    public enum MoveType
    {
        Fixed,

        Moving,
    }

    public MoveType Mode { get; set; } = MoveType.Fixed;
    public void SetMoveType(MoveType _moveType)
    {
        Mode = _moveType;
    }


    protected IEnumerator MoveCor()
    {
        while (Main.Instance.Management == false && State == MonsterState.Placement)
        {
            float ranDelay = Random.Range(1.5f, 2.5f);
            switch (Mode)
            {
                case MoveType.Fixed:
                    break;

                case MoveType.Moving:
                    Moving(ranDelay);
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
                        Debug.Log($"���� ����������");
                    }
                    break;

                default:
                    Debug.Log($"{eventType.ToString()} : �ƹ��̺�Ʈ ����");
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
            //? ���� ������
            //transform.localRotation = Quaternion.Euler(0, 180, 0);
            transform.localScale = Vector3.one * sizeOffset;
        }
        else if (dir.x < 0)
        {
            //? ����
            //transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(-sizeOffset, sizeOffset, sizeOffset);
        }
        //else if (dir.y > 0)
        //{
        //    //? ��
        //    _monster.Anim_State = NPC.moveState.back;
        //}
        //else if (dir.y < 0)
        //{
        //    //? �Ʒ�
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
            Debug.Log($"{Name} �� ��Ʋ �Ұ���");
            return null;
        }
    }

    IEnumerator BattleWait(NPC npc)
    {
        BattleCount++;
        StartCoroutine(BattleStateBusy());
        if (Cor_Moving != null)
        {
            StopCoroutine(Cor_Moving);
        }
        LookAtTarget(npc.PlacementInfo.Place_Tile);

        npc.ActionPoint -= Data.Battle_AP;
        int battleMP = npc.Rank * 8;
        npc.Mana -= battleMP;


        UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� �����߻� : " +
            $"{npc.Name_KR} vs " +
            $"{Name_KR}");

        BattleField.BattleResult result = 0;
        yield return BattleManager.Instance.ShowBattleField(npc, this, out result);

        BattleEvent(result, npc);
        switch (result)
        {
            case BattleField.BattleResult.Nothing:
                UI_EventBox.AddEventText($"��{Name_KR} vs {npc.Name_KR} �� ������ ����Ǿ����ϴ�.");
                GetBattlePoint(npc.Rank);
                break;

            case BattleField.BattleResult.Monster_Die:
                UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� �������� �й��߽��ϴ�..");
                MonsterOutFloor();
                break;

            case BattleField.BattleResult.NPC_Die:
                UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� {npc.Name_KR} ���� �¸��Ͽ����ϴ�!");
                GetBattlePoint(npc.Rank * 2);
                break;
        }

        BattleCount--;
        if (BattleCount == 0)
        {
            MoveSelf();
        }

        Main.Instance.ShowDM(battleMP, Main.TextType.mana, transform);
    }

    IEnumerator BattleStateBusy()
    {
        PlacementState = PlacementState.Busy;
        yield return new WaitForSeconds(Data.Battle_Interval);
        PlacementState = PlacementState.Standby;
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
            Debug.Log("���ӿ���");
            GameManager.Placement.PlacementClear(this);
            return;
        }

        PlacementInfo.Place_Floor.MaxMonsterSize++;
        State = HP <= 0 ? MonsterState.Injury : MonsterState.Standby;
        GameManager.Placement.PlacementClear(this);
    }





    //protected IEnumerator SetBattleConfigure(NPC npc)
    //{
    //    UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� �����߻� : " +
    //        $"{npc.Name_KR} vs " +
    //        $"{Name_KR}");
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        GiveAndTakeOnce(this, npc);

    //        if (this.HP <= 0)
    //        {
    //            this.HP = 0;
    //            UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� �������� �й��߽��ϴ�..");
    //            Debug.Log("���� �й�");
    //            MonsterOutFloor();
    //            break;
    //        }

    //        if (npc.HP <= 0)
    //        {
    //            UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� {npc.Name_KR} {npc.Name_Index} ���� �¸��Ͽ����ϴ�!");
    //            Debug.Log("NPC �й�");
    //            break;
    //        }
    //    }
    //}

    //private void GiveAndTakeOnce(Monster monster, NPC npc)
    //{
    //    monster.HP -= Mathf.Clamp((npc.ATK - monster.DEF), 1, monster.HP);

    //    npc.HP -= Mathf.Clamp((monster.ATK - npc.DEF), 1, npc.HP);
    //    //Debug.Log($"��Ʋ �� : {monster.name}�� ���� ü�� : {monster.HP} / {npc.name}�� ���� ü�� : {npc.HP}");
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

    public int BattleCount_Rank { get; set; }
    public int BattleCount_Quantity { get; set; }


    public void GetBattlePoint(int _npcRank)
    {
        BattleCount_Rank += _npcRank;
        BattleCount_Quantity++;
        //Debug.Log($"{Name_KR}// ��ũ����Ʈ:{BattleCount_Rank} // ����Ƚ��:{BattleCount_Quantity}");

        if (BattleCount_Quantity >= 5 || BattleCount_Rank >= LV * 2)
        {
            Debug.Log($"{Name_KR}.Lv{LV}�� ������");
            BattleLevelUp();
            BattleCount_Rank = 0;
            BattleCount_Quantity = 0;
        }
    }


    public void BattleLevelUp()
    {
        if (GetType() == typeof(Player)) return;

        LevelUpEvent(LevelUpEventType.Battle);

        if (LV >= Data.MAXLV) return;
        GameManager.Monster.AddLevelUpEvent(this);
    }

    void Injury()
    {
        GameManager.Monster.RemoveLevelUpEvent(this);
        BattleCount_Rank = 0;
        BattleCount_Quantity = 0;
        GameManager.Monster.InjuryMonster++;
    }


    #endregion




    public void Recover(int mana)
    {
        //? ȸ��
        if (Main.Instance.Player_Mana >= mana)
        {
            Main.Instance.CurrentDay.SubtractMana(mana);
            HP = HP_Max;
            State = MonsterState.Standby;
            Debug.Log("ȸ������");
        }
        else
        {
            Debug.Log("��������");
        }
    }

    public void Training()
    {
        if (Main.Instance.Player_AP <= 0)
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            ui.Message = "�ൿ���� �����մϴ�.";
            return;
        }
        if (Data.MAXLV <= LV)
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            ui.Message = "�ִ뷹���� �����߽��ϴ�.";
            return;
        }

        Main.Instance.Player_AP--;
        Debug.Log($"{Name_KR} �Ʒ�����");
        LevelUpEvent(LevelUpEventType.Training);
        LevelUp(true); ;
    }

    public void LevelUp(bool _showPopup)
    {
        if (Data.MAXLV <= LV)
        {
            Debug.Log("�ִ뷹����");
            return;
        }

        if (_showPopup)
        {
            var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
            ui.TargetMonster(this);
        }

        LV++;

        HP_Max += TryStatUp(Data.HP_chance, ref hp_chance);
        HP = HP_Max;

        ATK += TryStatUp(Data.ATK_chance, ref atk_chance);
        DEF += TryStatUp(Data.DEF_chance, ref def_chance);
        AGI += TryStatUp(Data.AGI_chance, ref agi_chance);
        LUK += TryStatUp(Data.LUK_chance, ref luk_chance);
    }
    public void StatUp()
    {
        HP_Max += TryStatUp(Data.HP_chance, ref hp_chance);
        ATK += TryStatUp(Data.ATK_chance, ref atk_chance);
        DEF += TryStatUp(Data.DEF_chance, ref def_chance);
        AGI += TryStatUp(Data.AGI_chance, ref agi_chance);
        LUK += TryStatUp(Data.LUK_chance, ref luk_chance);
    }

    int TryStatUp(float origin, ref float probability)
    {
        int value = 0;

        //? 1���� ũ�� �ϴ� Ȯ�������� �÷��ְ�
        while (probability >= 1)
        {
            value++;
            probability--;
        }

        //? 1���� �������� Ȯ������
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


