using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class NPC : MonoBehaviour, IPlacementable
{
    void Start()
    {
        Initialize_Status();
        anim = GetComponent<Animator>();
    }
    //void Update()
    //{

    //}



    #region Animation
    Animator anim;
    protected enum moveState
    {
        none = 0,
        front = 1,
        left = 2,
        right = 3,
        back = 4,
    }
    private moveState _animState;
    protected moveState Anim_State { get { return _animState; } 
        set 
        { 
            _animState = value;
            SetAnim(_animState);
        } }



    void SetAnim(moveState state)
    {
        if (anim == null)
        {
            return;
        }
        anim.SetInteger("move", (int)state);
    }

    #endregion







    #region IPlacementable
    public Define.PlacementType PlacementType { get; set; }
    public PlacementInfo PlacementInfo { get; set; }
    public GameObject GetObject()
    {
        return this.gameObject;
    }
    public string Name_KR { get { return $"{name_Tag_Start}{Name}_{Name_Index}{name_Tag_End}"; } }
    string name_Tag_Start = "<color=#ff4444ff>";
    string name_Tag_End = "</color>";
    #endregion




    #region PriorityList
    protected abstract Define.TileType[] AvoidTileType { get; set; }
    public abstract List<BasementTile> PriorityList { get; set; }
    protected abstract void SetPriorityList();


    protected List<BasementTile> GetFloorObjectsAll(Define.TileType searchType = Define.TileType.Empty) //? Ÿ�� ��ü �������� ����Ʈ
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
            if (allList[i].placementable.GetType() == type)
            {
                newList.Add(allList[i]);
            }
        }

        newList = Util.ListShuffle(newList);
        return RefinementList(newList);
    }
    protected List<BasementTile> GetFacilityPick(Facility.FacilityType facilityType) //? Ư��Ÿ���� �۽Ǹ�Ƽ�� �������� ����Ʈ
    {
        List<BasementTile> allList = GetFloorObjectsAll(Define.TileType.Facility);
        List<BasementTile> newList = new List<BasementTile>();

        for (int i = 0; i < allList.Count; i++)
        {
            var facil = allList[i].placementable as Facility;
            if (facil != null && facil.Type == facilityType)
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

    #endregion






    #region Ground

    bool inDungeon;

    public void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = startPoint;
        GameManager.Placement.Visible(this);

        Managers.Resource.Instantiate("UI/UI_StateBar", transform);

        StartCoroutine(MoveToPoint(endPoint));
        Anim_State = moveState.right;
    }

    IEnumerator MoveToPoint(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        float dis = Vector3.Distance(transform.position, point);

        while (dis > 0.1f)
        {
            transform.Translate(dir.normalized * Time.deltaTime * Speed_Ground);
            dis = Vector3.Distance(transform.position, point);
            yield return null;
        }

        UI_EventBox.AddEventText($"��{Name_KR} (��)�� ������ ����");
        inDungeon = true;
        FloorNext();
    }



    public void Arrival()
    {
        inDungeon = false;
        transform.position = GameManager.NPC.dungeonEntrance.position;
        GameManager.Placement.Visible(this);
        StartCoroutine(MoveToHome(GameManager.NPC.guild.position));
        Anim_State = moveState.left;
    }

    IEnumerator MoveToHome(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        float dis = Vector3.Distance(transform.position, point);

        while (dis > 0.1f)
        {
            transform.Translate(dir.normalized * Time.deltaTime * Speed_Ground);
            dis = Vector3.Distance(transform.position, point);
            yield return null;
        }
        GameManager.NPC.InactiveNPC(this);
    }
    #endregion Ground





    #region Npc Status Property
    public int Name_Index { get; set; }
    public string Name { get; set; }
    public int LV { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int AGI { get; set; }
    public int LUK { get; set; }



    public int HP { get; set; }
    private int _HP_Origin;
    public int ActionPoint { get; set; }
    public int Mana { get; set; }



    public float Speed_Ground { get; set; } //? Ŭ���� ����
    public float ActionDelay { get; set; } //? �������� ����


    protected abstract void Initialize_Status();

    protected void SetStatus(string name, int lv, int atk, int def, int agi, int luk, int hp, int ap, int mp, float speed, float delay)
    {
        Name = name; LV = lv;  
        
        ATK = atk; DEF = def;
        AGI = agi; LUK = luk;

        HP = hp; ActionPoint = ap; Mana = mp;
        
        Speed_Ground = speed; ActionDelay = delay;

        _HP_Origin = hp;
    }

    #endregion



    public enum NPCState
    {
        Next,

        Priority,

        Runaway,
        Return,

        Die,

        Interaction,

        Battle,

        Non,
    }

    NPCState _state;
    public NPCState State
    {
        get { return _state; }
        set 
        { 
            _state = value;
            BehaviourByState();
        }
    }

    int FloorLevel { get; set; }

    void FloorNext() //? ���������� �������� �Ա��� �������� �� ȣ��
    {
        if (FloorLevel == 3)
        {
            FloorLevel++;
        }

        if (FloorLevel == Main.Instance.ActiveFloor_Basement)
        {
            Debug.Log($"{name}(��)�� {FloorLevel}������ ���̻� �ö� �� ����");
            State = NPCState.Return;
            return;
        }

        GameManager.Placement.PlacementClear(this);

        //? ������ġ�� ��ȯ
        //var info = Managers.Placement.GetRandomPlacement(this, Main.Instance.Floor[FloorLevel]); 
        //? �Ա����� ��ȯ
        var info_Entrance = new PlacementInfo(Main.Instance.Floor[FloorLevel], Main.Instance.Floor[FloorLevel].Exit.PlacementInfo.Place_Tile);
        GameManager.Placement.PlacementConfirm(this, info_Entrance);

        FloorLevel++;
        //Debug.Log($"{name}(��)�� {FloorLevel}���� ����");

        SetPriorityList(); //? �켱������ ���� ��Ž��

        if (PriorityList.Count > 0)
        {
            State = NPCState.Priority;
        }
        else
        {
            State = StateRefresh();
        }
        
    }
    void FloorPrevious() //? ���������� �ö󰡴� �Ա��� �������� �� ȣ��
    {
        if (FloorLevel == 5)
        {
            FloorLevel--;
        }

        FloorLevel--;

        if (FloorLevel == 0)
        {
            FloorEscape();
            return;
        }

        //Debug.Log($"{name}(��)�� {FloorLevel}���� ����");

        GameManager.Placement.PlacementClear(this);

        var info_Exit = new PlacementInfo(Main.Instance.Floor[FloorLevel - 1], Main.Instance.Floor[FloorLevel - 1].Entrance.PlacementInfo.Place_Tile);
        GameManager.Placement.PlacementConfirm(this, info_Exit);

        SearchPreviousFloor(); //? �������� �ö󰡴� ���� ������ ���ϱ� �켱����������� �ⱸ�� 1������ ã��. �߰��� State�� ���浵 �ϸ� �ȵ�.
    }

    void FloorEscape() //? ���Ż�� - �ٷ� �������� ��
    {
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� �������� Ż��");
        //Debug.Log($"{name}(��)�� �������� Ż��");
        Arrival();

        switch (State)
        {
            case NPCState.Runaway:
                Main.Instance.FameOfDungeon += 5;
                Main.Instance.DangerOfDungeon += 1;
                break;
            case NPCState.Return:
                Main.Instance.FameOfDungeon += -2;
                Main.Instance.DangerOfDungeon += -2;
                break;
        }
    }

    void FloorPortal(int hiddenFloor) //? Ư�������� �����̵�
    {
        GameManager.Placement.PlacementClear(this);

        //? �Ա����� ��ȯ
        var info_Exit = new PlacementInfo(Main.Instance.Floor[hiddenFloor], Main.Instance.Floor[hiddenFloor].Exit.PlacementInfo.Place_Tile);
        //var info = new PlacementInfo(Main.Instance.Floor[hiddenFloor], Main.Instance.Floor[hiddenFloor].GetRandomTile(this));

        GameManager.Placement.PlacementConfirm(this, info_Exit);


        SetPriorityList(); //? �켱������ ���� ��Ž��

        if (PriorityList.Count > 0)
        {
            State = NPCState.Priority;
        }
        else
        {
            State = NPCState.Return;
        }
        Debug.Log("�ϴ� ��й� �̵��Ϸ�");
    }




    void SearchNextFloor()
    {
        PriorityList.Add(PlacementInfo.Place_Floor.Entrance.PlacementInfo.Place_Tile);
        MoveToTargetTile(PriorityList[0]);
    }
    void SearchPreviousFloor()
    {
        PriorityList.Clear();
        PriorityList.Add(PlacementInfo.Place_Floor.Exit.PlacementInfo.Place_Tile);
        MoveToTargetTile(PriorityList[0]);
    }


    void NPC_Die()
    {
        Main.Instance.CurrentDay.AddKill(1);
        Main.Instance.CurrentDay.AddGold(100);
        Debug.Log($"{name}(��)�� ���� + �ڼ��� ������ ���Ŀ� �߰�");
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� ������");
        GameManager.NPC.InactiveNPC(this);

        Main.Instance.FameOfDungeon += 1;
        Main.Instance.DangerOfDungeon += 5;
    }

    void NPC_Captive()
    {
        Main.Instance.CurrentDay.AddPrisoner(1);
        Debug.Log($"{name}(��)�� ���η� ����");
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� ���η� ����");
        GameManager.NPC.InactiveNPC(this);


    }



    NPCState StateRefresh()
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
            return NPCState.Runaway;
        }

        if (HP < (_HP_Origin / 4))
        {
            return NPCState.Runaway;
        }


        PriorityList.RemoveAll(r => r.tileType == Define.TileType.Empty || r.tileType == Define.TileType.NPC);

        if (PriorityList.Count == 0)
        {
            return NPCState.Next;
        }
        if (PriorityList[0].unchangeable == PlacementInfo.Place_Floor.Entrance.PlacementInfo.Place_Tile.unchangeable)
        {
            return NPCState.Next;
        }
        if (PriorityList[0].unchangeable == PlacementInfo.Place_Floor.Exit.PlacementInfo.Place_Tile.unchangeable)
        {
            return NPCState.Return;
        }

        if (prevState == NPCState.Return)
        {
            return NPCState.Return;
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

            case NPCState.Return:
                SearchPreviousFloor();
                break;


            case NPCState.Runaway:
                SearchPreviousFloor();
                break;

            case NPCState.Die:
                NPC_Die();
                break;
        }
    }





    Coroutine Cor_Encounter;
    Coroutine Cor_Move;


    public void MoveToTargetTile(BasementTile target)
    {
        bool pathFind = false;
        bool pathRefind = false;
        List<BasementTile> path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target, AvoidTileType, out pathFind);

        //Debug.Log($"�켱���� ��ã�� ��� : {pathFind}");

        if (!pathFind) //? ���� ��ã������ ��ֹ����� ã�� �ѹ��� ��ȸ�� �� ��.
        {
            path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target, out pathRefind);
            //Debug.Log($"�Ϲ� ��ã�� ��� : {pathRefind}");
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
            yield return new WaitForSeconds(ActionDelay);

            var encount = path[i].TryPlacement(this, overlap);
            //Debug.Log($"{name} ��ǥ : {PlacementInfo.Place_Floor.Floor} / {PlacementInfo.Place_Tile.index} / ���� : {State} / \n" +
            //    $"����Ÿ��Ÿ�� : {path[i].tileType} /��ǥ : {path[i].floor.Floor} / {path[i].index} / �̺�ƮŸ�� : {encount}\n" +
            //    $"���� ����Ʈ �� :{PriorityList.Count} / 0�� Ÿ�� : {PriorityList[0]} / �Ա� Ÿ�� {PlacementInfo.Place_Floor.Entrance}");
            if (EncountOver(encount, path[i]))
            {
                yield return new WaitForEndOfFrame();
                //Debug.Log("�̰� ������ �ȉ�");
                break;
            }
            else
            {
                var next = new PlacementInfo(PlacementInfo.Place_Floor, path[i]);
                GameManager.Placement.PlacementMove(this, next);
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
            case Define.PlaceEvent.Event:
                GameManager.Placement.PlacementMove(this, next);
                StopCoroutine(Cor_Move);
                Cor_Move = null;
                Cor_Encounter = StartCoroutine(Encounter_NoStateRefresh(tile, () => State = NPCState.Return));
                return true;

            case Define.PlaceEvent.Using:
                GameManager.Placement.PlacementMove(this, next);
                StopCoroutine(Cor_Move);
                Cor_Move = null;
                Cor_Encounter = StartCoroutine(Encounter_Facility(tile));
                State = NPCState.Interaction;
                return true;


            case Define.PlaceEvent.Using_Portal:
                GameManager.Placement.PlacementMove(this, next);
                StopCoroutine(Cor_Move);
                Cor_Move = null;
                Cor_Encounter = StartCoroutine(Encounter_Portal(tile, (floor) => FloorPortal(floor)));
                return true;


            case Define.PlaceEvent.Entrance:
                if (State == NPCState.Next)
                {
                    GameManager.Placement.PlacementMove(this, next);
                    StopCoroutine(Cor_Move);
                    Cor_Move = null;
                    Cor_Encounter = StartCoroutine(Encounter_NoStateRefresh(tile, () => FloorNext()));
                    return true;
                }
                if (State == NPCState.Priority)
                {
                    return false;
                }
                return false;

            case Define.PlaceEvent.Exit:
                if (State == NPCState.Return)
                {
                    GameManager.Placement.PlacementMove(this, next);
                    StopCoroutine(Cor_Move);
                    Cor_Move = null;
                    Cor_Encounter = StartCoroutine(Encounter_NoStateRefresh(tile, () => FloorPrevious()));

                    //FloorPrevious();
                    return true;
                }
                else if (State == NPCState.Runaway)
                {
                    GameManager.Placement.PlacementMove(this, next);
                    StopCoroutine(Cor_Move);
                    Cor_Move = null;
                    Cor_Encounter = StartCoroutine(Encounter_NoStateRefresh(tile, () => FloorEscape()));

                    //FloorEscape();
                    return true;
                }
                if (State == NPCState.Priority)
                {
                    return false;
                }
                return false;


            case Define.PlaceEvent.Avoid:
                State = StateRefresh();
                return true;

            case Define.PlaceEvent.Overlap:
                break;


            case Define.PlaceEvent.Battle:
                StopCoroutine(Cor_Move);
                Cor_Move = null;
                Cor_Encounter = StartCoroutine(Encounter_Monster(tile));
                State = NPCState.Battle;
                return true;

            case Define.PlaceEvent.Interaction:
                StopCoroutine(Cor_Move);
                Cor_Move = null;
                Cor_Encounter = StartCoroutine(Encounter_Facility(tile));
                State = NPCState.Interaction;
                return true;

            case Define.PlaceEvent.Placement:
                break;

            case Define.PlaceEvent.Nothing:
                break;

            default:
                break;
        }

        return false;
    }

    IEnumerator Encounter_Portal(BasementTile tile, Action<int> action)
    {
        var type = tile.unchangeable as Facility;
        int floor;

        if (type)
        {
            yield return type.NPC_Interaction_Portal(this, out floor);
            yield return new WaitForEndOfFrame();
            action.Invoke(floor);
        }
    }

    IEnumerator Encounter_NoStateRefresh(BasementTile tile, Action action)
    {
        var type = tile.unchangeable as Facility;

        if (type)
        {
            yield return type.NPC_Interaction(this);
            yield return new WaitForEndOfFrame();
            action.Invoke();
        }
    }

    IEnumerator Encounter_Facility(BasementTile tile)
    {
        Facility facility;
        if (tile.isUnchangeable)
        {
            facility = tile.unchangeable as Facility;
        }
        else
        {
            facility = tile.placementable as Facility;
        }
        
        if (facility)
        {
            yield return facility.NPC_Interaction(this);
            yield return new WaitForEndOfFrame();
            State = StateRefresh();
        }
    }

    IEnumerator Encounter_Monster(BasementTile tile)
    {
        var monster = tile.placementable as Monster;

        if (monster)
        {
            //Debug.Log("��Ʋ ����");
            yield return monster.Battle(this);
            //Debug.Log("��Ʋ ����");
            yield return new WaitForEndOfFrame();
            State = StateRefresh();
        }
    }



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
            Debug.Log("��ã�� ���� / �����̵�" + Time.time);
            var next = new PlacementInfo(PlacementInfo.Place_Floor, newTile);
            GameManager.Placement.PlacementMove(this, next);
        }
        yield return new WaitForEndOfFrame();
        Cor_Move = null;
        Debug.Log("��ã�� ���� / ���»��ΰ�ħ" + Time.time);
        //? �̰� ap����Ʈ �� ���̸� �ⱸ���� ������ �̵��������ϴ� ��Ȳ ����. �ٸ� ī��Ʈ�� ��Ȳ�� �����̻��ϸ� �� �޼���� �ؾ��ҵ�
        //ActionPoint--;
        //Debug.Log(ActionPoint);
        State = StateRefresh();
    }

}
