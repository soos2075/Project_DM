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


        //? 타일위치 알고싶을 떄 쓰는 디버그용
        //if (FloorIndex == 4)
        //{
        //    foreach (var item in TileMap)
        //    {
        //        Debug.Log(item.Value.index + "//" + item.Value.worldPosition);
        //    }
        //}
    }

    public BasementTile GetRandomTile()
    {
        int whileCount = 0; //? 무한루프 방지용
        Vector2Int randomTile;

        BasementTile emptyTile = null;
        BasementTile tempTile = null;
        while (whileCount < 100 && emptyTile == null)
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

    public BasementTile GetRandomTile(out bool findEmpty)
    {
        int whileCount = 0; //? 무한루프 방지용
        Vector2Int randomTile;

        BasementTile tempTile = null;
        while (whileCount < 100)
        {
            whileCount++;
            randomTile = new Vector2Int(UnityEngine.Random.Range(0, tilemap.cellBounds.size.x), UnityEngine.Random.Range(0, tilemap.cellBounds.size.y));
            if (TileMap.TryGetValue(randomTile, out tempTile))
            {
                if (tempTile.Original == null)
                {
                    findEmpty = true;
                    return tempTile;
                }
            }
        }

        Debug.Log("빈 공간을 못찾음");
        findEmpty = false;
        return tempTile;
    }

    public BasementTile GetRandomTile_Common(out bool findEmpty)
    {
        int whileCount = 0; //? 무한루프 방지용
        Vector2Int randomTile;

        BasementTile tempTile = null;
        while (whileCount < 100)
        {
            whileCount++;
            randomTile = new Vector2Int(UnityEngine.Random.Range(0, tilemap.cellBounds.size.x), UnityEngine.Random.Range(0, tilemap.cellBounds.size.y));
            if (TileMap.TryGetValue(randomTile, out tempTile))
            {
                if (tempTile.Original == null)
                {
                    findEmpty = true;
                    return tempTile;
                }
            }
        }

        Debug.Log("빈 공간 없음 - 약초나 광물 제거");
        findEmpty = false;


        while (true)
        {
            randomTile = new Vector2Int(UnityEngine.Random.Range(0, tilemap.cellBounds.size.x), UnityEngine.Random.Range(0, tilemap.cellBounds.size.y));
            if (TileMap.TryGetValue(randomTile, out tempTile))
            {
                if (tempTile.Original.GetType() == typeof(Herb) || tempTile.Original.GetType() == typeof(Mineral))
                {
                    return tempTile;
                }
            }
        }


        //return tempTile;
    }



    public List<BasementTile> PathFinding(BasementTile startPoint, BasementTile targetPoint, Define.TileType[] avoidType, out bool isFind, 
        PathFindingType findType = PathFindingType.Normal)
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

                //? 상대가 바쁜 상태라면 건너뛰기(NPC,Monster,Facility 모두 포함)
                if (value.Original != null && value.Original.PlacementState == PlacementState.Busy)
                {
                    continue;
                }

                //? 갈 수 없는 타일이라면 건너뛰기
                if (findType == PathFindingType.Normal)
                {
                    if (Check_IWall(value)) continue;
                }
                else if (findType == PathFindingType.Allow_Wall)
                {
                    if (value.Original != null)
                    {
                        if (value.Original.GetType() == typeof(Obstacle_Wall))
                        {
                            continue;
                        }
                    }
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
                            if (facil.EventType == Facility.FacilityEventType.NPC_Interaction)
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
    public List<BasementTile> PathFinding(BasementTile startPoint, BasementTile targetPoint, out bool isFind, PathFindingType findType = PathFindingType.Normal)
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

                //? 갈 수 없는 타일이라면 건너뛰기
                if (findType == PathFindingType.Normal)
                {
                    if (Check_IWall(value)) continue;
                }
                else if (findType == PathFindingType.Allow_Wall)
                {
                    if (value.Original != null)
                    {
                        if (value.Original.GetType() == typeof(Obstacle_Wall))
                        {
                            continue;
                        }
                    }
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



    public enum PathFindingType
    {
        Normal,
        Allow_Wall,
    }
    bool Check_IWall(BasementTile tile)
    {
        if (tile.Original != null && tile.Original as IWall != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }




    // 몬스터버전 패스파인딩
    public List<BasementTile> PathFinding_Monster(BasementTile startPoint, BasementTile targetPoint, out bool isFind)
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

                //? 몬스터 회피 조건 - 빈타일과 NPC가 아닌 모든타일 건너뛰기
                if (value.tileType_Original != Define.TileType.Empty && value.tileType_Original != Define.TileType.NPC)
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
            bool isEmpty;
            var tile = GetRandomTile_Common(out isEmpty);
            var info = new PlacementInfo(this, tile);
            if (isEmpty == false)
            {
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }

            GameManager.Facility.CreateFacility_OnlyOne("Entrance", info);
        }

        if (PickObjectOfType(typeof(Exit)) == null)
        {
            bool isEmpty;
            var tile = GetRandomTile_Common(out isEmpty);
            var info = new PlacementInfo(this, tile);
            if (isEmpty == false)
            {
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }

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

    public Define.TileType tileType_Current;
    public Define.TileType tileType_Original;

    IPlacementable _current;
    public IPlacementable Current { get { return _current; } set { _current = value; GetObj_IPlacement(); } }

    IPlacementable _original;
    public IPlacementable Original { get { return _original; } set { _original = value; GetObj_IPlacement(); } }


    #region For Inspector Debug
    public GameObject Original_Obj;
    public GameObject Current_Obj;
    public void GetObj_IPlacement()
    {
        if (Current != null)
        {
            Current_Obj = Current.GetObject();
        }
        if (Original != null)
        {
            Original_Obj = Original.GetObject();
        }
    }
    #endregion

    public BasementTile(Vector2Int _index, Vector3 _worldPosition, Define.TileType _type, BasementFloor _floor)
    {
        floor = _floor;
        index = _index;
        worldPosition = _worldPosition;
        tileType_Original = _type;
        tileType_Current = _type;
    }

    public bool NonInteract_TileCheck() //? 240423 추가 / 빌드시, 플로어 선택시 타일을 숨기게 하는 역할
    {
        if (tileType_Original == Define.TileType.Non_Interaction || tileType_Original == Define.TileType.Player_Interaction)
        {
            return true;
        }

        if (tileType_Original == Define.TileType.Monster)
        {
            if (Original.GetType() == typeof(Player)) // 플레이어는 예외처리
            {
                return true;
            }
        }


        if (tileType_Original == Define.TileType.Facility)
        {
            if (Original.GetType() == typeof(SpecialEgg)) // 에그는 예외처리
            {
                return true;
            }

            var fa = Original as Facility;
            switch (fa.EventType)
            {
                case Facility.FacilityEventType.NPC_Interaction:
                    return false;

                case Facility.FacilityEventType.NPC_Event:
                    return false;

                case Facility.FacilityEventType.Player_Event:
                    return true;

                case Facility.FacilityEventType.Non_Interaction:
                    return true;
            }
        }

        return false;
    }

    public void SetPlacement_Facility(IPlacementable _placementable)
    {
        Current = _placementable;
        Original = _placementable;

        if (_placementable.GetType() == typeof(Statue))
        {
            tileType_Current = Define.TileType.Player_Interaction;
            tileType_Original = Define.TileType.Player_Interaction;
        }
        else if (_placementable.GetType() == typeof(Obstacle))
        {
            tileType_Current = Define.TileType.Non_Interaction;
            tileType_Original = Define.TileType.Non_Interaction;
        }
        else
        {
            tileType_Current = Define.TileType.Facility;
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
                        if (overlap)
                        {
                            return Define.PlaceEvent.Placement;
                        }
                        else
                        {
                            return Define.PlaceEvent.Avoid;
                        }
                        //return Define.PlaceEvent.Avoid;
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
                        if (facil.EventType == Facility.FacilityEventType.NPC_Interaction)
                        {
                            return Define.PlaceEvent.Interaction;
                        }
                        else if (facil.EventType == Facility.FacilityEventType.NPC_Event)
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



            case Define.TileType.Player_Interaction:
                return Define.PlaceEvent.Nothing;

            case Define.TileType.Non_Interaction:
                //return Define.PlaceEvent.Nothing;
                if (_placementable.PlacementType == PlacementType.Monster)
                {
                    return Define.PlaceEvent.Nothing;
                }
                if (Original as IWall != null)
                {
                    return Define.PlaceEvent.Nothing;
                }
                //Debug.Log("논인터렉션");
                return Define.PlaceEvent.Placement;


            default:
                return Define.PlaceEvent.Nothing;
        }
    }
}

public class TileDistanceComparer : IComparer<BasementTile>
{
    private Vector3 comparePosition;

    public TileDistanceComparer(Vector3 pos)
    {
        this.comparePosition = pos;
    }

    public int Compare(BasementTile x, BasementTile y)
    {
        float distanceX = Vector3.Distance(comparePosition, x.worldPosition);
        float distanceY = Vector3.Distance(comparePosition, y.worldPosition);

        if (distanceX < distanceY)
        {
            return -1;
        }
        else if (distanceX > distanceY)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

