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

    public string LabelName { get; set; }

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
                if (tempTile.Original == null)
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


                if (value.Original != null && value.Original.PlacementState == PlacementState.Busy)
                {
                    continue;
                }

                //? 추가한 회피 조건
                bool avoid = false;
                foreach (var type in avoidType)
                {
                    if (value.tileType_Original == type)
                    {
                        if (type == Define.TileType.Facility)
                        {
                            var facil = value.Original as Facility;
                            if (facil.Type == Facility.FacilityEventType.NPC_Interaction)
                            {
                                avoid = true;
                                break;
                            }
                        }
                        else
                        {
                            avoid = true;
                            break;
                        }
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


                //if (value.tileType == Define.TileType.Using)
                //{
                //    continue;
                //}


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



    //? 처리는 받아간곳에서
    public BasementTile GetTileUp(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x, currentTile.index.y + 1), out tile))
        {
            return tile;
        }
        return null;
    }
    public BasementTile GetTileDown(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x, currentTile.index.y - 1), out tile))
        {
            return tile;
        }
        return null;
    }
    public BasementTile GetTileLeft(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x - 1, currentTile.index.y), out tile))
        {
            return tile;
        }
        return null;
    }
    public BasementTile GetTileRight(IPlacementable placementable, BasementTile currentTile)
    {
        BasementTile tile = null;
        if (TileMap.TryGetValue(new Vector2Int(currentTile.index.x + 1, currentTile.index.y), out tile))
        {
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
                obj = GameManager.Facility.CreateFacility_OnlyOne("Entrance", info);
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
                obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info);
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

        if (PickObjectOfType(typeof(Entrance)) == null)
        {
            var info = new PlacementInfo(this, GetRandomTile());
            GameManager.Facility.CreateFacility_OnlyOne("Entrance", info);
        }

        if (PickObjectOfType(typeof(Exit)) == null)
        {
            var info = new PlacementInfo(this, GetRandomTile());
            GameManager.Facility.CreateFacility_OnlyOne("Exit", info);
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



}

[Serializable]
public class BasementTile
{
    public BasementFloor floor;

    public Vector2Int index;
    public Vector3 worldPosition;

    Define.TileType tileType_Current;
    public Define.TileType tileType_Original;

    IPlacementable Current { get; set; }
    public IPlacementable Original { get; set; }


    public BasementTile(Vector2Int _index, Vector3 _worldPosition, Define.TileType _type, BasementFloor _floor)
    {
        floor = _floor;
        index = _index;
        worldPosition = _worldPosition;
        tileType_Original = _type;
        tileType_Current = _type;
    }

    public void SetPlacement_Facility(IPlacementable _placementable)
    {
        Current = _placementable;
        Original = _placementable;

        if (_placementable.GetType() == typeof(Statue))
        {
            tileType_Current = Define.TileType.Player;
            tileType_Original = Define.TileType.Player;
        }
        else if (_placementable.GetType() == typeof(Obstacle))
        {
            tileType_Current = Define.TileType.Non_Interaction;
            tileType_Original = Define.TileType.Non_Interaction;
        }
        else
        {
            tileType_Current = Define.TileType.Facility; //? 입구 출구가 아니고 변하지 않는 퍼실리티 = 지나가면 발동하는 설치형 함정 등등
            tileType_Original = Define.TileType.Facility;
        }
    }

    public void SetPlacement(IPlacementable _placementable)
    {
        switch (_placementable.PlacementType)
        {
            case PlacementType.Facility:
                //Debug.Log("오류!! 여긴 퍼실리티가 호출하면 안됨");
                SetPlacement_Facility(_placementable);
                return;

            case PlacementType.Monster:
                tileType_Current = Define.TileType.Monster;
                break;
            case PlacementType.NPC:
                tileType_Current = Define.TileType.NPC;
                break;
        }

        Current = _placementable;

        if (Original == null)
        {
            Original = _placementable;
            tileType_Original = tileType_Current;
        }
    }

    public void ClearPlacement(IPlacementable placementable)
    {
        if (placementable == Original)
        {
            if (Current != null &&  Original != Current)
            {
                Original = null;
                tileType_Original = Define.TileType.Empty;
                SetPlacement(Current);
            }
            else
            {
                ClearAbsolute();
            }

        }
        else if (placementable == Current)
        {
            Current = Original;
            tileType_Current = tileType_Original;
            Original.PlacementState = PlacementState.Standby;
        }
    }
    public void ClearAbsolute()
    {
        Current = null;
        Original = null;
        tileType_Current = Define.TileType.Empty;
        tileType_Original = Define.TileType.Empty;
    }


    public Define.PlaceEvent TryPlacement(IPlacementable _placementable, bool overlap = false)
    {
        switch (tileType_Original)
        {
            case Define.TileType.Empty:
                return Define.PlaceEvent.Placement;

            case Define.TileType.Monster:
                if (_placementable.PlacementType == PlacementType.NPC)
                {
                    if (Original.PlacementState == PlacementState.Standby)
                    {
                        return Define.PlaceEvent.Battle;
                    }
                    else
                    {
                        return Define.PlaceEvent.Avoid;
                    }
                }
                else
                    return Define.PlaceEvent.Nothing;

            case Define.TileType.NPC:
                if (_placementable.PlacementType == PlacementType.Monster)
                {
                    if (Original.PlacementState == PlacementState.Standby)
                    {
                        return Define.PlaceEvent.Battle;
                    }
                    else
                    {
                        return Define.PlaceEvent.Avoid;
                    }
                }
                else if(_placementable.PlacementType == PlacementType.NPC)
                {
                    if (overlap)
                    {
                        return Define.PlaceEvent.Placement;
                    }
                    else
                    {
                        return Define.PlaceEvent.Avoid;
                    }
                    
                }
                else
                    return Define.PlaceEvent.Nothing;


            case Define.TileType.Facility:
                if (_placementable.PlacementType == PlacementType.NPC)
                {
                    if (Original.GetType() == typeof(Entrance) || Original.GetType() == typeof(Exit))
                    {
                        return Define.PlaceEvent.Event;
                    }


                    if (Original.PlacementState == PlacementState.Standby)
                    {
                        var facil = Original as Facility;
                        if (facil.Type == Facility.FacilityEventType.NPC_Interaction)
                        {
                            return Define.PlaceEvent.Interaction;
                        }
                        else if (facil.Type == Facility.FacilityEventType.NPC_Event)
                        {
                            return Define.PlaceEvent.Event;
                        }
                        else
                        {
                            return Define.PlaceEvent.Nothing;
                        }
                    }
                    else
                    {
                        if (overlap)
                        {
                            return Define.PlaceEvent.Placement;
                        }
                        else
                        {
                            return Define.PlaceEvent.Avoid;
                        }
                    }
                }
                else
                    return Define.PlaceEvent.Avoid;



            case Define.TileType.Player:
                return Define.PlaceEvent.Nothing;

            case Define.TileType.Non_Interaction:
                return Define.PlaceEvent.Nothing;


            default:
                return Define.PlaceEvent.Nothing;
        }
    }
}

