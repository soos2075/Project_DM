using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IPlacementable
{
    protected void Awake()
    {

    }
    protected void Start()
    {
        //MonsterInit();
        //Initialize_Status();
    }


    #region IPlacementable
    public Define.PlacementType PlacementType { get; set; }
    public PlacementInfo PlacementInfo { get; set; }
    public GameObject GetObject()
    {
        return this.gameObject;
    }
    public string Name_KR { get { return $"{name_Tag_Start}{Name}{name_Tag_End}"; } }
    private string name_Tag_Start = "<color=#44ff44ff>";
    private string name_Tag_End = "</color>";
    #endregion



    #region SaveLoad

    public void Initialize_SaveData(Save_MonsterData Data)
    {
        if (Data == null) { Debug.Log($"세이브데이터 없음 : {name}"); return; }

        LV = Data.LV;
        HP = Data.HP;
        HP_Max = Data.HP_MAX;

        ATK = Data.ATK;
        DEF = Data.DEF;
        AGI = Data.AGI;
        LUK = Data.LUK;

        hp_chance = Data.HP_chance;
        atk_chance = Data.ATK_chance;
        def_chance = Data.DEF_chance;
        agi_chance = Data.AGI_chance;
        luk_chance = Data.LUK_chance;

        State = Data.State;
    }

    #endregion



    #region For Management
    public enum MonsterState
    {
        Standby,
        Placement,
        Injury,
    }
    public MonsterState State { get; set; }


    #endregion




    public abstract MonsterData Data { get; set; }
    public int MonsterID { get; set; }



    #region Monster Status
    public string Name { get; protected set; }
    public int LV { get; protected set; }
    public int HP { get; set; }
    public int HP_Max { get; protected set; }

    public int ATK { get; protected set; }
    public int DEF { get; protected set; }
    public int AGI { get; protected set; }
    public int LUK { get; protected set; }

    public float hp_chance;
    public float atk_chance;
    public float def_chance;
    public float agi_chance;
    public float luk_chance;

    public enum MonsterType
    {
        Normal_Move,
        Normal_Fixed,
        Boss,
    }

    public abstract MonsterType Type { get; set; }


    public abstract void MonsterInit();
    public void Initialize_Status()
    {
        if (Data == null) { Debug.Log($"데이터 없음 : {name}"); return; }

        Name = Data.Name_KR;
        LV = Data.LV;

        HP = Data.HP;
        HP_Max = Data.HP;

        ATK = Data.ATK;
        DEF = Data.DEF;
        AGI = Data.AGI;
        LUK = Data.LUK;

        hp_chance = Data.HP_chance;
        atk_chance = Data.ATK_chance;
        def_chance = Data.DEF_chance;
        agi_chance = Data.AGI_chance;
        luk_chance = Data.LUK_chance;
    }


    #endregion




    Coroutine Cor_Battle;

    public virtual Coroutine Battle(NPC npc)
    {
        if (this.HP > 0)
        {
            Cor_Battle = StartCoroutine(BattleWait(npc));
            return Cor_Battle;
        }
        else
        {
            Debug.Log($"{Name} 가 배틀 불가능");
            return null;
        }
    }

    IEnumerator BattleWait(NPC npc)
    {
        npc.ActionPoint -= 2;

        UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 전투발생 : " +
            $"{npc.Name_KR} vs " +
            $"{Name_KR}");

        var bf = Managers.Resource.Instantiate("Battle/BattleField").GetComponent<BattleField>();
        bf.transform.position = npc.transform.position + new Vector3(Random.value, Random.value, 0);
        var result = bf.Battle(npc, this);

        Time.timeScale = 0;
        yield return bf.BattlePlay();
        Managers.Resource.Destroy(bf.gameObject);
        Time.timeScale = 1;

        switch (result)
        {
            case BattleField.BattleResult.Nothing:
                UI_EventBox.AddEventText($"★{Name_KR} vs {npc.Name_KR} 의 전투가 종료되었습니다.");
                break;

            case BattleField.BattleResult.Monster_Die:
                UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 {Name_KR} (이)가 전투에서 패배했습니다..");
                MonsterOutFloor();
                break;

            case BattleField.BattleResult.NPC_Die:
                UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 {Name_KR} (이)가 {npc.Name_KR} {npc.Name_Index} 에게 승리하였습니다!");
                break;
        }

    }

    public void MonsterOutFloor()
    {
        PlacementInfo.Place_Floor.MaxMonsterSize++;
        State = HP <= 0 ? MonsterState.Injury : MonsterState.Standby;
        GameManager.Placement.PlacementClear(this);
    }





    protected IEnumerator SetBattleConfigure(NPC npc)
    {
        UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 전투발생 : " +
            $"{npc.Name_KR} vs " +
            $"{Name_KR}");
        while (true)
        {
            yield return new WaitForSeconds(1f);
            GiveAndTakeOnce(this, npc);

            if (this.HP <= 0)
            {
                this.HP = 0;
                UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 {Name_KR} (이)가 전투에서 패배했습니다..");
                Debug.Log("몬스터 패배");
                MonsterOutFloor();
                break;
            }

            if (npc.HP <= 0)
            {
                UI_EventBox.AddEventText($"★{PlacementInfo.Place_Floor.Name_KR}에서 {Name_KR} (이)가 {npc.Name_KR} {npc.Name_Index} 에게 승리하였습니다!");
                Debug.Log("NPC 패배");
                break;
            }
        }
    }

    private void GiveAndTakeOnce(Monster monster, NPC npc)
    {
        monster.HP -= Mathf.Clamp((npc.ATK - monster.DEF), 1, monster.HP);

        npc.HP -= Mathf.Clamp((monster.ATK - npc.DEF), 1, npc.HP);
        //Debug.Log($"배틀 상세 : {monster.name}의 남은 체력 : {monster.HP} / {npc.name}의 남은 체력 : {npc.HP}");
    }







    public void Recover(int mana)
    {
        //? 회복
        if (Main.Instance.Player_Mana >= mana)
        {
            Main.Instance.CurrentDay.SubtractMana(mana);
            HP = HP_Max;
            State = MonsterState.Standby;
            Debug.Log("회복성공");
        }
        else
        {
            Debug.Log("마나부족");
        }
    }

    public void Training()
    {
        if (Main.Instance.Player_AP <= 0)
        {
            Debug.Log("훈련횟수 없음");
            return;
        }
        if (Data.MAXLV <= LV)
        {
            Debug.Log("최대레벨임");
            return;
        }

        Main.Instance.Player_AP--;
        Debug.Log($"{Name_KR} 훈련진행");
        LevelUp();
    }

    public void LevelUp()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_StatusUp>();
        ui.TargetMonster(this);

        LV++;

        HP_Max += TryStatUp(Data.HP_chance, ref hp_chance);
        HP = HP_Max;

        ATK += TryStatUp(Data.ATK_chance, ref atk_chance);
        DEF += TryStatUp(Data.DEF_chance, ref def_chance);
        AGI += TryStatUp(Data.AGI_chance, ref agi_chance);
        LUK += TryStatUp(Data.LUK_chance, ref luk_chance);
    }
    public void StatUp()
    {
        HP_Max += TryStatUp(Data.HP_chance, ref hp_chance);
        ATK += TryStatUp(Data.ATK_chance, ref atk_chance);
        DEF += TryStatUp(Data.DEF_chance, ref def_chance);
        AGI += TryStatUp(Data.AGI_chance, ref agi_chance);
        LUK += TryStatUp(Data.LUK_chance, ref luk_chance);
    }

    int TryStatUp(float origin, ref float probability)
    {
        int value = 0;

        //? 1보다 크면 일단 확정적으로 올려주고
        while (probability >= 1)
        {
            value++;
            probability--;
        }

        //? 1보다 작은값은 확률굴림
        if (probability > Random.value)
        {
            value++;
            probability = origin;
        }
        else
        {
            probability += origin;
        }

        return value;
    }




}


