using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData_Guild : MonoBehaviour
{
    public Tilemap tilemap_Main;
    public Tilemap tilemap_Obj;

    private void Awake()
    {
        //tilemap = GetComponent<Tilemap>();
    }
    void Start()
    {
        Init_GuildTileMap();
    }



    public Dictionary<Vector2Int, GuildTile> guildTileMap;


    void Init_GuildTileMap()
    {
        guildTileMap = new Dictionary<Vector2Int, GuildTile>();

        var tt = tilemap_Main.cellBounds.allPositionsWithin;

        int offset_X = tilemap_Main.cellBounds.xMin;
        int offset_Y = tilemap_Main.cellBounds.yMin;

        //Debug.Log(offset_X + "@" + offset_Y);

        foreach (var pos in tt)
        {
            if (tilemap_Main.HasTile(pos))
            {
                var cellPos = new Vector2Int(pos.x, pos.y);
                var index = cellPos - new Vector2Int(offset_X, offset_Y);
                var worldPos = tilemap_Main.CellToWorld(pos);

                guildTileMap.Add(index, new GuildTile(false, worldPos, index));
                //Debug.Log(index + $"�� �� ���� ��ǥ");
            }
            else
            {
                var cellPos = new Vector2Int(pos.x, pos.y);
                var index = cellPos - new Vector2Int(offset_X, offset_Y);
                var worldPos = tilemap_Main.CellToWorld(pos);

                guildTileMap.Add(index, new GuildTile(true, worldPos, index));
                //Debug.Log(index + $"�� �� �ִ� ��ǥ");
            }
        }

        Init_TilemapObj();
    }

    void Init_TilemapObj()
    {
        var tt = tilemap_Obj.cellBounds.allPositionsWithin;

        int offset_X = tilemap_Obj.cellBounds.xMin;
        int offset_Y = tilemap_Obj.cellBounds.yMin;

        //Debug.Log(offset_X + "@" + offset_Y);

        foreach (var pos in tt)
        {
            if (tilemap_Obj.HasTile(pos))
            {
                var cellPos = new Vector2Int(pos.x, pos.y);
                var index = cellPos - new Vector2Int(offset_X, offset_Y);
                var worldPos = tilemap_Obj.CellToWorld(pos);

                //GuildTile obj = null;
                //if (guildTileMap.TryGetValue(worldPos, out obj))
                //{
                //    obj.isPath = false;
                //    Debug.Log();
                //}

                foreach (var item in guildTileMap)
                {
                    if (item.Value.worldPosition == worldPos)
                    {
                        item.Value.isPath = false;
                    }
                    
                }

                //Debug.Log(index + $"�� �� ���� ��ǥ");
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
        //? 0�� 1�Ʒ� 2���� 3������ // 4�»� 5���� 6��� 7����
        int[] deltaX = new int[4] { 0, 0, -1, 1 };
        int[] deltaY = new int[4] { 1, -1, 0, 0 };

        deltaX = new int[8] { 0, 0, -1, 1, -1, -1, 1, 1 };
        deltaY = new int[8] { 1, -1, 0, 0, 1, -1, 1, -1 };

        bool[,] closed = new bool[tilemap_Main.cellBounds.size.x, tilemap_Main.cellBounds.size.y];

        PriorityQueue<PQNode> priorityQueue = new PriorityQueue<PQNode>();
        Vector2Int[,] pathTile = new Vector2Int[tilemap_Main.cellBounds.size.x, tilemap_Main.cellBounds.size.y];

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

            //? �̹� �湮�Ѱ��̸� ��ŵ
            if (closed[node.posX, node.posY])
                continue;

            closed[node.posX, node.posY] = true;

            //? �����̸� ��ŵ
            if (node.posX == targetPoint.index.x && node.posY == targetPoint.index.y)
                break;


            bool[] diagonalAllow = new bool[4] { true, true, true, true }; //? �밢�� �̵��� ������� ����

            for (int i = 0; i < deltaX.Length; i++)
            {
                if (i == 4 && diagonalAllow[0] == false)
                {
                    continue;
                }
                if (i == 5 && diagonalAllow[1] == false)
                {
                    continue;
                }
                if (i == 6 && diagonalAllow[2] == false)
                {
                    continue;
                }
                if (i == 7 && diagonalAllow[3] == false)
                {
                    continue;
                }



                int nextX = node.posX + deltaX[i];
                int nextY = node.posY + deltaY[i];

                if (nextX == tilemap_Main.cellBounds.size.x || nextX < 0 || nextY == tilemap_Main.cellBounds.size.y || nextY < 0)
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
                {//? �����¿찡 ���̶�� �ش������ �밢�� �̵��� ��ŵ��
                    if (i == 0) 
                    {
                        diagonalAllow[0] = false; diagonalAllow[2] = false;
                    }
                    if (i == 1)
                    {
                        diagonalAllow[1] = false; diagonalAllow[3] = false;
                    }
                    if (i == 2)
                    {
                        diagonalAllow[0] = false; diagonalAllow[1] = false;
                    }
                    if (i == 3)
                    {
                        diagonalAllow[2] = false; diagonalAllow[3] = false;
                    }
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

        while (path[x, y].x != x || path[x, y].y != y)  //? ���⼭ path[x,y]�� �������� ���Ŀ� �����. �������� ������ ��ΰ� ������ ��ŵ�Ǵ°�
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
        moveList.Reverse();      //? ��ü ��θ� �߰������� ���ۺ��� Ž���ؾ��ϹǷ� ���������ش�.

        return moveList;
    }
}
