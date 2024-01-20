using System.Collections.Generic;
using UnityEngine;

public class BasementFloor : MonoBehaviour
{
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        Floor = gameObject.name;


        Vector2Int size = new Vector2Int((int)(boxCollider.bounds.size.x * 2), (int)(boxCollider.bounds.size.y * 2));
        TileMap = new BasementTile[size.x, size.y];

        float standard = 0.5f;
        float offset = 0.25f;

        for (int i = 0; i < size.x; i++)
        {
            for (int k = 0; k < size.y; k++)
            {
                Vector3 pos = new Vector2((-size.x * offset) + (standard * i) + offset, (-size.y * offset) + (standard * k) + offset);
                Vector3 worldPos = pos + transform.position;
                TileMap[i, k] = new BasementTile(new Vector2Int(i, k), worldPos, Define.TileType.Empty);
            }
        }
    }



    public string Floor { get; set; }

    public string Name_KR;

    public int Size { get; set; } = 1;

    public BoxCollider2D boxCollider;

    public List<NPC> npcList;

    public List<Monster> monsterList;

    public List<Facility> facilityList;

    public BasementTile[,] TileMap { get; set; }




    public List<BasementTile> SearchAllObjects(Define.TileType searchType = Define.TileType.Empty)
    {
        List<BasementTile> objectsList = new List<BasementTile>();

        for (int i = 0; i < TileMap.GetLength(0); i++)
        {
            for (int k = 0; k < TileMap.GetLength(1); k++)
            {
                if (TileMap[i,k].placementable != null)
                {
                    objectsList.Add(TileMap[i, k]);
                }
            }
        }

        if (searchType == Define.TileType.Empty)
        {
            return objectsList;
        }
        return SearchTarget(objectsList, searchType);
    }

    List<BasementTile> SearchTarget(List<BasementTile> listAll, Define.TileType type)
    {
        List<BasementTile> targetList = new List<BasementTile>();
        
        foreach (var item in listAll)
        {
            if (item.tileType == type)
            {
                targetList.Add(item);
            }
        }
        return targetList;
    }



    public BasementTile GetRandomTile(Interface.IPlacementable placementable)
    {
        int whileCount = 0; //? 무한루프 방지용
        var randomTile = new Vector2Int(Random.Range(0, TileMap.GetLength(0)), Random.Range(0, TileMap.GetLength(1)));

        var tile = TileMap[randomTile.x, randomTile.y].TryPlacement(placementable);
        while (tile != Define.PlaceEvent.Placement && whileCount < 20)
        {
            whileCount++;
            randomTile = new Vector2Int(Random.Range(0, TileMap.GetLength(0)), Random.Range(0, TileMap.GetLength(1)));
            tile = TileMap[randomTile.x, randomTile.y].TryPlacement(placementable);
        }

        return TileMap[randomTile.x, randomTile.y];
    }


    public BasementTile MoveUp(Interface.IPlacementable placementable, BasementTile currentTile)
    {
        if (currentTile.index.y + 1 == TileMap.GetLength(1)) return null;
        if (TileMap[currentTile.index.x, currentTile.index.y + 1].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

        return TileMap[currentTile.index.x, currentTile.index.y + 1];
    }
    public BasementTile MoveDown(Interface.IPlacementable placementable, BasementTile currentTile)
    {
        if (currentTile.index.y == 0) return null;
        if (TileMap[currentTile.index.x, currentTile.index.y - 1].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

        return TileMap[currentTile.index.x, currentTile.index.y - 1];
    }
    public BasementTile MoveLeft(Interface.IPlacementable placementable, BasementTile currentTile)
    {
        if (currentTile.index.x == 0) return null;
        if (TileMap[currentTile.index.x - 1, currentTile.index.y].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

        return TileMap[currentTile.index.x - 1, currentTile.index.y];
    }
    public BasementTile MoveRight(Interface.IPlacementable placementable, BasementTile currentTile)
    {
        if (currentTile.index.x + 1 == TileMap.GetLength(0)) return null;
        if (TileMap[currentTile.index.x + 1, currentTile.index.y].TryPlacement(placementable) != Define.PlaceEvent.Placement) return null;

        return TileMap[currentTile.index.x + 1, currentTile.index.y];
    }





    public List<BasementTile> PathFinding(BasementTile startPoint, BasementTile targetPoint)
    {
        //? 순서는 위 아래 왼쪽 오른쪽 순서
        int[] deltaX = new int[4] { 0, 0, -1, 1 };
        int[] deltaY = new int[4] { 1, -1, 0, 0 };

        bool[,] closed = new bool[TileMap.GetLength(0), TileMap.GetLength(1)];

        PriorityQueue<PQNode> priorityQueue = new PriorityQueue<PQNode>();
        Vector2Int[,] pathTile = new Vector2Int[TileMap.GetLength(0), TileMap.GetLength(1)];


        priorityQueue.Push(new PQNode()
        {
            F = Vector3.Distance(startPoint.worldPosition, targetPoint.worldPosition),
            posX = startPoint.index.x,
            posY = startPoint.index.y
        });
        pathTile[startPoint.index.x, startPoint.index.y] = new Vector2Int(startPoint.index.x, startPoint.index.y);

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

                if (nextX == TileMap.GetLength(0) || nextX < 0 || nextY == TileMap.GetLength(1) || nextY < 0)
                {
                    continue;
                }
                if (closed[nextX, nextY])
                    continue;

                priorityQueue.Push(new PQNode()
                {
                    F = Vector3.Distance(TileMap[nextX, nextY].worldPosition, targetPoint.worldPosition),
                    posX = nextX,
                    posY = nextY
                });
                pathTile[nextX, nextY] = new Vector2Int(node.posX, node.posY);
            }
        }


        return (MoveList(pathTile, targetPoint));
    }



    List<BasementTile> MoveList(Vector2Int[,] path, BasementTile destination)
    {
        List<BasementTile> moveList = new List<BasementTile>();
        moveList.Add(destination);

        int x = destination.index.x;
        int y = destination.index.y;

        while (path[x, y].x != x || path[x, y].y != y)  //? 목적지부터 경로를 거슬러 올라가서 하나씩 추가
        {
            moveList.Add(TileMap[path[x, y].x, path[x, y].y]);
            Vector2Int pos = path[x, y];
            x = pos.x;
            y = pos.y;
        }
        moveList.Reverse();      //? 전체 경로를 추가했으면 시작부터 탐색해야하므로 반전시켜준다.

        return moveList;
    }


}


public class BasementTile
{
    public Vector2Int index;
    public Vector3 worldPosition;
    public Define.TileType tileType;
    public Interface.IPlacementable placementable;


    public BasementTile(Vector2Int _index, Vector3 _worldPosition, Define.TileType _type)
    {
        index = _index;
        worldPosition = _worldPosition;
        tileType = _type;
    }


    public void SetPlacement(Interface.IPlacementable _placementable)
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

    public void ClearPlacement()
    {
        placementable = null;
        tileType = Define.TileType.Empty;
    }


    public Define.PlaceEvent TryPlacement(Interface.IPlacementable _placementable)
    {
        switch (tileType)
        {
            case Define.TileType.Empty:
                return Define.PlaceEvent.Placement;


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

                    default:
                        return Define.PlaceEvent.Nothing;
                }

            case Define.TileType.Facility:
                switch (_placementable.PlacementType)
                {
                    case Define.PlacementType.NPC:
                        return Define.PlaceEvent.Interaction;

                    default:
                        return Define.PlaceEvent.Nothing;
                }


            default:
                return Define.PlaceEvent.Nothing;
        }
    }
}

