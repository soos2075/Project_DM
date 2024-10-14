using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData : MonoBehaviour
{

    public Tilemap FloorTileMap { get; set; }

    private void Awake()
    {
        FloorTileMap = GetComponent<Tilemap>();
    }
    void Start()
    {
        //var dic = GetDictionary(new BasementFloor());
    }


    public Dictionary<Vector2Int, BasementTile> GetDictionary(BasementFloor floor)
    {
        var dic = new Dictionary<Vector2Int, BasementTile>();

        var tt = FloorTileMap.cellBounds.allPositionsWithin;

        int offset_X = FloorTileMap.cellBounds.xMin;
        int offset_Y = FloorTileMap.cellBounds.yMin;
        foreach (var pos in tt)
        {
            if (FloorTileMap.HasTile(pos))
            {
                //Debug.Log($"{gameObject.name} + {FloorTileMap.GetTile(pos).name}");

                if (FloorTileMap.GetTile(pos).name == "_Fixed" || FloorTileMap.GetTile(pos).name == "_Changeable")
                {
                    //Debug.Log("@@@@");
                    continue;
                }
                
                var saveKey = new Vector2Int(pos.x - offset_X, pos.y - offset_Y);
                dic.Add(saveKey, new BasementTile(saveKey, FloorTileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0), Define.TileType.Empty, floor));
                //Debug.Log(saveKey + "@@@@" + pos + "////");
                // + new Vector3(0.25f, 0.25f, 0)
            }
        }
        return dic;
    }

}
