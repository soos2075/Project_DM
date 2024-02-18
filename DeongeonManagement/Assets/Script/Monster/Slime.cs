using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    public override MonsterData Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetMonsterData("Slime");
        //Mode = MoveType.Move_Hunting;
    }

    public override void TurnStart()
    {
        MoveSelf();
    }



    void MoveSelf()
    {
        Cor_Moving = StartCoroutine(MoveCor());
    }


    IEnumerator MoveCor()
    {
        while (Main.Instance.Management == false && State == MonsterState.Placement)
        {
            yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
            switch (Mode)
            {
                case MoveType.Fixed:
                    break;
                case MoveType.Move_Wandering:
                    Move_Wandering();
                    break;
                case MoveType.Move_Hunting:
                    Moving();
                    break;
            }
        }
    }



    BasementTile GetRandomTile()
    {
        BasementTile newTile;

        int dir = Random.Range(0, 5);
        switch (dir)
        {
            case 0:
                newTile = PlacementInfo.Place_Floor.GetTileUp(this, PlacementInfo.Place_Tile);
                break;

            case 1:
                newTile = PlacementInfo.Place_Floor.GetTileDown(this, PlacementInfo.Place_Tile);
                break;

            case 2:
                newTile = PlacementInfo.Place_Floor.GetTileLeft(this, PlacementInfo.Place_Tile);
                break;

            case 3:
                newTile = PlacementInfo.Place_Floor.GetTileRight(this, PlacementInfo.Place_Tile);
                break;

            default:
                newTile = null;
                break;
        }

        return newTile;
    }

    void Moving()
    {
        BasementTile tile = GetRandomTile();
        if (tile != null)
        {
            var eventType = tile.TryPlacement(this);

            switch (eventType)
            {
                case Define.PlaceEvent.Placement:
                    GameManager.Placement.PlacementMove(this, new PlacementInfo(PlacementInfo.Place_Floor, tile));
                    Debug.Log($"이동 이벤트");
                    break;

                case Define.PlaceEvent.Battle:
                    var npc = tile.placementable as NPC;
                    switch (Mode)
                    {
                        case MoveType.Move_Wandering:

                            if (npc.Cor_Encounter == null)
                            {
                                npc.Cor_Encounter = StartCoroutine(npc.Encounter_ByMonster(this));
                                Debug.Log($"몬스터 배틀 이벤트");
                            }
                            break;

                        case MoveType.Move_Hunting:
                            if (npc.Cor_Encounter == null)
                            {
                                npc.Cor_Encounter = StartCoroutine(npc.Encounter_ByMonster(this));
                                Debug.Log($"몬스터 배틀 이벤트");
                            }
                            break;
                    }
                    break;

                default:
                    Debug.Log($"{eventType.ToString()} : 아무이벤트 없음");
                    break;
            }
        }
    }




    void Move_Wandering()
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
