using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHunter : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; } = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };
    public override int RunawayHpRatio { get; set; } = 999;

    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);

        switch (Hunter)
        {
            case EventNPCType.Hunter_Slime:
                if (UserData.Instance.FileConfig.firstAppear_Hunter_Slime == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Hunter_Slime = true;
                    StartCoroutine(EventCor($"Hunter_{Hunter.ToString()}"));
                }
                break;

            case EventNPCType.Hunter_EarthGolem:
                if (UserData.Instance.FileConfig.firstAppear_Hunter_EarthGolem == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Hunter_EarthGolem = true;
                    StartCoroutine(EventCor($"Hunter_{Hunter.ToString()}"));
                }
                break;
        }
    }

    //IEnumerator EventCor(string dialogueName)
    //{
    //    yield return new WaitUntil(() => Vector3.Distance(transform.position, Main.Instance.Dungeon.position) < 2);

    //    Managers.Dialogue.ShowDialogueUI(dialogueName, transform);
    //}

    protected override void Start_Setting()
    {
        gameObject.name = $"Hunter_{Hunter.ToString()}";
    }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;

        switch (Hunter)
        {
            case EventNPCType.Hunter_Slime:
                characterBuilder.Armor = "ThiefTunic";
                characterBuilder.Weapon = "Butcher";
                characterBuilder.Helmet = "HornsHelmet";
                characterBuilder.Back = "BackSword";
                break;

            case EventNPCType.Hunter_EarthGolem:
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

    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        SetHunterType();
    }


    //public enum HunterType
    //{
    //    Slime = 1100,
    //    EarthGolem = 1101,
    //}
    public EventNPCType Hunter { get { return (EventNPCType)EventID; } }


    void SetHunterType()
    {
        switch (Hunter)
        {
            case EventNPCType.Hunter_Slime:
                AddList(GetPriorityPick(typeof(Slime)));
                break;

            case EventNPCType.Hunter_EarthGolem:
                AddList(GetPriorityPick(typeof(EarthGolem)));
                break;
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
        UI_EventBox.AddEventText($"¢Â{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
        GameManager.NPC.InactiveNPC(this);

        AddCollectionPoint();
    }
    protected override void NPC_Captive()
    {

    }


}
