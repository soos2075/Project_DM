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
        SetStatus(
            name: "ΩΩ∂Û¿”", 
            lv: 1,
            hp: 15,
            atk: 2,
            def: 4,
            agi: 2,
            luk: 3
            );
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
            Managers.Placement.PlacementMove(this, new PlacementInfo(PlacementInfo.Place_Floor, newTile));
        }
    }

}
