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
    }
    void Update()
    {

    }


    #region Npc Status Property

    public string Name { get; set; }
    public int LV { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }



    public int HP { get; set; }
    public int ActionPoint { get; set; }
    public int Mana { get; set; }


    #endregion







    enum NPCState
    {
        Standby,

        Interaction,
        Battle,
        Die,
        Runaway,
        Return,
    }

    NPCState State;

    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }
    public Define.PlacementType PlacementType { get; set; }

    SpriteRenderer npcRenderer;

    int floorIndex;


    protected abstract void SetPriorityList();




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
            transform.Translate(dir.normalized * Time.deltaTime * 1);
            dis = Vector3.Distance(transform.position, point);
            yield return null;
        }

        Debug.Log("입구도착");

        Placement(Main.Instance.Floor[floorIndex]);
        SetPriorityList();
    }
    #endregion Ground






    #region InDungeon

    public void Placement(BasementFloor place)
    {
        //Debug.Log($"{name} 가 {place} 에 배치됨.");
        PlacementConfirm(place, place.GetRandomTile(this));
    }

    private void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        State = NPCState.Standby;

        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
        Visible();
    }

    public void PlacementClear()
    {
        //Debug.Log($"{name} 가 쓰러짐.");
        State = NPCState.Die;

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




    Coroutine Cor_Encounter;
    Coroutine Cor_Move;


    public void NextFloor()
    {
        floorIndex++;
        if (floorIndex < Main.Instance.Floor.Length)
        {
            Placement(Main.Instance.Floor[floorIndex]);
        }
        else
        {
            State = NPCState.Return;
        }

        WaitBehaviourFinished = StartCoroutine(EndBehaviour(() => true, 1));
    }

    public void Interaction(Facility facility)
    {

        WaitBehaviourFinished = StartCoroutine(EndBehaviour(() => true, 1));
    }

    public void Battle(Monster monster)
    {

        WaitBehaviourFinished = StartCoroutine(EndBehaviour(() => true, 3));
    }


    Coroutine WaitBehaviourFinished;
    IEnumerator EndBehaviour(Func<bool> func, int AP)
    {
        yield return new WaitUntil(func);

        ActionPoint -= AP;
        if (ActionPoint <= 0)
        {
            State = NPCState.Return;
        }
    }

    public abstract List<BasementTile> PriorityList { get; set; }

    protected List<BasementTile> GetFloorObjectsAll(Define.TileType searchType = Define.TileType.Empty)
    {
        var newList = Place_Floor.SearchAllObjects(searchType);
        var refresh = AddDistinctList(newList);

        if (newList.Count == 0)
        {
            Debug.Log("탐색결과 없음");
            return null;
        }

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








    public void MoveToTargetTile(BasementTile target)
    {
        List<BasementTile> path = Place_Floor.PathFinding(Place_Tile, target);

        Cor_Move = StartCoroutine(DungeonMoveToPath(path));
    }


    IEnumerator DungeonMoveToPath(List<BasementTile> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            yield return new WaitForSeconds(1);

            var encount = path[i].TryPlacement(this);

            if (encount == Define.PlaceEvent.Interaction)
            {
                Cor_Encounter = StartCoroutine(Encounter_Facility(path[i]));
                Cor_Move = null;
                break;
            }
            else if (encount == Define.PlaceEvent.Battle)
            {
                Cor_Encounter = StartCoroutine(Encounter_Monster(path[i]));
                Cor_Move = null;
                break;
            }
            else
            {
                Place_Tile.ClearPlacement();
                PlacementConfirm(Place_Floor, path[i]);
            }
        }
    }

    IEnumerator Encounter_Facility(BasementTile tile)
    {
        var type = tile.placementable as Facility;

        if (type)
        {
            yield return type.NPC_Interaction(this);
            if (RefreshList())
            {
                MoveToTargetTile(PriorityList[0]);
            }
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
        }
    }



    public void Move()
    {

    }

    public void Die()
    {
        StopCoroutine(WaitBehaviourFinished);

        State = NPCState.Die;
    }

    public void Runaway()
    {

    }



    #endregion

}
