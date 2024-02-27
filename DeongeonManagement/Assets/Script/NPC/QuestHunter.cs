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

        StartCoroutine(EventCor());
    }
    IEnumerator EventCor()
    {
        yield return new WaitForSeconds(5);

        Managers.Dialogue.ShowDialogueUI($"Quest_{Hunter.ToString()}", transform);
    }
    protected override void Start_Setting()
    {
        gameObject.name = $"Hunter_{Hunter.ToString()}";
    }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;

        switch (Hunter)
        {
            case HunterType.Slime:
                characterBuilder.Armor = "ThiefTunic";
                characterBuilder.Weapon = "Butcher";
                characterBuilder.Helmet = "HornsHelmet";
                characterBuilder.Back = "BackSword";
                break;

            case HunterType.EarthGolem:
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


    public enum HunterType
    {
        Slime = 1100,
        EarthGolem = 1101,
    }
    public HunterType Hunter { get { return (HunterType)EventID; } }


    void SetHunterType()
    {
        switch (Hunter)
        {
            case HunterType.Slime:
                AddList(GetPriorityPick(typeof(Slime)));
                break;

            case HunterType.EarthGolem:
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
    protected override void NPC_Runaway()
    {
        
    }

    protected override void NPC_Die()
    {
        Main.Instance.CurrentDay.AddKill(1);
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� ������");
        GameManager.NPC.InactiveNPC(this);

        //EventManager.Instance.RemoveQuestAction((int)Hunter);
        //switch (Hunter)
        //{
        //    case HunterType.Slime:
        //        //? ���� ��ȭ
        //        Debug.Log("������ ��~~ȭ ���� �׼� �����ϸ��");
        //        break;

        //    case HunterType.EarthGolem:
        //        //? ���� ��ȭ
        //        Debug.Log("���̷��� ��~~ȭ");
        //        break;
        //}
    }
    protected override void NPC_Captive()
    {

    }


}
