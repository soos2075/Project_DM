using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData_Guild : MonoBehaviour
{
    public Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    void Start()
    {
        Init_GuildTileMap();
    }



    public Dictionary<Vector2Int, GuildTile> guildTileMap;


    void Init_GuildTileMap()
    {
        guildTileMap = new Dictionary<Vector2Int, GuildTile>();

        var tt = tilemap.cellBounds.allPositionsWithin;

        int offset_X = tilemap.cellBounds.xMin;
        int offset_Y = tilemap.cellBounds.yMin;

        //Debug.Log(offset_X + "@" + offset_Y);

        foreach (var pos in tt)
        {
            if (tilemap.HasTile(pos))
            {
                var cellPos = new Vector2Int(pos.x, pos.y);
                var index = cellPos - new Vector2Int(offset_X, offset_Y);
                var worldPos = tilemap.CellToWorld(pos);

                guildTileMap.Add(index, new GuildTile(false, worldPos, index));
                //Debug.Log(index + $"갈 수 없는 좌표");
            }
            else
            {
                var cellPos = new Vector2Int(pos.x, pos.y);
                var index = cellPos - new Vector2Int(offset_X, offset_Y);
                var worldPos = tilemap.CellToWorld(pos);

                guildTileMap.Add(index, new GuildTile(true, worldPos, index));
                //Debug.Log(index + $"갈 수 있는 좌표");
            }
        }
    }



    public class GuildTile
    {
        public bool isPath;

        public Vector3 worldPosition;

        public Vector2Int index;

        public GuildTile(bool pathAble, Vector3 _pos, Vector2Int _index)
        {
            isPath = pathAble;
            worldPosition = _pos;
            index = _index;
        }
    }



    //public GameObject player;
    //public GuildTile startPoint;
    //public GuildTile targetPoint;


    public List<GuildTile> PathFinding(GuildTile startPoint, GuildTile targetPoint)
    {
        //? 순서는 위 아래 왼쪽 오른쪽 순서 // 좌상 좌하 우상 우하
        //int[] deltaX = new int[4] { 0, 0, -1, 1 };
        //int[] deltaY = new int[4] { 1, -1, 0, 0 };
        int[] deltaX = new int[8] { 0, 0, -1, 1, -1, -1, 1, 1 };
        int[] deltaY = new int[8] { 1, -1, 0, 0, 1, -1, 1, -1 };

        bool[,] closed = new bool[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];

        PriorityQueue<PQNode> priorityQueue = new PriorityQueue<PQNode>();
        Vector2Int[,] pathTile = new Vector2Int[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];


        priorityQueue.Push(new PQNode()
        {
            F = Vector2.Distance(startPoint.worldPosition, targetPoint.worldPosition) + 1,
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

                GuildTile value = null;
                if (guildTileMap.TryGetValue(new Vector2Int(nextX, nextY), out value) == false)
                {
                    continue;
                }

                if (value.isPath == false)
                {
                    continue;
                }


                float Weights = i < 4 ? 1 : Mathf.Sqrt(2.0f);
                priorityQueue.Push(new PQNode()
                {
                    F = Vector2.Distance(value.worldPosition, targetPoint.worldPosition) + Weights,
                    posX = nextX,
                    posY = nextY
                });
                pathTile[nextX, nextY] = new Vector2Int(node.posX, node.posY);
            }
        }
        return MoveList(pathTile, startPoint, targetPoint);
    }

    List<GuildTile> MoveList(Vector2Int[,] path, GuildTile startPoint, GuildTile targetPoint)
    {
        List<GuildTile> moveList = new List<GuildTile>();
        moveList.Add(targetPoint);

        int x = targetPoint.index.x;
        int y = targetPoint.index.y;

        float F = 0;
        float currentF = 0;

        while (path[x, y].x != x || path[x, y].y != y)  //? 여기서 path[x,y]는 목적지를 거쳐온 경로임. 목적지에 도달할 경로가 없으면 스킵되는거
        {
            GuildTile temp = null;
            guildTileMap.TryGetValue(new Vector2Int(path[x, y].x, path[x, y].y), out temp);

            currentF = Vector2.Distance(temp.worldPosition, targetPoint.worldPosition);
            if (currentF > F)
            {
                F = currentF;
                moveList.Add(temp);
            }

            //moveList.Add(temp);
            Vector2Int pos = path[x, y];
            x = pos.x;
            y = pos.y;
        }
        moveList.Reverse();      //? 전체 경로를 추가했으면 시작부터 탐색해야하므로 반전시켜준다.

        return moveList;
    }
}
