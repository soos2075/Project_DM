using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BasementFloor : MonoBehaviour
{
    void Start()
    {
        
    }
    public void Init_Floor()
    {
        Floor = gameObject.name;

        npcList = new List<NPC>();
        monsterList = new List<Monster>();
        facilityList = new List<Facility>();


        Init_TileMap();
        //Init_Entrance();
    }


    public UI_Floor UI_Floor { get; set; }

    public string Floor { get; set; }
    public int FloorIndex { get; set; }

    public bool Hidden = false;

    public string Name_KR;
    public int MaxMonsterSize = 3;

    public List<NPC> npcList;
    public List<Monster> monsterList;
    public List<Facility> facilityList;


    //public BasementTile[,] TileMap { get; set; }

    public Dictionary<Vector2Int, BasementTile> TileMap { get; set; }



    #region TileMapDic
    Tilemap tilemap;

    public int X_Size { get; set; }
    public int Y_Size { get; set; }

    void Init_TileMap()
    {
        var tileData = GetComponentInChildren<TileMapData>();
        TileMap = tileData.GetDictionary(this);
        tilemap = tileData.FloorTileMap;

        X_Size = tilemap.cellBounds.size.x;
        Y_Size = tilemap.cellBounds.size.y;
        //Debug.Log(tilemap.cellBounds.size.x + "//" + tilemap.cellBounds.size.y);

        //BasementTile start = null;
        //BasementTile end = null;
        //TileMap_Dic.TryGetValue(new Vector2Int(1, 1), out start);
        //TileMap_Dic.TryGetValue(new Vector2Int(1, 8), out end);


        //var obj = GameManager.Placement.CreatePlacementObject("Facility/Herb_Low", new PlacementInfo(this, start), Define.PlacementType.Facility);
        //GameManager.Placement.PlacementConfirm(obj, new PlacementInfo(this, start));
        //var obj2 = GameManager.Placement.CreatePlacementObject("Facility/Herb_Low", new PlacementInfo(this, end), Define.PlacementType.Facility);
        //GameManager.Placement.PlacementConfirm(obj2, new PlacementInfo(this, end));

        //bool isFind = false;
        //var path = PathFinding(start, end, out isFind);

        //foreach (var item in path)
        //{
        //    var obj3 = GameManager.Placement.CreatePlacementObject("Facility/Herb_High", new PlacementInfo(this, item), Define.PlacementType.Facility);
        //    GameManager.Placement.PlacementConfirm(obj3, new PlacementInfo(this, item));
        //}
    }

    public BasementTile GetRandomTile()
    {
        int whileCount = 0; //? 무한루프 방지용
        Vector2Int randomTile;

        BasementTile emptyTile = null;
        BasementTile tempTile = null;
        while (emptyTile == null && whileCount < 50)
        {
            whileCount++;
            randomTile = new Vector2Int(UnityEngine.Random.Range(0, tilemap.cellBounds.size.x), UnityEngine.Random.Range(0, tilemap.cellBounds.size.y));
            if (TileMap.TryGetValue(randomTile, out tempTile))
            {
                if (tempTile.placementable == null)
                {
                    emptyTile = tempTile;
                }
            }
        }
        if (emptyTile == null)
        {
            emptyTile = tempTile;
        }

        return emptyTile;
    }


    public List<BasementTile> PathFinding(BasementTile startPoint, BasementTile targetPoint, Define.TileType[] avoidType, out bool isFind)
    {
        //? 순서는 위 아래 왼쪽 오른쪽 순서
        int[] deltaX = new int[4] { 0, 0, -1, 1 };
        int[] deltaY = new int[4] { 1, -1, 0, 0 };

        bool[,] closed = new bool[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];

        PriorityQueue<PQNode> priorityQueue = new PriorityQueue<PQNode>();
        Vector2Int[,] pathTile = new Vector2Int[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];


        priorityQueue.Push(new PQNode()
        {
            F = Vector3.Distance(startPoint.worldPosition, targetPoint.worldPosition),
            posX = startPoint.index.x,
            posY = startPoint.index.y
        });

        pathTile[startPoint.index.x, startPoint.index.y] = new Vector2Int(startPoint.index.x, startPoint.index.y);
        pathTile[targetPoint.index.x, targetPoint.index.y] = new Vector2Int(targetPoint.index.x, targetPoint.index.y);

        while (priorityQueue.Count > 0)
        {
            PQNode node = priorityQueue.Pop();

            //? 이미 방문한곳이면 스킵
            if (closed[node.posX, node.posY])
                continue;

            closed[node.posX, node.posY] = true;

            //? 도착이면 스킵
            if (node.posX == targetPoint.index.x && node.posY == targetPoint.index.y)
                break;


            for (int i = 0; i < deltaX.Length; i++)
            {
                int nextX = node.posX + deltaX[i];
                int nextY = node.posY + deltaY[i];

                if (nextX == tilemap.cellBounds.size.x || nextX < 0 || nextY == tilemap.cellBounds.size.y || nextY < 0)
                {
                    continue;
                }
                if (closed[nextX, nextY])
                    continue;

                BasementTile value = null;
                if (TileMap.TryGetValue(new Vector2Int(nextX, nextY), out value) == false)
                {
                    continue;
                }


                if (value.tileType == Define.TileType.Using)
                {
                    continue;
                }

                //? 추가한 회피 조건
                bool avoid = false;
                foreach (var type in avoidType)
                {
                    if (value.tileType == type)
                    {
                        avoid = true;
                        break;
                    }
                }

                if (avoid)
                {
                    continue;
                }


                priorityQueue.Push(new PQNode()
                {
                    F = Vector3.Distance(value.worldPosition, targetPoint.worldPosition),
                    posX = nextX,
                    posY = nextY
                });
                pathTile[nextX, nextY] = new Vector2Int(node.posX, node.posY);
            }
        }
        return (MoveList(pathTile, targetPoint, out isFind));
    }
    public List<BasementTile> PathFinding(BasementTile startPoint, BasementTile targetPoint, out bool isFind)
    {
        //? 순서는 위 아래 왼쪽 오른쪽 순서
        int[] deltaX = new int[4] { 0, 0, -1, 1 };
        int[] deltaY = new int[4] { 1, -1, 0, 0 };

        bool[,] closed = new bool[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];

        PriorityQueue<PQNode> priorityQueue = new PriorityQueue<PQNode>();
        Vector2Int[,] pathTile = new Vector2Int[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];


        priorityQueue.Push(new PQNode()
        {
            F = Vector3.Distance(startPoint.worldPosition, targetPoint.worldPosition),
            posX = startPoint.index.x,
            posY = startPoint.index.y
        });

        pathTile[startPoint.index.x, startPoint.index.y] = new Vector2Int(startPoint.index.x, startPoint.index.y);
        pathTile[targetPoint.index.x, targetPoint.index.y] = new Vector2Int(targetPoint.index.x, targetPoint.index.y);

        while (priorityQueue.Count > 0)
        {
            PQNode node = priorityQueue.Pop();

            //? 이미 방문한곳이면 스킵
            if (closed[node.posX, node.posY])
                continue;

            closed[node.posX, node.posY] = true;

            //? 도착이면 스킵
            if (node.posX == targetPoint.index.x && node.posY == targetPoint.index.y)
                break;


            for (int i = 0; i < deltaX.Length; i++)
            {
                int nextX = node.posX + deltaX[i];
                int nextY = node.posY + deltaY[i];

                if (nextX == tilemap.cellBounds.size.x || nextX < 0 || nextY == tilemap.cellBounds.size.y || nextY < 0)
                {
                    continue;
                }
                if (closed[nextX, nextY])
                    continue;

                BasementTile value = null;
                if (TileMap.TryGetValue(new Vector2Int(nextX,nextY), out value) == false)
                {
                    continue;
                }


                if (value.tileType == Define.TileType.Using)
                {
                    continue;
                }


                priorityQueue.Push(new PQNode()
                {
                    F = Vector3.Distance(value.worldPosition, targetPoint.worldPosition),
                    posX = nextX,
                    posY = nextY
                });
                pathTile[nextX, nextY] = new Vector2Int(node.posX, node.posY);
            }
        }
        return (MoveList(pathTile, targetPoint, out isFind));
    }
    List<BasementTile> MoveList(Vector2Int[,] path, BasementTile destination, out bool isFind)
    {
        List<BasementTile> moveList = new List<BasementTile>();
        moveList.Add(destination);

        int x = destination.index.x;
        int y = destination.index.y;

        while (path[x, y].x != x || path[x, y].y != y)  //? 여기서 path[x,y]는 목적지를 거쳐온 경로임. 목적지에 도달할 경로가 없으면 스킵되는거
        {
            BasementTile temp = null;
            TileMap.TryGetValue(new Vector2Int(path[x, y].x, path[x, y].y), out temp);
            moveList.Add(temp);
            Vector2Int pos = path[x, y];
            x = pos.x;
            y = pos.y;
        }
        moveList.Reverse();      //? 전체 경로를 추가했으면 시작부터 탐색해야하므로 반전시켜준다.

        if (moveList.Count == 1)
        {
            isFind = false;
            return moveList;
        }

        isFind = true;
        return moveList;
    }




    public BasementTile MoveUp(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x, currentTile.index.y + 1), out tile))
        {
            if (tile.TryPlacement(placementable) != Define.PlaceEvent.Placement)
                return null;
            else
                return tile;
        }
        return null;
    }
    public BasementTile MoveDown(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x, currentTile.index.y - 1), out tile))
        {
            if (tile.TryPlacement(placementable) != Define.PlaceEvent.Placement)
                return null;
            else
                return tile;
        }
        return null;
    }
    public BasementTile MoveLeft(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x - 1, currentTile.index.y), out tile))
        {
            if (tile.TryPlacement(placementable) != Define.PlaceEvent.Placement)
                return null;
            else
                return tile;
        }
        return null;
    }
    public BasementTile MoveRight(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x + 1, currentTile.index.y), out tile))
        {
            if (tile.TryPlacement(placementable) != Define.PlaceEvent.Placement)
                return null;
            else
                return tile;
        }
        return null;
    }

    #endregion




    public IPlacementable Entrance
    {
        get
        {
            var obj = PickObjectOfType(typeof(Entrance));
            if (obj == null)
            {
                var info = new PlacementInfo(this, GetRandomTile());
                obj = GameManager.Facility.CreateFacility_OnlyOne("Entrance", info, true);
            }
            return obj;
        }
    }

    public IPlacementable Exit
    {
        get
        {
            var obj = PickObjectOfType(typeof(Exit));
            if (obj == null)
            {
                var info = new PlacementInfo(this, GetRandomTile());
                obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info, true);
            }
            return obj;
        }
    }

    public void Init_Entrance()
    {
        if (Hidden)
        {
            return;
        }

        {
            var info = new PlacementInfo(this, GetRandomTile());
            GameManager.Facility.CreateFacility_OnlyOne("Exit", info, true);
        }
        {
            var info = new PlacementInfo(this, GetRandomTile());
            GameManager.Facility.CreateFacility_OnlyOne("Entrance", info, true);
        }
    }



    public void AddObject(IPlacementable placementable)
    {
        var npc = placementable as NPC;
        var monster = placementable as Monster;
        var facility = placementable as Facility;


        if (npc) npcList.Add(npc);
        if (monster) monsterList.Add(monster);
        if (facility) facilityList.Add(facility);
    }

    public void RemoveObject(IPlacementable placementable)
    {
        var npc = placementable as NPC;
        var monster = placementable as Monster;
        var facility = placementable as Facility;

        if (npc) npcList.Remove(npc);
        if (monster) monsterList.Remove(monster);
        if (facility) facilityList.Remove(facility);

        //npcList.RemoveAll(a => a == null);
    }





    public IPlacementable PickObjectOfType(Type type)
    {
        foreach (var item in facilityList)
        {
            if (item.GetType() == type)
            {
                return item;
            }
        }
        foreach (var item in monsterList)
        {
            if (item.GetType() == type)
            {
                return item;
            }
        }
        foreach (var item in npcList)
        {
            if (item.GetType() == type)
            {
                return item;
            }
        }


        Debug.Log($"{type}의 클래스가 존재하지 않음");
        return null;
    }


    public List<BasementTile> GetFloorObjectList(Define.TileType getType = Define.TileType.Empty)
    {
        List<BasementTile> objectsList = new List<BasementTile>();

        switch (getType)
        {
            case Define.TileType.Empty:
                foreach (var item in monsterList)
                {
                    objectsList.Add(item.PlacementInfo.Place_Tile);
                }
                foreach (var item in npcList)
                {
                    objectsList.Add(item.PlacementInfo.Place_Tile);
                }
                foreach (var item in facilityList)
                {
                    objectsList.Add(item.PlacementInfo.Place_Tile);
                }
                break;

            case Define.TileType.Monster:
                foreach (var item in monsterList)
                {
                    objectsList.Add(item.PlacementInfo.Place_Tile);
                }
                break;

            case Define.TileType.NPC:
                foreach (var item in npcList)
                {
                    objectsList.Add(item.PlacementInfo.Place_Tile);
                }
                break;

            case Define.TileType.Facility:
                foreach (var item in facilityList)
                {
                    objectsList.Add(item.PlacementInfo.Place_Tile);
                }
                break;
        }


        return objectsList;
    }








    //#region Tilemap_Legacy
    //void Init_TileMap()
    //{
    //    Vector2Int size = new Vector2Int((int)(BoxCollider.bounds.size.x * 2), (int)(BoxCollider.bounds.size.y * 2));
    //    TileMap = new BasementTile[size.x, size.y];

    //    float standard = 0.5f;
    //    float offset = 0.25f;

    //    for (int i = 0; i < size.x; i++)
    //    {
    //        for (int k = 0; k < size.y; k++)
    //        {
    //            Vector3 pos = new Vector2((-size.x * offset) + (standard * i) + offset, (-size.y * offset) + (standard * k) + offset);
    //            Vector3 worldPos = pos + transform.position;
    //            TileMap[i, k] = new BasementTile(new Vector2Int(i, k), worldPos, Define.TileType.Empty, this);
    //        }
    //    }
    //}


    //public BasementTile GetRandomTile()
    //{
    //    int whileCount = 0; //? 무한루프 방지용
    //    Vector2Int randomTile;

    //    BasementTile emptyTile = null;
    //    while (emptyTile == null && whileCount < 50)
    //    {
    //        whileCount++;
    //        randomTile = new Vector2Int(UnityEngine.Random.Range(0, TileMap.GetLength(0)), UnityEngine.Random.Range(0, TileMap.GetLength(1)));
    //        if (TileMap[randomTile.x, randomTile.y].placementable == null)
    //        {
    //            emptyTile = TileMap[randomTile.x, randomTile.y];
    //        }
    //    }

    //    if (emptyTile == null)
    //    {
    //        randomTile = new Vector2Int(UnityEngine.Random.Range(0, TileMap.GetLength(0)), UnityEngine.Random.Range(0, TileMap.GetLength(1)));
    //        emptyTile = TileMap[randomTile.x, randomTile.y];
    //    }

    //    return emptyTile;
    //}





    //public BasementTile MoveUp(IPlacementable placementable, BasementTile currentTile)
    //{
    //    if (currentTile.index.y + 1 == TileMap.GetLength(1)) return null;
    //    if (TileMap[currentTile.index.x, currentTile.index.y + 1].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

    //    return TileMap[currentTile.index.x, currentTile.index.y + 1];
    //}
    //public BasementTile MoveDown(IPlacementable placementable, BasementTile currentTile)
    //{
    //    if (currentTile.index.y == 0) return null;
    //    if (TileMap[currentTile.index.x, currentTile.index.y - 1].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

    //    return TileMap[currentTile.index.x, currentTile.index.y - 1];
    //}
    //public BasementTile MoveLeft(IPlacementable placementable, BasementTile currentTile)
    //{
    //    if (currentTile.index.x == 0) return null;
    //    if (TileMap[currentTile.index.x - 1, currentTile.index.y].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

    //    return TileMap[currentTile.index.x - 1, currentTile.index.y];
    //}
    //public BasementTile MoveRight(IPlacementable placementable, BasementTile currentTile)
    //{
    //    if (currentTile.index.x + 1 == TileMap.GetLength(0)) return null;
    //    if (TileMap[currentTile.index.x + 1, currentTile.index.y].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

    //    return TileMap[currentTile.index.x + 1, currentTile.index.y];
    //}

    //#region PathFinding

    //public List<BasementTile> PathFinding(BasementTile startPoint, BasementTile targetPoint, Define.TileType[] avoidType, out bool isFind)
    //{
    //    //? 순서는 위 아래 왼쪽 오른쪽 순서
    //    int[] deltaX = new int[4] { 0, 0, -1, 1 };
    //    int[] deltaY = new int[4] { 1, -1, 0, 0 };

    //    bool[,] closed = new bool[TileMap.GetLength(0), TileMap.GetLength(1)];

    //    PriorityQueue<PQNode> priorityQueue = new PriorityQueue<PQNode>();
    //    Vector2Int[,] pathTile = new Vector2Int[TileMap.GetLength(0), TileMap.GetLength(1)];


    //    priorityQueue.Push(new PQNode()
    //    {
    //        F = Vector3.Distance(startPoint.worldPosition, targetPoint.worldPosition),
    //        posX = startPoint.index.x,
    //        posY = startPoint.index.y
    //    });

    //    //? 첨에 엔드포인트도 정해줌
    //    pathTile[startPoint.index.x, startPoint.index.y] = new Vector2Int(startPoint.index.x, startPoint.index.y);
    //    pathTile[targetPoint.index.x, targetPoint.index.y] = new Vector2Int(targetPoint.index.x, targetPoint.index.y);

    //    while (priorityQueue.Count > 0)
    //    {
    //        PQNode node = priorityQueue.Pop();

    //        //? 이미 방문한곳이면 스킵
    //        if (closed[node.posX, node.posY])
    //            continue;

    //        closed[node.posX, node.posY] = true;

    //        //? 도착이면 바로 끝내기
    //        if (node.posX == targetPoint.index.x && node.posY == targetPoint.index.y)
    //        {
    //            break;
    //        }

    //        for (int i = 0; i < deltaX.Length; i++)
    //        {
    //            int nextX = node.posX + deltaX[i];
    //            int nextY = node.posY + deltaY[i];

    //            if (nextX == TileMap.GetLength(0) || nextX < 0 || nextY == TileMap.GetLength(1) || nextY < 0)
    //            {
    //                continue;
    //            }
    //            if (closed[nextX, nextY])
    //                continue;


    //            //? 사용중타일 = 갈수없는 타일
    //            if (TileMap[nextX, nextY].tileType == Define.TileType.Using)
    //            {
    //                continue;
    //            }

    //            //? 추가한 회피 조건
    //            bool avoid = false;
    //            foreach (var type in avoidType)
    //            {
    //                if (TileMap[nextX, nextY].tileType == type)
    //                {
    //                    avoid = true;
    //                    break;
    //                }
    //            }

    //            if (avoid)
    //            {
    //                continue;
    //            }


    //            priorityQueue.Push(new PQNode()
    //            {
    //                F = Vector3.Distance(TileMap[nextX, nextY].worldPosition, targetPoint.worldPosition),
    //                posX = nextX,
    //                posY = nextY
    //            });
    //            pathTile[nextX, nextY] = new Vector2Int(node.posX, node.posY);
    //        }
    //    }

    //    return (MoveList(pathTile, targetPoint, out isFind));
    //}



    //public List<BasementTile> PathFinding(BasementTile startPoint, BasementTile targetPoint, out bool isFind)
    //{
    //    //? 순서는 위 아래 왼쪽 오른쪽 순서
    //    int[] deltaX = new int[4] { 0, 0, -1, 1 };
    //    int[] deltaY = new int[4] { 1, -1, 0, 0 };

    //    bool[,] closed = new bool[TileMap.GetLength(0), TileMap.GetLength(1)];

    //    PriorityQueue<PQNode> priorityQueue = new PriorityQueue<PQNode>();
    //    Vector2Int[,] pathTile = new Vector2Int[TileMap.GetLength(0), TileMap.GetLength(1)];


    //    priorityQueue.Push(new PQNode()
    //    {
    //        F = Vector3.Distance(startPoint.worldPosition, targetPoint.worldPosition),
    //        posX = startPoint.index.x,
    //        posY = startPoint.index.y
    //    });

    //    pathTile[startPoint.index.x, startPoint.index.y] = new Vector2Int(startPoint.index.x, startPoint.index.y);
    //    pathTile[targetPoint.index.x, targetPoint.index.y] = new Vector2Int(targetPoint.index.x, targetPoint.index.y);

    //    while (priorityQueue.Count > 0)
    //    {
    //        PQNode node = priorityQueue.Pop();

    //        //? 이미 방문한곳이면 스킵
    //        if (closed[node.posX, node.posY])
    //            continue;

    //        closed[node.posX, node.posY] = true;

    //        //? 도착이면 스킵
    //        if (node.posX == targetPoint.index.x && node.posY == targetPoint.index.y)
    //            break;


    //        for (int i = 0; i < deltaX.Length; i++)
    //        {
    //            int nextX = node.posX + deltaX[i];
    //            int nextY = node.posY + deltaY[i];

    //            if (nextX == TileMap.GetLength(0) || nextX < 0 || nextY == TileMap.GetLength(1) || nextY < 0)
    //            {
    //                continue;
    //            }
    //            if (closed[nextX, nextY])
    //                continue;


    //            if (TileMap[nextX, nextY].tileType == Define.TileType.Using)
    //            {
    //                continue;
    //            }


    //            priorityQueue.Push(new PQNode()
    //            {
    //                F = Vector3.Distance(TileMap[nextX, nextY].worldPosition, targetPoint.worldPosition),
    //                posX = nextX,
    //                posY = nextY
    //            });
    //            pathTile[nextX, nextY] = new Vector2Int(node.posX, node.posY);
    //        }
    //    }


    //    return (MoveList(pathTile, targetPoint, out isFind));
    //}



    //List<BasementTile> MoveList(Vector2Int[,] path, BasementTile destination, out bool isFind)
    //{
    //    List<BasementTile> moveList = new List<BasementTile>();
    //    moveList.Add(destination);

    //    int x = destination.index.x;
    //    int y = destination.index.y;

    //    while (path[x, y].x != x || path[x, y].y != y)  //? 여기서 path[x,y]는 목적지를 거쳐온 경로임. 목적지에 도달할 경로가 없으면 스킵되는거
    //    {
    //        moveList.Add(TileMap[path[x, y].x, path[x, y].y]);
    //        Vector2Int pos = path[x, y];
    //        x = pos.x;
    //        y = pos.y;
    //    }
    //    moveList.Reverse();      //? 전체 경로를 추가했으면 시작부터 탐색해야하므로 반전시켜준다.

    //    if (moveList.Count == 1)
    //    {
    //        isFind = false;
    //        return moveList;
    //    }

    //    isFind = true;
    //    return moveList;
    //}


    //#endregion Pathfinding


    //#endregion







}


public class BasementTile
{
    public BasementFloor floor;

    public Vector2Int index;
    public Vector3 worldPosition;

    public Define.TileType tileType;
    Define.TileType tileType_unchange;

    public IPlacementable placementable;
    public IPlacementable unchangeable;
    public bool isUnchangeable { get { return (unchangeable != null); } }


    public BasementTile(Vector2Int _index, Vector3 _worldPosition, Define.TileType _type, BasementFloor _floor)
    {
        floor = _floor;
        index = _index;
        worldPosition = _worldPosition;
        tileType = _type;
    }


    public void SetUnchangeable(IPlacementable _placementable)
    {
        placementable = _placementable;
        unchangeable = _placementable;

        if (_placementable.GetType() == typeof(Entrance))
        {
            tileType = Define.TileType.Entrance;
            tileType_unchange = Define.TileType.Entrance;
        }
        else if (_placementable.GetType() == typeof(Exit))
        {
            tileType = Define.TileType.Exit;
            tileType_unchange = Define.TileType.Exit;
        }
        else if (_placementable.GetType() == typeof(SpecialEgg))
        {
            tileType = Define.TileType.Special;
            tileType_unchange = Define.TileType.Special;
        }
        else
        {
            tileType = Define.TileType.Facility; //? 입구 출구가 아니고 변하지 않는 퍼실리티 = 지나가면 발동하는 설치형 함정 등등
            tileType_unchange = Define.TileType.Facility;
        }
    }

    public void SetPlacement(IPlacementable _placementable)
    {
        if (isUnchangeable) //? 얘는 타입을 바꾸지않음.(다른 객체가 들어올 순 있어도 타입은 불변이라 Clear에서도 바꿔줄 필요 없음)
        {
            placementable = _placementable;

            if (tileType_unchange == Define.TileType.Facility)
            {
                tileType = Define.TileType.Using;
            }
            return;
        }
        else
        {
            placementable = _placementable;

            switch (_placementable.PlacementType)
            {
                case Define.PlacementType.Facility:
                    tileType = Define.TileType.Facility;
                    break;
                case Define.PlacementType.Monster:
                    tileType = Define.TileType.Monster;
                    break;
                case Define.PlacementType.NPC:
                    tileType = Define.TileType.NPC;
                    break;
            }
        }
    }

    public void ClearPlacement()
    {
        if (isUnchangeable)
        {
            placementable = unchangeable;
            tileType = tileType_unchange;
        }
        else
        {
            placementable = null;
            tileType = Define.TileType.Empty;
        }
    }
    public void ClearAbsolute()
    {
        placementable = null;
        unchangeable = null;
        tileType = Define.TileType.Empty;
        tileType_unchange = Define.TileType.Empty;
    }


    public Define.PlaceEvent TryPlacement(IPlacementable _placementable, bool overlap = false)
    {
        switch (tileType)
        {
            case Define.TileType.Empty:
                return Define.PlaceEvent.Placement;

            case Define.TileType.Special:
                return Define.PlaceEvent.Event;

            case Define.TileType.Entrance:
                return Define.PlaceEvent.Entrance;

            case Define.TileType.Exit:
                return Define.PlaceEvent.Exit;

            case Define.TileType.Using:
                return Define.PlaceEvent.Avoid;


            case Define.TileType.Monster:
                switch (_placementable.PlacementType)
                {
                    case Define.PlacementType.NPC:
                        return Define.PlaceEvent.Battle;

                    default:
                        return Define.PlaceEvent.Nothing;
                }

            case Define.TileType.NPC:
                switch (_placementable.PlacementType)
                {
                    case Define.PlacementType.Facility:
                        return Define.PlaceEvent.Interaction;

                    case Define.PlacementType.Monster:
                        return Define.PlaceEvent.Battle;

                    case Define.PlacementType.NPC:
                        if (overlap)
                        {
                            return Define.PlaceEvent.Overlap;
                        }
                        return Define.PlaceEvent.Avoid;
                    //var npc = placementable as NPC;
                    //if (npc.State == NPC.NPCState.Interaction)
                    //{
                    //    return Define.PlaceEvent.Avoid;
                    //}
                    //else
                    //{
                    //    return Define.PlaceEvent.Nothing;
                    //}

                    default:
                        return Define.PlaceEvent.Nothing;
                }

            case Define.TileType.Facility:
                switch (_placementable.PlacementType)
                {
                    case Define.PlacementType.NPC:
                        if (isUnchangeable)
                        {
                            var facil = unchangeable as Facility;
                            if (facil.Type == Facility.FacilityType.Portal)
                            {
                                return Define.PlaceEvent.Using_Portal;
                            }
                            if (facil.Type == Facility.FacilityType.Event)
                            {
                                return Define.PlaceEvent.Event;
                            }
                            return Define.PlaceEvent.Using;
                        }
                        else
                        {
                            return Define.PlaceEvent.Interaction;
                        }
                        

                    default:
                        return Define.PlaceEvent.Nothing;
                }


            default:
                return Define.PlaceEvent.Nothing;
        }
    }
}

