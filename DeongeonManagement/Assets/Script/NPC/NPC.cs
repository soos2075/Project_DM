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
    #endregion




    #region PriorityList
    public abstract List<BasementTile> PriorityList { get; set; }
    protected abstract void SetPriorityList();


    protected List<BasementTile> GetFloorObjectsAll(Define.TileType searchType = Define.TileType.Empty)
    {
        var newList = PlacementInfo.Place_Floor.GetFloorObjectList(searchType);
        var refresh = AddDistinctList(newList);
        return refresh;
    }

    protected List<BasementTile> GetPriorityPick(Type type, bool includeList = false)
    {
        if (PriorityList == null)
        {
            PriorityList = PlacementInfo.Place_Floor.GetFloorObjectList();
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

        return includeList
            ? AddDistinctList(newList)
            : newList;
    }

    protected void PriorityRemove(BasementTile item)
    {
        if (PriorityList == null) return;

        PriorityList.Remove(item);
    }

    List<BasementTile> AddDistinctList(List<BasementTile> addList)
    {
        var newList = addList;
        if (PriorityList != null)
        {
            newList.AddRange(PriorityList);
        }
        PriorityRemove(PlacementInfo.Place_Tile);
        return newList.Distinct().ToList();
    }

    #endregion






    #region Ground

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

        FloorNext();
    }

    public void Arrival()
    {
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

    public string Name { get; set; }
    public int LV { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }



    public int HP { get; set; }
    private int _HP_Origin;
    public int ActionPoint { get; set; }
    public int Mana { get; set; }



    public float Speed_Ground { get; set; } //? Ŭ���� ����
    public float ActionDelay { get; set; } //? �������� ����


    protected abstract void Initialize_Status();

    protected void SetStatus(string name, int lv, int atk, int def, int hp, int ap, int mp, float speed, float delay)
    {
        Name = name; LV = lv;  ATK = atk; DEF = def;
        HP = hp; ActionPoint = ap; Mana = mp;

        Speed_Ground = speed; ActionDelay = delay;

        _HP_Origin = hp;
    }

    #endregion



    enum NPCState
    {
        Next,

        Priority,

        Runaway,
        Return,

        Die,

        Interaction,
        Battle,
    }

    NPCState _state;
    NPCState State
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
        Debug.Log($"{name}(��)�� {FloorLevel}���� ����");

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
        Debug.Log($"{name}(��)�� �������� Ż��");
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
        Debug.Log($"{name}(��)�� ����");
    }

    void NPC_Captive()
    {
        Debug.Log($"{name}(��)�� ���η� ����");
    }



    NPCState StateRefresh()
    {
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

        PriorityList.RemoveAll(r => r.tileType == Define.TileType.Empty);

        if (PriorityList.Count == 0)
        {
            return NPCState.Next;
        }

        if (PriorityList[0].tileType == Define.TileType.Entrance || PriorityList[0].tileType == Define.TileType.Exit)
        {
            return NPCState.Next;
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

            case NPCState.Battle:
                break;
        }
    }





    Coroutine Cor_Encounter;
    Coroutine Cor_Move;


    public void MoveToTargetTile(BasementTile target)
    {
        List<BasementTile> path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target);

        if (Cor_Move != null)
        {
            Debug.Log("�ߺ��ڷ�ƾ �־���");
            StopCoroutine(Cor_Move);
        }
        Cor_Move = StartCoroutine(DungeonMoveToPath(path));
    }


    IEnumerator DungeonMoveToPath(List<BasementTile> path)
    {
        for (int i = 1; i <= path.Count; i++)
        {
            yield return new WaitForSeconds(ActionDelay);

            var encount = path[i].TryPlacement(this);

            if (encount == Define.PlaceEvent.Entrance)
            {
                if (State == NPCState.Next)
                {
                    FloorNext();
                    Cor_Move = null;
                    break;
                }
            }
            else if (encount == Define.PlaceEvent.Exit)
            {
                if (State == NPCState.Return)
                {
                    FloorPrevious();
                    Cor_Move = null;
                    break;
                }
                else if (State == NPCState.Runaway)
                {
                    FloorEscape();
                    Cor_Move = null;
                    break;
                }
            }
            else if (encount == Define.PlaceEvent.Interaction)
            {
                Cor_Encounter = StartCoroutine(Encounter_Facility(path[i]));
                Cor_Move = null;
                State = NPCState.Interaction;
                break;
            }
            else if (encount == Define.PlaceEvent.Battle)
            {
                Cor_Encounter = StartCoroutine(Encounter_Monster(path[i]));
                Cor_Move = null;
                State = NPCState.Battle;
                break;
            }
            var next = new PlacementInfo(PlacementInfo.Place_Floor, path[i]);
            Managers.Placement.PlacementMove(this, next);
        }
    }

    IEnumerator Encounter_Facility(BasementTile tile)
    {
        var type = tile.placementable as Facility;

        if (type)
        {
            yield return type.NPC_Interaction(this);
            State = StateRefresh();
        }
    }

    IEnumerator Encounter_Monster(BasementTile tile)
    {
        var type = tile.placementable as Monster;

        if (type)
        {
            Debug.Log("��Ʋ ����");
            yield return type.Battle(this);
            Debug.Log("��Ʋ ����");
            State = StateRefresh();
        }
    }


}
