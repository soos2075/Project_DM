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


    List<BasementTile> RefinementList(List<BasementTile> _list) //? 출입구와 자기자신, 중복참조를 모두 제거하는 작업
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

        UI_EventBox.AddEventText($"◆{Name_KR} (이)가 던전에 입장");
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



    public float Speed_Ground { get; set; } //? 클수록 빠름
    public float ActionDelay { get; set; } //? 작을수록 빠름


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

    void FloorNext() //? 지하층으로 내려가는 입구에 도착했을 때 호출
    {
        if (FloorLevel == Main.Instance.Floor.Length)
        {
            Debug.Log($"{name}(이)가 {FloorLevel}층에서 더이상 올라갈 수 없음");
            State = NPCState.Return;
            return;
        }





        Managers.Placement.PlacementClear(this);

        //? 랜덤위치로 소환
        //var info = Managers.Placement.GetRandomPlacement(this, Main.Instance.Floor[FloorLevel]); 
        //? 입구에서 소환
        var info_Entrance = new PlacementInfo(Main.Instance.Floor[FloorLevel], Main.Instance.Floor[FloorLevel].Exit.PlacementInfo.Place_Tile);
        Managers.Placement.PlacementConfirm(this, info_Entrance);

        FloorLevel++;
        //Debug.Log($"{name}(이)가 {FloorLevel}층에 도착");

        SetPriorityList(); //? 우선순위에 맞춰 맵탐색

        if (PriorityList.Count > 0)
        {
            State = NPCState.Priority;
        }
        else
        {
            State = NPCState.Next;
        }
        
    }
    void FloorPrevious() //? 지상층으로 올라가는 입구에 도착했을 때 호출
    {
        FloorLevel--;
        if (FloorLevel == 0)
        {
            FloorEscape();
            return;
        }
        //Debug.Log($"{name}(이)가 {FloorLevel}층에 도착");

        Managers.Placement.PlacementClear(this);

        var info_Exit = new PlacementInfo(Main.Instance.Floor[FloorLevel - 1], Main.Instance.Floor[FloorLevel - 1].Entrance.PlacementInfo.Place_Tile);
        Managers.Placement.PlacementConfirm(this, info_Exit);

        SearchPreviousFloor(); //? 지상으로 올라가는 경우는 나가는 경우니까 우선순위상관없이 출구만 1순위로 찾음. 추가로 State의 변경도 하면 안됨.
    }

    void FloorEscape() //? 긴급탈출 - 바로 지상으로 감
    {
        UI_EventBox.AddEventText($"◆{Name_KR} (이)가 지상으로 탈출");
        //Debug.Log($"{name}(이)가 지상으로 탈출");
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
        Debug.Log($"{name}(이)가 죽음");
    }

    void NPC_Captive()
    {
        Main.Instance.CurrentDay.AddPrisoner(1);
        Debug.Log($"{name}(이)가 포로로 잡힘");
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

        //Debug.Log($"우선순위 길찾기 결과 : {pathFind}");

        if (!pathFind) //? 길을 못찾았으면 장애물없이 찾을 한번의 기회는 더 줌.
        {
            path = PlacementInfo.Place_Floor.PathFinding(PlacementInfo.Place_Tile, target, out pathRefind);
            //Debug.Log($"일반 길찾기 결과 : {pathRefind}");
        }

        if (Cor_Move != null)
        {
            //Debug.Log("중복코루틴 멈춤");
            StopCoroutine(Cor_Move);
        }

        if (pathFind || pathRefind)
        {
            //Debug.Log("코루틴 시작");
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
            //Debug.Log($"{name} 좌표 : {PlacementInfo.Place_Floor.Floor} / {PlacementInfo.Place_Tile.index} / 상태 : {State} / \n" +
            //    $"다음타일타입 : {path[i].tileType} /좌표 : {path[i].floor.Floor} / {path[i].index} / 이벤트타입 : {encount}\n" +
            //    $"현재 리스트 수 :{PriorityList.Count} / 0번 타일 : {PriorityList[0]} / 입구 타일 {PlacementInfo.Place_Floor.Entrance}");
            if (EncountOver(encount, path[i]))
            {
                yield return new WaitForEndOfFrame();
                //Debug.Log("이거 읽으면 안됌");
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
        State = StateRefresh(); //? 모든 경로를 탐색했는데 이벤트가 발생을 안했다? 뭔가 이상이 있는것. 상태업데이트 필요
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
            //Debug.Log("배틀 시작");
            yield return monster.Battle(this);
            //Debug.Log("배틀 종료");
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
