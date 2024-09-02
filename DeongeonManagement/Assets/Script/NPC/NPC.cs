using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;

public abstract class NPC : MonoBehaviour, IPlacementable, I_BattleStat, I_TraitSystem
{
    void Start()
    {
        anim = GetComponent<Animator>();
        characterBuilder = GetComponent<CharacterBuilder>();
        SetRandomClothes();
        Start_Setting();
        Init_Trait();
    }


    // �ν����� Ȯ�ο�
    public List<BasementTile> prioList;
    [System.Obsolete]
    void Update()
    {
        prioList = PriorityList;
    }


    protected virtual void Start_Setting()
    {

    }



    #region PixelEditor
    protected CharacterBuilder characterBuilder;
    protected abstract void SetRandomClothes();

    #endregion



    #region Animation
    Animator anim;
    public enum animState
    {
        none = 0,

        // running
        front = 1,
        left = 2,
        right = 3,
        back = 4,

        // interaction
        front_Action = 5,
        left_Action = 6,
        right_Action = 7,
        back_Action = 8,

        // battle
        front_Battle,
        left_Battle ,
        right_Battle ,
        back_Battle ,

        // trap
        Trap,

        // resting
        Resting,

        Idle,
        Ready,
    }
    private animState _animState;
    public animState Anim_State { get { return _animState; } 
        set 
        { 
            _animState = value;
            SetAnim_State(_animState);
        } }

    void SetAnim_State(animState state)
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null) return;
        }

        switch (state)
        {
            case animState.front:
            case animState.left:
                transform.localScale = new Vector3(-1, 1, 1);
                anim.Play("Running");
                break;

            case animState.back:
            case animState.right:
                transform.localScale = Vector3.one;
                anim.Play("Running");
                break;

            case animState.front_Action:
            case animState.left_Action:
                transform.localScale = new Vector3(-1, 1, 1);
                anim.Play("Interaction");
                break;

            case animState.back_Action:
            case animState.right_Action:
                transform.localScale = Vector3.one;
                anim.Play("Interaction");
                break;

            case animState.front_Battle:
            case animState.left_Battle:
                transform.localScale = new Vector3(-1, 1, 1);
                anim.Play("Ready");
                break;

            case animState.back_Battle:
            case animState.right_Battle:
                transform.localScale = Vector3.one;
                anim.Play("Ready");
                break;


            case animState.Idle:
                anim.SetTrigger("Idle");
                //anim.Play(Define.ANIM_Idle);
                break;

            case animState.Ready:
                anim.Play(Define.ANIM_Ready);
                break;
        }
    }



    //void SetAnim(moveState state)
    //{
    //    if (anim == null)
    //    {
    //        return;
    //    }
    //    //anim.SetInteger("move", (int)state);
    //    switch (state)
    //    {
    //        case moveState.front:
    //            anim.Play("walk_f");
    //            break;

    //        case moveState.left:
    //            anim.Play("walk_l");
    //            break;

    //        case moveState.right:
    //            anim.Play("walk_r");
    //            break;

    //        case moveState.back:
    //            anim.Play("walk_b");
    //            break;


    //        case moveState.front_Action:
    //            anim.Play("ing_f");
    //            break;

    //        case moveState.left_Action:
    //            anim.Play("ing_l");
    //            break;

    //        case moveState.right_Action:
    //            anim.Play("ing_r");
    //            break;

    //        case moveState.back_Action:
    //            anim.Play("ing_b");
    //            break;
    //    }
    //}


    #region MoveAnimation

    Coroutine Cor_MoveAnim;

    void PlacementMove_NPC(NPC npc, PlacementInfo newPlace, float duration)
    {
        if (Cor_MoveAnim != null)
        {
            StopCoroutine(Cor_MoveAnim);
        }
        Cor_MoveAnim = StartCoroutine(MoveUpdate(newPlace.Place_Tile, duration));

        GameManager.Placement.PlacementMove(this, newPlace);
    }

    IEnumerator MoveUpdate(BasementTile endPos, float duration)
    {
        var startPos = PlacementInfo.Place_Tile;
        Vector3 dir = endPos.worldPosition - startPos.worldPosition;
        //Debug.Log(dir);
        if (dir.x > 0)
        {
            //? ���� ������
            Anim_State = animState.right;
        }
        else if (dir.x < 0)
        {
            //? ����
            Anim_State = animState.left;
        }
        else
        {
            anim.Play("Running");
        }
        //else if (dir.y > 0) //? �̰� �Ⱦ��� ������ �� �� �� �� �̵��� �� ĳ���Ͱ� �� �� �� �� ���������� ���������� ���� �ȴ�
        //{
        //    //? ��
        //    Anim_State = animState.back;
        //}
        //else if (dir.y < 0)
        //{
        //    //? �Ʒ�
        //    Anim_State = animState.front;
        //}

        float dis = Vector3.Distance(startPos.worldPosition, endPos.worldPosition);

        float moveValue = dis / duration;
        float timer = 0;

        while (timer < duration)// && dis > 0.05f)
        {
            //yield return null;
            yield return UserData.Instance.Wait_GamePlay;

            timer += Time.deltaTime;
            transform.position += dir.normalized * moveValue * Time.deltaTime;
            //dis = Vector3.Distance(npc.transform.position, endPos.worldPosition);
        }

        transform.position = endPos.worldPosition;
        Cor_MoveAnim = null;
    }

    void LookInteraction(BasementTile endPos)
    {
        var startPos = PlacementInfo.Place_Tile;
        Vector3 dir = endPos.worldPosition - startPos.worldPosition;
        //Debug.Log(dir);
        if (dir.x > 0)
        {
            //? ���� ������
            Anim_State = NPC.animState.right_Action;
        }
        else if (dir.x < 0)
        {
            //? ����
            Anim_State = NPC.animState.left_Action;
        }
        else if (dir.y > 0)
        {
            //? ��
            Anim_State = NPC.animState.back_Action;
        }
        else if (dir.y < 0)
        {
            //? �Ʒ�
            Anim_State = NPC.animState.front_Action;
        }
    }
    void LookBattle(BasementTile endPos)
    {
        var startPos = PlacementInfo.Place_Tile;
        Vector3 dir = endPos.worldPosition - startPos.worldPosition;
        //Debug.Log(dir);
        if (dir.x > 0)
        {
            //? ���� ������
            Anim_State = NPC.animState.right_Battle;
        }
        else if (dir.x < 0)
        {
            //? ����
            Anim_State = NPC.animState.left_Battle;
        }
        //else if (dir.y > 0)
        //{
        //    //? ��
        //    Anim_State = NPC.moveState.back_Battle;
        //}
        //else if (dir.y < 0)
        //{
        //    //? �Ʒ�
        //    Anim_State = NPC.moveState.front_Battle;
        //}
    }

    #endregion
    #endregion







    #region IPlacementable
    [field:SerializeField]
    public PlacementInfo PlacementInfo { get; set; }
    public PlacementType PlacementType { get; set; }
    public PlacementState PlacementState { get; set; }
    public GameObject GetObject()
    {
        return this.gameObject;
    }
    public string Name_Color { get { return $"{name_Tag_Start}{Name}{Name_Index}{name_Tag_End}"; } }
    string name_Tag_Start = "<color=#ff4444ff>";
    string name_Tag_End = "</color>";

    public string Detail_KR { get { return Data.detail; } }

    public virtual void MouseClickEvent()
    {
        
    }
    public virtual void MouseMoveEvent()
    {
        //if (Main.Instance.Management == false) return;
    }
    public virtual void MouseExitEvent()
    {

    }
    public virtual void MouseDownEvent()
    {

    }
    public virtual void MouseUpEvent()
    {

    }
    #endregion




    #region PriorityList
    protected abstract Define.TileType[] AvoidTileType { get; set; }


    public abstract List<BasementTile> PriorityList { get; set; }


    protected enum PrioritySortOption
    {
        Random,
        SortByDistance,
    }

    protected abstract void SetPriorityList(PrioritySortOption option);


    protected void SortByDistance(List<BasementTile> targetList)
    {
        TileDistanceComparer comparer = new TileDistanceComparer(PlacementInfo.Place_Tile.worldPosition);
        targetList.Sort(comparer);
    }


    public bool isWellsCheck { get; set; }
    protected virtual void Add_Wells()
    {
        if (isWellsCheck) return;

        if (Mana <= (Data.MP / 2) || ActionPoint <= (Data.AP / 2) || HP <= (HP_MAX / 2))
        {
            var list = GetPriorityPick(typeof(Wells));
            AddList(list, AddPos.Front);
        }
    }

    public void SetPriorityListForPublic()
    {
        if (State == NPCState.Priority)
        {
            SetPriorityList(PrioritySortOption.SortByDistance);
        }
    }
    
    //public void OverWell_Interaction()
    //{
    //    SetPriorityList();
    //    State = StateRefresh();
    //}



    List<BasementTile> GetFloorObjectsAll(Define.TileType searchType = Define.TileType.Empty) //? Ÿ�� ��ü �������� ����Ʈ
    {
        var newList = PlacementInfo.Place_Floor.GetFloorObjectList(searchType);

        return RefinementList(newList);
    }

    protected enum AddPos
    {
        Back,
        Front,
    }
    protected List<BasementTile> GetPriorityPick(Type type) //? Ư�� class�� �������� ����Ʈ
    {
        List<BasementTile> allList = GetFloorObjectsAll();
        List<BasementTile> newList = new List<BasementTile>();

        for (int i = 0; i < allList.Count; i++)
        {
            if (allList[i].Original != null && type.IsAssignableFrom(allList[i].Original.GetType()))
            {
                newList.Add(allList[i]);
            }
        }

        newList = Util.ListShuffle(newList);
        return RefinementList(newList);
    }
    protected List<BasementTile> GetFacilityPick(Facility.FacilityEventType facilityType) //? Ư��Ÿ���� �۽Ǹ�Ƽ�� �������� ����Ʈ
    {
        List<BasementTile> allList = GetFloorObjectsAll(Define.TileType.Facility);
        List<BasementTile> newList = new List<BasementTile>();

        for (int i = 0; i < allList.Count; i++)
        {
            var facil = allList[i].Original as Facility;
            if (facil != null && facil.EventType == facilityType)
            {
                newList.Add(allList[i]);
            }
        }

        newList = Util.ListShuffle(newList);
        return RefinementList(newList);
    }

    protected void AddList(List<BasementTile> addList, AddPos pos = AddPos.Back)
    {
        if (PriorityList == null)
        {
            PriorityList = new List<BasementTile>();
        }

        switch (pos)
        {
            case AddPos.Front:
                var newList = addList;
                newList.AddRange(PriorityList);
                PriorityList = RefinementList(newList);
                break;

            case AddPos.Back:
                PriorityList.AddRange(addList);
                RefinementList(PriorityList);
                break;
        }
    }


    List<BasementTile> RefinementList(List<BasementTile> _list) //? ���Ա��� �ڱ��ڽ�, �ߺ������� ��� �����ϴ� �۾�
    {
        var newList = _list.Distinct().ToList();

        newList.Remove(PlacementInfo.Place_Tile);
        //newList.Remove(PlacementInfo.Place_Floor.Entrance.PlacementInfo.Place_Tile);
        //newList.Remove(PlacementInfo.Place_Floor.Exit.PlacementInfo.Place_Tile);
        //? ���� ������ ���Ա������� �������½����� �������� �̰� ���� �ȵɵ�? �׳� ���谡���� ����Ʈ ã���� ���Ա� ��ã���ϴ°� ������.

        return newList;
    }

    protected void PriorityRemove(BasementTile item)
    {
        if (PriorityList == null) return;

        PriorityList.Remove(item);
    }

    protected void PickToProbability(List<BasementTile> pick, float probability, AddPos pos = AddPos.Back)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue < probability)
        {
            AddList(pick);
        }
    }
    protected void RemoveToProbability(List<BasementTile> pick, float probability)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue < probability)
        {
            foreach (var item in pick)
            {
                PriorityRemove(item);
            }
        }
    }


    #endregion






    #region Ground

    protected bool inDungeon;

    public virtual void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = startPoint;
        GameManager.Placement.Visible(this);

        //Managers.Resource.Instantiate("UI/UI_StateBar", transform);

        StartCoroutine(MoveToPoint(endPoint));

        var dir = endPoint.x - startPoint.x;
        if (dir > 0)
        {
            Anim_State = animState.right;
        }
        else
        {
            Anim_State = animState.left;
        }
    }

    IEnumerator MoveToPoint(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        //float dis = Vector3.Distance(transform.position, point);

        while (true)
        {
            var changeDir = point - transform.position;
            if (Mathf.Sign(dir.x) != Mathf.Sign(changeDir.x))
            {
                yield return UserData.Instance.Wait_GamePlay;
                break;
            }

            transform.Translate(dir.normalized * Time.deltaTime * Speed_Ground);

            //dis = Vector3.Distance(transform.position, point);
            ////yield return null;
            yield return UserData.Instance.Wait_GamePlay;
        }

        UI_EventBox.AddEventText($"��{Name_Color} {UserData.Instance.LocaleText("Event_Enter")}");
        if (GameManager.Technical.Donation != null)
        {
            Main.Instance.CurrentDay.AddGold(5, Main.DayResult.EventType.Etc);
            Main.Instance.ShowDM(5, Main.TextType.gold, GameManager.Technical.Donation_Pos, 1);
        }

        inDungeon = true;
        FloorNext();
        Main.Instance.CurrentDay.AddVisit(1);
    }



    public void Arrival()
    {
        inDungeon = false;
        transform.position = Main.Instance.Dungeon.position;
        GameManager.Placement.Visible(this);

        if (Cor_Move != null)
        {
            StopCoroutine(Cor_Move);
        }
        if (Cor_MoveAnim != null)
        {
            StopCoroutine(Cor_MoveAnim);
        }

        StartCoroutine(MoveToHome(Main.Instance.Guild.position));
        Anim_State = animState.left;
    }

    IEnumerator MoveToHome(Vector3 point)
    {
        Vector3 dir = point - transform.position;

        while (true)
        {
            var changeDir = point - transform.position;
            if (Mathf.Sign(dir.x) != Mathf.Sign(changeDir.x))
            {
                yield return UserData.Instance.Wait_GamePlay;
                break;
            }

            transform.Translate(dir.normalized * Time.deltaTime * Speed_Ground);
            yield return UserData.Instance.Wait_GamePlay;
        }
        GameManager.NPC.InactiveNPC(this);
    }
    #endregion Ground


    #region DialogueEvent

    protected IEnumerator EventCor(DialogueName dialogueName, float dis = 1.5f)
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.position, Main.Instance.Dungeon.position) < dis);

        var renderer = GetComponentInChildren<SpriteRenderer>();
        int originLayer = renderer.sortingOrder;
        renderer.sortingOrder = 10;

        Camera.main.GetComponent<CameraControl>().ChasingTarget(transform.position, 2); //? ���⼭ �ϴ� ���� = ī�޶� �ٸ��� ����Ű�°� ����
        Managers.Dialogue.ShowDialogueUI(dialogueName, transform);

        anim.Play(Define.ANIM_Idle);

        yield return null;
        yield return UserData.Instance.Wait_GamePlay;

        Anim_State = Anim_State;

        renderer.sortingOrder = originLayer;
    }

    protected IEnumerator EventCor(string dialogueName, float dis = 1.5f)
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.position, Main.Instance.Dungeon.position) < dis);
        Managers.Dialogue.ShowDialogueUI(dialogueName, transform);

        anim.Play(Define.ANIM_Idle);

        yield return null;
        yield return UserData.Instance.Wait_GamePlay;

        Anim_State = Anim_State;
    }



    #endregion




    #region I_Battle Stat

    public int B_HP { get => HP; }
    public int B_HP_Max { get => HP_MAX; }
    public int B_ATK { get => ATK; }
    public int B_DEF { get => DEF; }
    public int B_AGI { get => AGI; }
    public int B_LUK { get => LUK; }

    #endregion


    #region Npc Status Property
    public SO_NPC Data { get; private set; }

    [field: SerializeField]
    private int name_index;
    public string Name_Index
    {
        get
        {
            if (name_index <= 0)
            {
                return string.Empty;
            }
            else
            {
                return $"_{name_index}";
            }
        }
    }

    public int Rank { get; private set; }
    public string Name { get; private set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int AGI { get; set; }
    public int LUK { get; set; }

    [field:SerializeField]
    public int HP { get; set; }
    public int HP_MAX { get; set; }
    //? HP_Runaway�� ����ĥ hp�� ���� ���ϴ°͵� ���
    [field: SerializeField]
    public int ActionPoint { get; set; }
    [field: SerializeField]
    public int Mana { get; set; }


    public float Speed_Ground { get; set; } //? Ŭ���� ����
    public float ActionDelay { get; set; } //? �������� ����


    public void SetData(SO_NPC data, int index)
    {
        Data = data;
        name_index = index;

        Rank = data.Rank;

        Name = data.labelName;
        ATK = data.ATK;
        DEF = data.DEF;
        AGI = data.AGI;
        LUK = data.LUK;

        HP = data.HP;
        HP_MAX = data.HP;
        ActionPoint = data.AP;
        Mana = data.MP;

        float speed = data.groundSpeed * 2 * UnityEngine.Random.Range(0.9f, 1.1f);
        float delay = data.actionDelay * UnityEngine.Random.Range(0.9f, 1.1f);

        Speed_Ground = speed;
        ActionDelay = delay;

        if (GetType() == typeof(EventNPC))
        {
            Speed_Ground = data.groundSpeed;
        }

        KillGold = Data.Rank * Random.Range(15, 31);
    }

    #endregion

    public int EventID { get; set; }


    public bool isReturn { get; set; } = false;
    public enum NPCState
    {
        Non,

        Priority,

        Next,

        Runaway,
        Return_Empty,
        Return_Satisfaction,
        Die,

        //Interaction,
        //Battle,
    }

    [SerializeField]
    NPCState _state;
    public NPCState State
    {
        get { return _state; }
        set 
        { 
            _state = value;
            Cor_Encounter = null;
            BehaviourByState();
        }
    }

    int FloorLevel { get; set; }

    public void FloorNext() //? ���������� �������� �Ա��� �������� �� ȣ��
    {
        //if (FloorLevel == 0)
        //{
        //    FloorLevel++;
        //}
        FloorLevel++;

        if (FloorLevel == Main.Instance.ActiveFloor_Basement)
        {
            //FloorLevel--;
            //Debug.Log($"{name}(��)�� {FloorLevel}������ ���̻� �ö� �� ����");
            State = NPCState.Return_Empty;
            isReturn = true;
            return;
        }

        // �� �̵��� ���� �� �ϴ� �ڷ�ƾ���� �������
        if (Cor_Move != null)
        {
            StopCoroutine(Cor_Move);
        }

        GameManager.Placement.PlacementClear(this);

        //? ������ġ�� ��ȯ
        //var info = Managers.Placement.GetRandomPlacement(this, Main.Instance.Floor[FloorLevel]); 

        UI_EventBox.AddEventText($"��{Name_Color} - {Main.Instance.Floor[FloorLevel].Exit.PlacementInfo.Place_Floor.LabelName} " +
            $"{UserData.Instance.LocaleText("Event_Next")}");
        //? �Ա����� ��ȯ
        var info_Entrance = new PlacementInfo(Main.Instance.Floor[FloorLevel], Main.Instance.Floor[FloorLevel].Exit.PlacementInfo.Place_Tile);
        GameManager.Placement.PlacementConfirm(this, info_Entrance);

        //FloorLevel++;
        //Debug.Log($"{name}(��)�� {FloorLevel}���� ����");

        SetPriorityList(PrioritySortOption.Random); //? �켱������ ���� ��Ž��

        if (PriorityList.Count > 0)
        {
            State = NPCState.Priority;
        }
        else
        {
            State = StateRefresh();
        }

    }
    public void FloorPrevious() //? ���������� �ö󰡴� �Ա��� �������� �� ȣ��
    {
        FloorLevel--;

        if (FloorLevel == 1)
        {
            FloorEscape();
            return;
        }


        // �� �̵��� ���� �� �ϴ� �ڷ�ƾ���� �������
        if (Cor_Move != null)
        {
            StopCoroutine(Cor_Move);
        }


        GameManager.Placement.PlacementClear(this);

        var info_Exit = new PlacementInfo(Main.Instance.Floor[FloorLevel - 1], Main.Instance.Floor[FloorLevel - 1].Entrance.PlacementInfo.Place_Tile);
        GameManager.Placement.PlacementConfirm(this, info_Exit);

        //SearchPreviousFloor(); // FloorPrevious�� �����°��� Return_Empty �ϳ���. ������ ��� Ż���̶�. �׷��ϱ� ���ư����� �߰���ġ�ϴ°� ����.
        SetPriorityList(PrioritySortOption.Random); //? �켱������ ���� ��Ž��

        if (PriorityList.Count > 0)
        {
            State = NPCState.Priority;
        }
        else
        {
            SearchPreviousFloor();
        }
    }

    public void FloorEscape() //? ���Ż�� - �ٷ� �������� ��
    {
        Arrival();
        NPC_Return();
    }

    public void FloorPortal(int hiddenFloor) //? Ư�������� �����̵�
    {
        UI_EventBox.AddEventText($"��{Name_Color} - {Main.Instance.Floor[hiddenFloor].Exit.PlacementInfo.Place_Floor.LabelName}" +
            $"{UserData.Instance.LocaleText("Event_Next")}");

        GameManager.Placement.PlacementClear(this);
        //? �Ա����� ��ȯ
        var info_Exit = new PlacementInfo(Main.Instance.Floor[hiddenFloor], Main.Instance.Floor[hiddenFloor].Exit.PlacementInfo.Place_Tile);
        GameManager.Placement.PlacementConfirm(this, info_Exit);

        SetPriorityList(PrioritySortOption.Random); //? �켱������ ���� ��Ž��
        if (PriorityList.Count > 0)
        {
            State = NPCState.Priority;
        }
        else
        {
            State = NPCState.Return_Empty;
        }
    }




    void SearchNextFloor()
    {
        PriorityList.Add(PlacementInfo.Place_Floor.Entrance.PlacementInfo.Place_Tile);
        MoveToTargetTile(PriorityList[0]);
    }
    void SearchPreviousFloor()
    {
        ////? ���� �칰�� �߰������� ���� �������� �칰�� ������ �칰 ���Ƚ���� 0���� ũ�ٸ� �칰�� ���ϵ���. �̿ܿ��� ������ ������ ����
        //if (isWellsCheck == false && PlacementInfo.Place_Floor.PickObjectOfType(typeof(Wells)) != null)
        //{
        //    var wells = PlacementInfo.Place_Floor.PickObjectOfType(typeof(Wells)) as Wells;
        //    if (wells != null && wells.InteractionOfTimes > 0)
        //    {
        //        PriorityList.Clear();
        //        PriorityList.Add(wells.PlacementInfo.Place_Tile);
        //        MoveToTargetTile(PriorityList[0]);
        //        return;
        //    }
        //}


        PriorityList.Clear();
        PriorityList.Add(PlacementInfo.Place_Floor.Exit.PlacementInfo.Place_Tile);
        MoveToTargetTile(PriorityList[0]);
    }



    void NPC_Return()
    {
        switch (State)
        {
            case NPCState.Runaway:
                UI_EventBox.AddEventText($"��{Name_Color} {UserData.Instance.LocaleText("Event_Exit_Runaway")}");
                Runaway_Base();
                break;

            case NPCState.Return_Empty:
                UI_EventBox.AddEventText($"��{Name_Color} {UserData.Instance.LocaleText("Event_Exit_Empty")}");
                Empty_Base();
                break;

            case NPCState.Return_Satisfaction:
                UI_EventBox.AddEventText($"��{Name_Color} {UserData.Instance.LocaleText("Event_Exit_Satisfaction")}");
                Satisfaction_Base();
                break;
        }
    }

    bool OneTimeCheck = false;
    void NPC_Inactive()
    {
        if (OneTimeCheck == false)
        {
            Die_Base();
            OneTimeCheck = true;
        }
    }

    void Runaway_Base()
    {
        Main.Instance.CurrentDay.AddRunaway(1);
        NPC_Runaway();

        var emotion = Managers.Resource.Instantiate("NPC/Emotions", transform);
        emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.GetSprite_SLA("Element_State", "Runaway");
    }
    void Empty_Base()
    {
        Main.Instance.CurrentDay.AddEmpty(1);
        NPC_Return_Empty();

        var emotion = Managers.Resource.Instantiate("NPC/Emotions", transform);
        emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.GetSprite_SLA("Element_State", "Bad");
    }
    void Satisfaction_Base()
    {
        AddCollectionPoint();

        if (Mana <= Data.MP / 4)
        {
            Main.Instance.CurrentDay.AddSatisfaction(1);
            NPC_Return_Satisfaction();

            var emotion = Managers.Resource.Instantiate("NPC/Emotions", transform);
            emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.GetSprite_SLA("Element_State", "Perfect");
        }
        else
        {
            Main.Instance.CurrentDay.AddNonSatisfaction(1);
            NPC_Return_NonSatisfaction();

            var emotion = Managers.Resource.Instantiate("NPC/Emotions", transform);
            emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.GetSprite_SLA("Element_State", "Good");
        }
    }
    void Die_Base()
    {
        Main.Instance.CurrentDay.AddKill(1);
        NPC_Die();
    }

    public void AddCollectionPoint()
    {
        var collection = CollectionManager.Instance.Get_Collection(Data);
        if (collection != null)
        {
            collection.AddPoint();
        }
    }


    protected abstract void NPC_Runaway();
    protected abstract void NPC_Return_Empty();
    protected virtual void NPC_Return_Satisfaction()
    {
        int value = Mathf.RoundToInt(Data.Rank * 1.6f);

        Main.Instance.CurrentDay.AddPop(value);
        Main.Instance.ShowDM(value, Main.TextType.pop, transform, 1);
    }
    protected virtual void NPC_Return_NonSatisfaction() //? RoundToInt����  0.5�� 0��  �׷��� 0.5���� �� ū���ڷ��ؾߵǼ� 0.6������
    {
        int value = Mathf.RoundToInt(Data.Rank * 0.6f);

        Main.Instance.CurrentDay.AddPop(value);
        Main.Instance.ShowDM(value, Main.TextType.pop, transform, 1);

        Main.Instance.CurrentDay.AddDanger(value);
        Main.Instance.ShowDM(value, Main.TextType.danger, transform, 1);
    }

    public virtual int KillGold { get; set; }
    protected abstract void NPC_Die();
    protected abstract void NPC_Captive();

    public virtual int RunawayHpRatio { get; set; } = 4;

    public NPCState StateRefresh()
    {
        if (!inDungeon)
        {
            return NPCState.Non;
        }

        NPCState prevState = State;

        if (HP <= 0)
        {
            return NPCState.Die;
        }

        if (ActionPoint <= 0 || Mana <= 0)
        {
            if (TraitCheck(TraitGroup.Indomitable) == false)
            {
                return NPCState.Return_Satisfaction;
            }
        }

        if (HP < (HP_MAX / RunawayHpRatio))
        {
            if (TraitCheck(TraitGroup.Indomitable) == false)
            {
                return NPCState.Runaway;
            }
        }


        PriorityList.RemoveAll(r => r.tileType_Original == Define.TileType.Empty || r.Original.PlacementState == PlacementState.Busy);
        PriorityList.RemoveAll(r => r.tileType_Original == Define.TileType.NPC);


        if (PriorityList.Count == 0)
        {
            SetPriorityList(PrioritySortOption.SortByDistance);
            PriorityList.RemoveAll(r => r.tileType_Original == Define.TileType.Empty || r.Original.PlacementState == PlacementState.Busy);
            if (PriorityList.Count == 0)
            {
                return isReturn ? NPCState.Return_Empty : NPCState.Next;
                //return NPCState.Next;
            }
            else
            {
                return NPCState.Priority;
            }
        }



        if (PlacementInfo.Place_Floor.FloorIndex != 0 && PriorityList[0].Original == PlacementInfo.Place_Floor.Entrance.PlacementInfo.Place_Tile.Original)
        {
            return NPCState.Next;
        }

        if (PriorityList[0].Original == PlacementInfo.Place_Floor.Exit.PlacementInfo.Place_Tile.Original)
        {
            return NPCState.Return_Empty;
        }

        if (prevState == NPCState.Return_Empty)
        {
            return NPCState.Return_Empty;
        }
        if (prevState == NPCState.Return_Satisfaction)
        {
            return NPCState.Return_Satisfaction;
        }
        if (prevState == NPCState.Runaway)
        {
            return NPCState.Runaway;
        }

        return NPCState.Priority;
    }


    void BehaviourByState()
    {
        switch (State)
        {
            case NPCState.Next:
                if (PriorityList.Count > 0)
                {
                    MoveToTargetTile(PriorityList[0]);
                }
                else
                {
                    SearchNextFloor();
                }
                break;

            case NPCState.Priority:
                MoveToTargetTile(PriorityList[0]);
                break;

            case NPCState.Return_Empty:
                SearchPreviousFloor();
                break;

            case NPCState.Return_Satisfaction:
                SearchPreviousFloor();
                break;

            case NPCState.Runaway:
                SearchPreviousFloor();
                break;

            case NPCState.Die:
                NPC_Inactive();
                break;
        }
    }





    public Coroutine Cor_Encounter { get; set; }
    Coroutine Cor_Move;

    void Cor_Move_Reset()
    {
        if (Cor_Move != null)
        {
            StopCoroutine(Cor_Move);
        }
        Cor_Move = null;
    }

    public void MoveToTargetTile(BasementTile target)
    {
        // ���� ��ǥ������ ���� �ִ� Ÿ���̶�� ���⼭ �ٷ� ��ȣ�ۿ��� �Ѵ��� ���Ͻ��ѹ�����
        if (target == PlacementInfo.Place_Tile)
        {
            if (target.tileType_Original == Define.TileType.Empty || target.tileType_Original == Define.TileType.NPC)
            {
                State = StateRefresh();
            }
            else
            {
                var encount = target.TryPlacement(this, true);
                EncountOver(encount, target);
            }
            return;
        }


        bool pathFind = false;
        bool pathRefind = false;
        List<BasementTile> path;

        //? ��ã��ÿ� IWall�� ��ӹ��� ����� ���Խ�ų�� ������ ���� �κ�
        if (target.Original != null && target.Original as IWall != null)
        {
            path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target, AvoidTileType, out pathFind, BasementFloor.PathFindingType.Allow_Wall);

            if (!pathFind) //? ���� ��ã������ ��ֹ����� ã�� �ѹ��� ��ȸ�� �� ��.
            {
                path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target, out pathRefind, BasementFloor.PathFindingType.Allow_Wall);
            }
        }
        else
        {
            path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target, AvoidTileType, out pathFind);

            //Debug.Log($"�켱���� ��ã�� ��� : {pathFind}");

            if (!pathFind) //? ���� ��ã������ ��ֹ����� ã�� �ѹ��� ��ȸ�� �� ��.
            {
                path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target, out pathRefind);
                //Debug.Log($"�Ϲ� ��ã�� ��� : {pathRefind}");
            }
        }



        if (Cor_Move != null)
        {
            //Debug.Log("�ߺ��ڷ�ƾ ����");
            StopCoroutine(Cor_Move);
        }

        if (pathFind || pathRefind)
        {
            Cor_Move = StartCoroutine(DungeonMoveToPath(path, pathRefind));
        }
        else
        {
            //Debug.Log("��ã�� ���� / ��Ȳ" + Time.time);
            Cor_Move = StartCoroutine(Wandering());
        }
    }





    IEnumerator DungeonMoveToPath(List<BasementTile> path, bool overlap = false)
    {
        for (int i = 1; i < path.Count; i++)
        {
            //Debug.Log(ActionDelay + Name_KR);
            yield return UserData.Instance.Wait_GamePlay;
            yield return new WaitForSeconds(ActionDelay);
            yield return UserData.Instance.Wait_GamePlay;

            //Debug.Log($"{name} ��ǥ : {PlacementInfo.Place_Floor.Floor} / {PlacementInfo.Place_Tile.index} / ���� : {State} / \n" +
            //    $"����Ÿ��Ÿ�� : {path[i].tileType} /��ǥ : {path[i].floor.Floor} / {path[i].index} / �̺�ƮŸ�� : {encount}\n" +
            //    $"���� ����Ʈ �� :{PriorityList.Count} / 0�� Ÿ�� : {PriorityList[0]} / �Ա� Ÿ�� {PlacementInfo.Place_Floor.Entrance}");
            var encount = path[i].TryPlacement(this, overlap);
            if (EncountOver(encount, path[i]))
            {
                yield return new WaitForEndOfFrame();
                //Debug.Log("�̰� ������ �ȉ�");
                break;
            }
            else
            {
                var next = new PlacementInfo(PlacementInfo.Place_Floor, path[i]);
                PlacementMove_NPC(this, next, ActionDelay);
            }
        }
        yield return new WaitForEndOfFrame();
        Cor_Move = null;
        State = StateRefresh(); //? ��� ��θ� Ž���ߴµ� �̺�Ʈ�� �߻��� ���ߴ�? ���� �̻��� �ִ°�. ���¾�����Ʈ �ʿ�
    }

    bool EncountOver(Define.PlaceEvent encount, BasementTile tile)
    {
        var next = new PlacementInfo(PlacementInfo.Place_Floor, tile);

        switch (encount)
        {
            case Define.PlaceEvent.Battle:
                Cor_Move_Reset();
                PlacementState = PlacementState.Busy;
                LookBattle(next.Place_Tile);
                Cor_Encounter = StartCoroutine(Encounter_Monster(tile));
                return true;

            case Define.PlaceEvent.Interaction:
                Cor_Move_Reset();
                PlacementState = PlacementState.Busy;
                LookInteraction(next.Place_Tile);
                Cor_Encounter = StartCoroutine(Encounter_Interaction(tile));
                return true;


            case Define.PlaceEvent.Event:
                Cor_Move_Reset();
                PlacementMove_NPC(this, next, ActionDelay);
                PlacementState = PlacementState.Busy;
                Cor_Encounter = StartCoroutine(Encounter_Event(tile));
                return true;



            case Define.PlaceEvent.Avoid:
                avoidCount++;
                if (avoidCount > 3)
                {
                    Cor_Move_Reset();
                    Cor_Move = StartCoroutine(Wandering());
                    avoidCount = 0;
                    return true;
                }
                State = StateRefresh();
                return true;

            case Define.PlaceEvent.Nothing:

                State = StateRefresh();
                return true;

            //case Define.PlaceEvent.Entrance:
            //    break;
            //case Define.PlaceEvent.Exit:
            //    break;


            case Define.PlaceEvent.Placement:
                break;


            default:
                break;
        }

        return false;

    }


    IEnumerator Encounter_Interaction(BasementTile tile)
    {
        Facility facility = tile.Original as Facility;

        if (facility)
        {
            yield return facility.NPC_Interaction(this);
            yield return new WaitForEndOfFrame();
            State = StateRefresh();
            PlacementState = PlacementState.Standby;
        }
    }
    IEnumerator Encounter_Event(BasementTile tile) //? FacilityEventType.NPC_Event �� �ڵ� State������ ������ �� �������� �������. ��!!!!!
    {
        Facility facility = tile.Original as Facility;

        if (facility)
        {
            yield return facility.NPC_Interaction(this);
            yield return new WaitForEndOfFrame();
            PlacementState = PlacementState.Standby;
        }
    }

    IEnumerator Encounter_Monster(BasementTile tile)
    {
        var monster = tile.Original as Monster;

        if (monster)
        {
            //Debug.Log("��Ʋ ����");
            yield return monster.Battle(this);
            //Debug.Log("��Ʋ ����");
            yield return new WaitForEndOfFrame();
            State = StateRefresh();
            PlacementState = PlacementState.Standby;
        }
    }

    public IEnumerator Encounter_ByMonster(Monster monster)
    {
        if (Cor_Move != null)
        {
            StopCoroutine(Cor_Move);
        }
        Cor_Move = null;

        //State = NPCState.Battle;

        //Debug.Log("��Ʋ ����");
        yield return monster.Battle(this);
        //Debug.Log("��Ʋ ����");
        yield return new WaitForEndOfFrame();
        State = StateRefresh();
        PlacementState = PlacementState.Standby;
    }



    int avoidCount = 0;
    IEnumerator Wandering()
    {
        yield return new WaitForSeconds(ActionDelay);

        BasementTile newTile;
        int dir = UnityEngine.Random.Range(0, 5);
        switch (dir)
        {
            case 0:
                newTile = PlacementInfo.Place_Floor.MoveUp(this, PlacementInfo.Place_Tile);
                break;

            case 1:
                newTile = PlacementInfo.Place_Floor.MoveDown(this, PlacementInfo.Place_Tile);
                break;

            case 2:
                newTile = PlacementInfo.Place_Floor.MoveLeft(this, PlacementInfo.Place_Tile);
                break;

            case 3:
                newTile = PlacementInfo.Place_Floor.MoveRight(this, PlacementInfo.Place_Tile);
                break;

            default:
                newTile = null;
                break;
        }

        if (newTile != null)
        {
            //Debug.Log("��ã�� ���� / �����̵�" + Time.time);
            var next = new PlacementInfo(PlacementInfo.Place_Floor, newTile);
            PlacementMove_NPC(this, next, ActionDelay);
        }
        yield return new WaitForEndOfFrame();
        Cor_Move = null;
        //Debug.Log("��ã�� ���� / ���»��ΰ�ħ");
        //? �̰� ap����Ʈ �� ���̸� �ⱸ���� ������ �̵��������ϴ� ��Ȳ ����. �ٸ� ī��Ʈ�� ��Ȳ�� �����̻��ϸ� �� �޼���� �ؾ��ҵ�
        //ActionPoint--;
        //Debug.Log(ActionPoint);
        State = StateRefresh();
    }





    #region NPC_Trait
    public List<ITrait> TraitList = new List<ITrait>();

    void Init_Trait()
    {
        foreach (var item in Data.NPC_TraitList)
        {
            AddTrait(item);
        }
    }

    public bool AddTrait(ITrait trait) //? ������ Ư�� �Ұ���
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
    //void AddTrait_Runtime(TraitGroup traitID)
    //{
    //    string className = $"Trait+{traitID.ToString()}";
    //    ITrait trait = Util.GetClassToString<ITrait>(className);
    //    if (AddTrait(trait))
    //    {
    //        Main.Instance.CurrentDay.AddTrait(1);
    //    }
    //}

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



    #endregion


}
//public enum TagGroup //? ���� �׳� �±׶�� �����Ѵٸ� ���� 4���� ���� �� ���� ���ʽ��±�
//{
//    //? Bonus
//    ������ = 1000,
//    ��������,
//    ������,
//    ������ɲ�,

//    //? Weak
//    ���� = 5000,
//    ������,
//    ��������,

//    //? Invalid
//    ����� = 9000,
//}

public enum InteractionGroup
{
    Interaction_Nothing = -100,

    Interaction_Monster = 0,

    Interaction_Herb = 100,
    Interaction_Mineral = 200,
    Interaction_Treasure = 300,

    Interaction_Trap = 400,

    Interaction_Artifact = 900,

    Interaction_NPC_Util = 1000,


}