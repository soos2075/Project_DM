using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    public override MonsterType Type { get; set; }


    protected override void MonsterInit()
    {
        Type = MonsterType.Normal_Move;
    }

    protected override void Initialize_Status()
    {
        SetStatus("Slime", 1, 10, 3, 1);
    }


    void MoveSelf()
    {
        moveCoroutine = StartCoroutine(MoveCor());
    }


    IEnumerator MoveCor()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            MoveAround();
        }
    }

    void MoveAround()
    {
        BasementTile newTile;

        int dir = Random.Range(0, 5);
        switch (dir)
        {
            case 0:
                newTile = Place_Floor.MoveUp(this, Place_Tile);
                break;

            case 1:
                newTile = Place_Floor.MoveDown(this, Place_Tile);
                break;

            case 2:
                newTile = Place_Floor.MoveLeft(this, Place_Tile);
                break;

            case 3:
                newTile = Place_Floor.MoveRight(this, Place_Tile);
                break;

            default:
                newTile = null;
                break;
        }

        if (newTile != null)
        {
            Place_Tile.ClearPlacement();
            PlacementConfirm(Place_Floor, newTile);
        }
    }

}
