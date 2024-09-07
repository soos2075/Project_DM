using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Unique : NPC
{
    NPC_Type_Unique NPCType { get { return (NPC_Type_Unique)EventID; } }

    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }

    protected override void Start_Setting()
    {
        //RunawayHpRatio = 1000;
        //KillGold = 0;
    }


    protected override Define.TileType[] AvoidTileType { get { return AvoidTile(); } }

    Define.TileType[] AvoidTile() //? ������ ���� ó�� ��ã�� �����̰�, ���� ��ã���� �ٽ� ã�� �� �̰� ���� ã���ϱ� ����
    {
        switch (State)
        {
            case NPCState.Runaway:
            case NPCState.Return_Satisfaction:
                return new Define.TileType[] { Define.TileType.Facility };

            default:
                switch (NPCType)
                {

                    default:
                        return new Define.TileType[] { Define.TileType.Facility };
                }
        }
    }

    protected override void SetRandomClothes() //? ����
    {
        var collection = characterBuilder.SpriteCollection;

        switch (NPCType)
        {
            case NPC_Type_Unique.ManaGoblin:
                characterBuilder.Head = "Merman#FFFFFF/0:0:0";
                characterBuilder.Body = "Merman#FFFFFF/0:0:0";
                characterBuilder.Ears = "Merman#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Merman#FFFFFF/0:0:0";
                characterBuilder.Weapon = "CurveBranch#FFFFFF/0:0:0";
                break;

            case NPC_Type_Unique.GoldLizard:
                characterBuilder.Head = "FireLizard#FFFFFF/0:0:0";
                characterBuilder.Body = "FireLizard#FFFFFF/0:0:0";
                characterBuilder.Ears = "FireLizard#FFFFFF/0:0:0";
                characterBuilder.Eyes = "FireLizard#FFFFFF/0:0:0";
                characterBuilder.Weapon = "BattleHammer#FFFFFF/0:0:0";
                break;

            case NPC_Type_Unique.PumpkinHead:
                characterBuilder.Armor = "SantaHelperTunic#FFFFFF/0:0:0";
                characterBuilder.Helmet = "PumpkinHead#FFFFFF/0:0:0";
                characterBuilder.Weapon = "Axe#FFFFFF/0:0:0";
                break;

            case NPC_Type_Unique.Santa:
                characterBuilder.Armor = "SantaTunic#FFFFFF/0:0:0";
                characterBuilder.Helmet = "SantaHoodBeard#FFFFFF/0:0:0";
                characterBuilder.Weapon = "AmurWand#FFFFFF/0:0:0";
                characterBuilder.Back = "SmallBackpack#FFFFFF/0:0:0";
                break;

            case NPC_Type_Unique.DungeonThief:
                characterBuilder.Armor = "SantaHelperTunic#FFFFFF/0:0:0";
                characterBuilder.Helmet = "SantaHelperCap [ShowEars]#FFFFFF/0:0:0";
                characterBuilder.Weapon = "Stick#FFFFFF/0:0:0";
                characterBuilder.Back = "LargeBackpack#FFFFFF/0:0:0";
                break;

                //case NPC_Type_Hunter.Hunter_Slime:
                //    characterBuilder.Armor = "ThiefTunic";
                //    characterBuilder.Weapon = "Butcher";
                //    characterBuilder.Helmet = "HornsHelmet";
                //    characterBuilder.Back = "BackSword";
                //    break;
        }


        characterBuilder.Rebuild();
    }

    public override void Departure(Vector3 startPoint, Vector3 endPoint) //? ������ �������� ��
    {
        base.Departure(startPoint, endPoint);

        switch (NPCType)
        {
            //case NPC_Type_Hunter.Hunter_Slime:
            //    if (UserData.Instance.FileConfig.firstAppear_Hunter_Slime == false)
            //    {
            //        UserData.Instance.FileConfig.firstAppear_Hunter_Slime = true;
            //        StartCoroutine(EventCor($"{NPCType.ToString()}"));
            //    }
            //    break;
        }
    }



    protected override void SetPriorityList(PrioritySortOption option)
    {
        if (PriorityList != null) PriorityList.Clear();

        List<BasementTile> main = null; 
        List<BasementTile> sub1 = null;
        List<BasementTile> sub2 = null;

        switch (NPCType)
        {
            //case NPC_Type_Hunter.Hunter_Slime:
            //    main = GetPriorityPick(typeof(Slime));
            //    break;
        }

        //? �����̶� ����� ������ ����
        switch (option)
        {
            case PrioritySortOption.Random:
                break;
            case PrioritySortOption.SortByDistance:
                SortByDistance(main);
                SortByDistance(sub1);
                SortByDistance(sub2);
                break;
        }

        AddList(main);
        AddList(sub1);
        AddList(sub2);

        {//? �칰 �� ���谡 ���� �̺�Ʈ
            Add_Wells();
        }
        {//? ���׼�ġ
            var add_egg = GetPriorityPick(typeof(SpecialEgg));
            AddList(add_egg);
        }
        {//? ������ ��ġ
            PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.04f);
        }
    }



    protected override void NPC_Return_Empty()
    {

    }
    protected override void NPC_Return_Satisfaction()
    {

    }
    protected override void NPC_Return_NonSatisfaction()
    {

    }
    protected override void NPC_Runaway()
    {

    }
    protected override void NPC_Die()
    {
        UI_EventBox.AddEventText($"��{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
        GameManager.NPC.InactiveNPC(this);
        //AddCollectionPoint();
    }


}
