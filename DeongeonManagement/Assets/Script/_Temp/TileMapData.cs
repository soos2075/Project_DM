using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData : MonoBehaviour
{
    Dictionary<Vector3Int, tlieData> tileList;


    public GameObject cube;

    public Tilemap tilemap1;
    void Start()
    {
        tileList = new Dictionary<Vector3Int, tlieData>();


        tilemap1 = transform.GetChild(2).GetComponent<Tilemap>();


        var tt = tilemap1.cellBounds.allPositionsWithin;

        foreach (var pos in tt)
        {
            if (tilemap1.HasTile(pos))
            {
                //var tile = tilemap1.GetTile<TileBase>(pos);

                Debug.Log(tilemap1.GetSprite(pos).name);
                //TileData data = tilemap1.get

                tileList.Add(pos, new tlieData(pos, tilemap1.CellToWorld(pos)));
            }
        }



        GetDictionary();
    }


    void GetDictionary()
    {
        foreach (var item in tileList)
        {
            var obj = Instantiate(cube);
            obj.transform.position = item.Value.worldPos + new Vector3(0.5f, 0.5f, 0);
        }
    }


    public class tlieData
    {
        public int floor;

        public Vector3 worldPos;
        public Vector3Int index;

        public tlieData(Vector3Int _index, Vector3 _pos)
        {
            index = _index;
            worldPos = _pos;
        }
    }

}
