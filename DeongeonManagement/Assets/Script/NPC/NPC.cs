using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour, Interface.IPlacementable
{
    void Start()
    {
        PlacementType = Define.PlacementType.NPC;
        npcSprite = GetComponentInChildren<SpriteRenderer>();
        npcSprite.enabled = false;
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

    SpriteRenderer npcSprite;

    int floorIndex;


    protected abstract void BehaviourPriority();


    protected void SetPriority()
    {
        //? ���⼭ �켱���� WishList�� �ۼ��ϰ� �ű⿡ ���� �����̸� �ɵ�.
        //? �� �ϳ��� ��ǥ�� �ʿ�� ����. ����Ʈ�ϱ� �켱������ ���� 1. ���� 2. ���� 3. ���� �̷������� �ϰ� ���������ϴ½�����.
        //? ���� �߰��� �ٸ� ��ǥ���� ������ ó���������. ���⼱ List�� ������ �ۼ��ϴ°��� �ƴϰ� �켱������ ���ϴ°ɷ�
        //? ���ʿ� ����Ʈ�� �÷ξ� ���� ���������� ������ �ȶ�
    }






    #region Ground

    public void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = startPoint;
        npcSprite.enabled = true;

        StartCoroutine(MoveToPoint(endPoint));
    }

    IEnumerator MoveToPoint(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        float dis = Vector3.Distance(transform.position, point);

        while(dis > 0.1f)
        {
            transform.Translate(dir.normalized * Time.deltaTime * 1);
            dis = Vector3.Distance(transform.position, point);
            yield return null;
        }

        Debug.Log("�Ա�����");

        Placement(Main.Instance.Floor[floorIndex]);
        SearchTarget();
    }
    #endregion Ground




    #region InDungeon

    private void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        State = NPCState.Standby;

        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
        npcSprite.enabled = true;
    }


    public void Placement(BasementFloor place)
    {
        //Debug.Log($"{name} �� {place} �� ��ġ��.");
        PlacementConfirm(place, place.GetRandomTile(this));
    }


    public void PlacementClear()
    {
        //Debug.Log($"{name} �� ������.");
        State = NPCState.Die;

        Place_Tile.ClearPlacement();
        Place_Floor = null;
        Place_Tile = null;
        npcSprite.enabled = false;
    }





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


    List<BasementTile> WishList;
    int wishObjectCount;
    List<BasementTile> path;

    public void SearchTarget()
    {
        WishList = Place_Floor.SearchAllObjects(Define.TileType.Facility);

        if (WishList.Count == 0)
        {
            Debug.Log("�����Ѱ��� ����");
            return;
        }

        path = Place_Floor.PathFinding(Place_Tile, WishList[wishObjectCount]);
        wishObjectCount++;

        StartCoroutine(DungeonMoveToTarget());
    }


    IEnumerator DungeonMoveToTarget()
    {
        for (int i = 0; i < path.Count; i++)
        {
            yield return new WaitForSeconds(1);
            Place_Tile.ClearPlacement();
            PlacementConfirm(Place_Floor, path[i]);
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
