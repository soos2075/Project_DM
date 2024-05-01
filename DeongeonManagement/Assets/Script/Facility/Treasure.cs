using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Treasure : Facility
{
    public override void Init_Personal()
    {
        int category = (CategoryIndex / 10) * 10;
        treasureType = (TreasureCategory)category;

        if (isInit)
        {
            CategorySelect(Data.spritePath);
        }
        else
        {
            First_Instantiate();
        }
    }

    public void First_Instantiate()
    {
        var SLA = GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset;
        var labels = SLA.GetCategoryLabelNames(treasureType.ToString());
        List<string> labelString = new List<string>();
        foreach (var item in labels)
        {
            labelString.Add(item);
        }

        int label = Random.Range(0, labelString.Count);
        string dataKeyName = labelString[label];
        Data = GameManager.Facility.GetData($"Treasure_{dataKeyName}");
        SetData();
        isInit = true;

        CategorySelect(dataKeyName);
    }


    public enum TreasureCategory
    {
        Swords = 2200,
        Rings = 2210,
        Hats = 2220,
        Scrolls = 2230,
        Artifacts = 2240,
        Crowns = 2250,
    }
    public TreasureCategory treasureType;

    void CategorySelect(string _dataKeyName)
    {
        var resolver = GetComponentInChildren<SpriteResolver>();
        //resolver.SetCategoryAndLabel(treasureType.ToString(), "Entry");
        resolver.SetCategoryAndLabel(treasureType.ToString(), _dataKeyName);
    }




    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(TreasureEvent(npc));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }

    protected IEnumerator TreasureEvent(NPC npc)
    {
        UI_EventBox.AddEventText($"●{npc.Name_Color} {UserData.Instance.GetLocaleText("Event_Treasure")}");
        PlacementState = PlacementState.Busy;

        bool isLastInteraction = false;
        if (InteractionOfTimes <= 0)
        {
            isLastInteraction = true;
        }

        yield return new WaitForSeconds(durationTime);

        int changeMP = mp_value;

        switch (GetTarget(npc))
        {
            case Target.Main:
                changeMP = mp_value;
                break;

            case Target.Sub:
                changeMP = (int)(mp_value * 0.7f);
                break;

            case Target.Weak:
                changeMP = (int)(mp_value * 0.3f);
                break;

            case Target.Invalid:
                changeMP = 0;
                break;

            case Target.Nothing:
                changeMP = (int)(mp_value * 0.5f);
                break;
        }


        int applyMana = Mathf.Clamp(changeMP, 0, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        if (applyMana > 0)
        {
            npc.Mana -= applyMana;
            Main.Instance.CurrentDay.AddMana(applyMana);
            var dm = Main.Instance.dm_small.Spawn(transform.position, $"+{applyMana} mana");
            dm.SetColor(Color.blue);
        }

        npc.ActionPoint -= ap_value;
        npc.HP -= hp_value;

        Main.Instance.CurrentDay.AddGold(gold_value);
        Main.Instance.CurrentDay.AddPop(pop_value);
        Main.Instance.CurrentDay.AddDanger(danger_value);

        OverCor(npc, isLastInteraction);
    }
}
