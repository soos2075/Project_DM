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
    }
    //void Update()
    //{

    //}

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


    protected List<BasementTile> GetFloorObjectsAll(Define.TileType searchType = Define.TileType.Empty)
    {
        var newList = PlacementInfo.Place_Floor.GetFloorObjectList(searchType);

        if (PriorityList != null)
        {
            newList.AddRange(PriorityList);
        }

        return RefinementList(newList);
    }

    protected List<BasementTile> GetPriorityPick(Type type, bool includeOrigin = false)
    {
        if (PriorityList == null)
        {
            PriorityList = GetFloorObjectsAll();
        }

        List<BasementTile> oldList = PriorityList;
        List<BasementTile> newList = new List<BasementTile>();

        for (int i = 0; i < oldList.Count; i++)
        {
            if (oldList[i].placementable.GetType() == type)
            {
                newList.Add(oldList[i]);
            }
        }

        newList = Util.ListShuffle(newList);

        if (includeOrigin)
        {
            newList.AddRange(PriorityList);
        }

        return RefinementList(newList);
    }


    List<BasementTile> RefinementList(List<BasementTile> _list) //? ���Ա��� �ڱ��ڽ�, �ߺ������� ��� �����ϴ� �۾�
    {
        var newList = _list.Distinct().ToList();

        newList.Remove(PlacementInfo.Place_Tile);
        newList.Remove(PlacementInfo.Place_Floor.Entrance.PlacementInfo.Place_Tile);
        newList.Remove(PlacementInfo.Place_Floor.Exit.PlacementInfo.Place_Tile);

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
        Managers.Placement.Visible(this);

        StartCoroutine(MoveToPoint(endPoint));
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
        transform.position = Main.Instance.dungeonEntrance.position;
        Managers.Placement.Visible(this);
        StartCoroutine(MoveToHome(Main.Instance.guild.position));
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
        Main.Instance.InactiveNPC(this);
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

        Interaction_Trap, 

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
        if (FloorLevel == Main.Instance.Floor.Length)
        {
            Debug.Log($"{name}(��)�� {FloorLevel}������ ���̻� �ö� �� ����");
            State = NPCState.Return;
            return;
        }





        Managers.Placement.PlacementClear(this);

        //? ������ġ�� ��ȯ
        //var info = Managers.Placement.GetRandomPlacement(this, Main.Instance.Floor[FloorLevel]); 
        //? �Ա����� ��ȯ
        var info_Entrance = new PlacementInfo(Main.Instance.Floor[FloorLevel], Main.Instance.Floor[FloorLevel].Exit.PlacementInfo.Place_Tile);
        Managers.Placement.PlacementConfirm(this, info_Entrance);

        FloorLevel++;
        //Debug.Log($"{name}(��)�� {FloorLevel}���� ����");

        SetPriorityList(); //? �켱������ ���� ��Ž��

        if (PriorityList.Count > 0)
        {
            State = NPCState.Priority;
        }
        else
        {
            State = NPCState.Next;
        }
        
    }
    void FloorPrevious() //? ���������� �ö󰡴� �Ա��� �������� �� ȣ��
    {
        FloorLevel--;
        if (FloorLevel == 0)
        {
            FloorEscape();
            return;
        }
        //Debug.Log($"{name}(��)�� {FloorLevel}���� ����");

        Managers.Placement.PlacementClear(this);

        var info_Exit = new PlacementInfo(Main.Instance.Floor[FloorLevel - 1], Main.Instance.Floor[FloorLevel - 1].Entrance.PlacementInfo.Place_Tile);
        Managers.Placement.PlacementConfirm(this, info_Exit);

        SearchPreviousFloor(); //? �������� �ö󰡴� ���� ������ ���ϱ� �켱����������� �ⱸ�� 1������ ã��. �߰��� State�� ���浵 �ϸ� �ȵ�.
    }

    void FloorEscape() //? ���Ż�� - �ٷ� �������� ��
    {
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� �������� Ż��");
        //Debug.Log($"{name}(��)�� �������� Ż��");
        Arrival();
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
        Debug.Log($"{name}(��)�� ����");
    }

    void NPC_Captive()
    {
        Main.Instance.CurrentDay.AddPrisoner(1);
        Debug.Log($"{name}(��)�� ���η� ����");
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


            case NPCState.Interaction:
                break;

            case NPCState.Interaction_Trap:
                break;

            case NPCState.Battle:
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
            //Debug.Log("�ڷ�ƾ ����");
            Cor_Move = StartCoroutine(DungeonMoveToPath(path, pathRefind));
        }
        else
        {
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
                Managers.Placement.PlacementMove(this, next);
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
            case Define.PlaceEvent.Trap:
                Managers.Placement.PlacementMove(this, next);
                StopCoroutine(Cor_Move);
                Cor_Move = null;
                Cor_Encounter = StartCoroutine(Encounter_Facility(tile));
                State = NPCState.Interaction_Trap;
                return true;

            case Define.PlaceEvent.Entrance:
                if (State == NPCState.Next)
                {
                    Managers.Placement.PlacementMove(this, next);
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
                    Managers.Placement.PlacementMove(this, next);
                    StopCoroutine(Cor_Move);
                    Cor_Move = null;
                    Cor_Encounter = StartCoroutine(Encounter_NoStateRefresh(tile, () => FloorPrevious()));

                    //FloorPrevious();
                    return true;
                }
                else if (State == NPCState.Runaway)
                {
                    Managers.Placement.PlacementMove(this, next);
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
        var type = tile.unchangeable as Facility;

        if (type)
        {
            yield return type.NPC_Interaction(this);
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
            var next = new PlacementInfo(PlacementInfo.Place_Floor, newTile);
            Managers.Placement.PlacementMove(this, next);
        }
        yield return new WaitForEndOfFrame();
        Cor_Move = null;
        ActionPoint--;
        //Debug.Log(ActionPoint);
        State = StateRefresh();
    }

}
