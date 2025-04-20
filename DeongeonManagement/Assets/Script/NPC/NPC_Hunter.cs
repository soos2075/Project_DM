using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Hunter : NPC
{
    NPC_Type_Hunter NPCType { get { return (NPC_Type_Hunter)EventID; } }

    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }



    protected override void Start_Setting()
    {
        RunawayHpRatio = 1000;
        KillGold = 0;
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
                    case NPC_Type_Hunter.Hunter_Slime:
                    case NPC_Type_Hunter.Hunter_EarthGolem:
                        return new Define.TileType[] { Define.TileType.Facility };

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
            case NPC_Type_Hunter.Hunter_Slime:
                characterBuilder.Armor = "ThiefTunic";
                characterBuilder.Weapon = "Butcher";
                characterBuilder.Helmet = "HornsHelmet";
                characterBuilder.Back = "BackSword";
                break;

            case NPC_Type_Hunter.Hunter_EarthGolem:
                characterBuilder.Hair = "Hair12#891E2B/0:0:0";
                characterBuilder.Armor = "ThiefTunic";
                characterBuilder.Weapon = "Dagger";
                //characterBuilder.Helmet = "VikingHelmet";
                //characterBuilder.Back = "BackSword";
                characterBuilder.Mask = "BanditMask";
                break;
        }


        characterBuilder.Rebuild();
    }

    public override void Departure(Vector3 startPoint, Vector3 endPoint) //? ������ �������� ��
    {
        base.Departure(startPoint, endPoint);

        switch (NPCType)
        {
            case NPC_Type_Hunter.Hunter_Slime:
                if (UserData.Instance.FileConfig.firstAppear_Hunter_Slime == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Hunter_Slime = true;
                    StartCoroutine(EventCor(DialogueName.Hunter_Slime));
                }
                break;

            case NPC_Type_Hunter.Hunter_EarthGolem:
                if (UserData.Instance.FileConfig.firstAppear_Hunter_EarthGolem == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Hunter_EarthGolem = true;
                    StartCoroutine(EventCor(DialogueName.Hunter_EarthGolem));
                }
                break;
        }
    }



    protected override void SetPriorityList(PrioritySortOption option)
    {
        if (PriorityList != null) PriorityList.Clear();

        List<BasementTile> main = null;

        switch (NPCType)
        {
            case NPC_Type_Hunter.Hunter_Slime:
                main = GetPriorityPick(typeof(Slime));
                break;

            case NPC_Type_Hunter.Hunter_EarthGolem:
                main = GetPriorityPick(typeof(EarthGolem));
                break;
        }

        main.Sort(new ValueComparer(ValueComparer.ValueOption.MonsterLv, false));
        AddList(main);
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
