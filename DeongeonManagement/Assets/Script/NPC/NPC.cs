using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class NPC : MonoBehaviour, Interface.IPlacementable
{
    void Start()
    {
        PlacementType = Define.PlacementType.NPC;
        npcRenderer = GetComponentInChildren<SpriteRenderer>();
        Disable();

        Initialize_Status();
    }
    void Update()
    {

    }

    SpriteRenderer npcRenderer;

    #region Placement

    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }
    public Define.PlacementType PlacementType { get; set; }


    public void Placement(BasementFloor place)
    {
        //Debug.Log($"{name} 가 {place} 에 배치됨.");
        PlacementConfirm(place, place.GetRandomTile(this));
    }

    private void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
        Visible();
    }

    public void PlacementClear()
    {
        Place_Tile.ClearPlacement();
        Place_Floor = null;
        Place_Tile = null;
        Disable();
    }

    protected void Visible()
    {
        npcRenderer.enabled = true;
    }

    protected void Disable()
    {
        npcRenderer.enabled = false;
    }

    #endregion



    #region PriorityList
    public abstract List<BasementTile> PriorityList { get; set; }
    protected abstract void SetPriorityList();


    protected List<BasementTile> GetFloorObjectsAll(Define.TileType searchType = Define.TileType.Empty)
    {
        var newList = Place_Floor.SearchAllObjects(searchType);
        var refresh = AddDistinctList(newList);
        return refresh;
    }

    protected List<BasementTile> GetPriorityPick(Type type, bool includeList = false)
    {
        if (PriorityList == null)
        {
            PriorityList = Place_Floor.SearchAllObjects();
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
        PriorityRemove(Place_Tile);
        return newList.Distinct().ToList();
    }

    bool RefreshList()
    {
        if (PriorityList == null) return false;

        for (int i = PriorityList.Count - 1; i >= 0; i--)
        {
            if (PriorityList[i].tileType == Define.TileType.Empty)
            {
                PriorityList.RemoveAt(i);
            }
        }

        if (PriorityList.Count == 0) return false;
        return true;
    }


    #endregion






    #region Ground

    public void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = startPoint;
        Visible();

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
        Visible();
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



    public float Speed_Ground { get; set; } //? 클수록 빠름
    public float ActionDelay { get; set; } //? 작을수록 빠름


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
        Standby,
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

    void FloorNext() //? 지하층으로 내려가는 입구에 도착했을 때 호출
    {
        if (FloorLevel == Main.Instance.Floor.Length)
        {
            Debug.Log($"{name}(이)가 {FloorLevel}층에서 더이상 올라갈 수 없음");
            State = NPCState.Return;
            return;
        }

        Placement(Main.Instance.Floor[FloorLevel]);
        FloorLevel++;
        Debug.Log($"{name}(이)가 {FloorLevel}층에 도착");

        SetPriorityList(); //? 우선순위에 맞춰 맵탐색

        State = NPCState.Standby;
    }
    void FloorPrevious() //? 지상층으로 올라가는 입구에 도착했을 때 호출
    {
        FloorLevel--;
        if (FloorLevel == 0)
        {
            Debug.Log($"{name}(이)가 던전 출구에 도착함");
            FloorEscape();
            return;
        }

        Debug.Log($"{name}(이)가 {FloorLevel}층에 도착");
        Placement(Main.Instance.Floor[FloorLevel - 1]);

        SearchPreviousFloor(); //? 지상으로 올라가는 경우는 나가는 경우니까 우선순위상관없이 출구만 1순위로 찾음.
        //State = NPCState.Standby;
    }

    void FloorEscape() //? 긴급탈출 - 바로 지상으로 감
    {
        Debug.Log($"{name}(이)가 지상으로 탈출");
        PlacementClear();
        Arrival();
    }



    void SearchNextFloor()
    {
        PriorityList.Add(Place_Floor.entrance.Place_Tile);
        MoveToTargetTile(PriorityList[0]);
    }
    void SearchPreviousFloor()
    {
        PriorityList.Clear();
        PriorityList.Add(Place_Floor.exit.Place_Tile);
        MoveToTargetTile(PriorityList[0]);
    }


    void NPC_Die()
    {
        Debug.Log($"{name}(이)가 죽음");
    }

    void NPC_Captive()
    {
        Debug.Log($"{name}(이)가 포로로 잡힘");
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


        RefreshList();
        return NPCState.Standby;
    }

    void BehaviourByState()
    {
        //? 출구를 최우선으로 찾음 (PriorityUpdate)
        //? 출구를 향해 움직임. 가는길에도 이벤트 발생 가능 / 다만 전투이벤트만 가능.
        //? 만약 오브젝트에 길막당해서 못간다? 라는 상황은 없을거임. 애초에 들어올 수 있으면 나갈수도 있는거
        //? 근데 미로같은거라서 들어오면 뒤로는 못가는 함정같은경우가 있을 수 있음. 이런경우는 패닉상태에 빠져서 HP를 줄이던가 포로로잡히던가 둘중하나.

        switch (State)
        {
            case NPCState.Standby:
                if (PriorityList.Count > 0)
                {
                    MoveToTargetTile(PriorityList[0]);
                }
                else
                {
                    Debug.Log("탐색결과 없음");
                    SearchNextFloor();
                }
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
        List<BasementTile> path = Place_Floor.PathFinding(Place_Tile, target);

        if (Cor_Move != null)
        {
            Debug.Log("중복코루틴 있었음");
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
                if (PriorityList.Count == 1 && State != NPCState.Return)
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


            Place_Tile.ClearPlacement();
            PlacementConfirm(Place_Floor, path[i]);
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
            Debug.Log("배틀 시작");
            yield return type.Battle(this);
            Debug.Log("배틀 종료");
            State = StateRefresh();
        }
    }


}
