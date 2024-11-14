using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Devil_Statue : Facility, IWall
{
    public override void Init_Personal()
    {
        //Size = (SizeOption)CategoryIndex;
        AddEvent();
        OnRemoveEvent += () => RemoveEvent();


        if (isInit == false)
        {
            Create_Clone();
            isInit = true;

            if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.DeathMagician_DevilStatue) == false &&
                EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.DeathMagician_Catastrophe))
            {
                EventManager.Instance.Add_GuildQuest_Special((int)DialogueName.DeathMagician_DevilStatue, true);
                GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);

                if (GameManager.Facility.GetFacilityCount<Devil_Statue>() >= 5)
                {
                    EventManager.Instance.Add_GuildQuest_Special((int)DialogueName.DevilStatue_5, true);
                    GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);
                }
            }
        }
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = false;
        isClearable = false;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        return null;
    }


    Action<int> StatueAction;
    void AddEvent()
    {
        StatueAction = (value) => AddAction();
        AddTurnEvent(StatueAction, DayType.Day);
    }

    void RemoveEvent()
    {
        RemoveTurnEvent(StatueAction, DayType.Day);
    }

    void AddAction()
    {
        int danger = Random.Range(10, 21);

        Main.Instance.CurrentDay.AddDanger(danger);
        Main.Instance.ShowDM(danger, Main.TextType.danger, transform, 2);
    }





    void Create_Clone()
    {
        SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 1)));
        SetClone(CreateClone(PlacementInfo, new Vector2Int(0, -1)));
        SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 0)));
        SetClone(CreateClone(PlacementInfo, new Vector2Int(-1, 0)));

        SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 1)));
        SetClone(CreateClone(PlacementInfo, new Vector2Int(1, -1)));
        SetClone(CreateClone(PlacementInfo, new Vector2Int(-1, 1)));
        SetClone(CreateClone(PlacementInfo, new Vector2Int(-1, -1)));
    }


    IPlacementable CreateClone(PlacementInfo data, Vector2Int offset)
    {
        BasementTile newTile = null;
        data.Place_Floor.TileMap.TryGetValue(data.Place_Tile.index + offset, out newTile);
        PlacementInfo info = new PlacementInfo(data.Place_Floor, newTile);
        IPlacementable obj = GameManager.Facility.CreateFacility("Clone_Facility_Wall", info);
        return obj;
    }

    void SetClone(IPlacementable clone)
    {
        var cf = clone as Clone_Facility_Wall;
        cf.OriginalTarget = this;
    }


}
