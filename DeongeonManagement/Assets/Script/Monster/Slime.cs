using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    public override MonsterData Data { get; set; }
    public override MonsterType Type { get; set; }


    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetMonsterData("Slime");
        Type = MonsterType.Normal_Move;
    }


    void MoveSelf()
    {
        StartCoroutine(MoveCor());
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
                newTile = PlacementInfo.Place_Floor.MoveUp(this, PlacementInfo.Place_Tile);
                break;

            case 1:
                newTile = PlacementInfo.Place_Floor.MoveDown(this, PlacementInfo.Place_Tile);
                break;

            case 2:
                newTile = PlacementInfo.Place_Floor.MoveLeft(this, PlacementInfo.Place_Tile);
                break;

            case 3:
                newTile = PlacementInfo.Place_Floor.MoveRight(this, PlacementInfo.Place_Tile);
                break;

            default:
                newTile = null;
                break;
        }

        if (newTile != null)
        {
            GameManager.Placement.PlacementMove(this, new PlacementInfo(PlacementInfo.Place_Floor, newTile));
        }
    }

}
