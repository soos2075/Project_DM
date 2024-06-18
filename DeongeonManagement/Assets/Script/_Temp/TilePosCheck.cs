using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePosCheck : MonoBehaviour
{
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

    //        Debug.Log("Tile clicked at: " + cellPosition);
    //    }
    //}


    private void OnMouseDown()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        int offset_X = tilemap.cellBounds.xMin;
        int offset_Y = tilemap.cellBounds.yMin;

        var saveKey = new Vector2Int(cellPosition.x - offset_X, cellPosition.y - offset_Y);

        Debug.Log("Tile clicked at: " + saveKey);




    }
}
