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
        if (Data == null) { Debug.Log($"���̺굥���� ���� : {name}"); return; }

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
        if (Data == null) { Debug.Log($"������ ���� : {name}"); return; }

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
            Debug.Log($"{Name} �� ��Ʋ �Ұ���");
            return null;
        }
    }

    IEnumerator BattleWait(NPC npc)
    {
        npc.ActionPoint -= 2;

        UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� �����߻� : " +
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
                UI_EventBox.AddEventText($"��{Name_KR} vs {npc.Name_KR} �� ������ ����Ǿ����ϴ�.");
                break;

            case BattleField.BattleResult.Monster_Die:
                UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� �������� �й��߽��ϴ�..");
                MonsterOutFloor();
                break;

            case BattleField.BattleResult.NPC_Die:
                UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� {npc.Name_KR} {npc.Name_Index} ���� �¸��Ͽ����ϴ�!");
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
        UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� �����߻� : " +
            $"{npc.Name_KR} vs " +
            $"{Name_KR}");
        while (true)
        {
            yield return new WaitForSeconds(1f);
            GiveAndTakeOnce(this, npc);

            if (this.HP <= 0)
            {
                this.HP = 0;
                UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� �������� �й��߽��ϴ�..");
                Debug.Log("���� �й�");
                MonsterOutFloor();
                break;
            }

            if (npc.HP <= 0)
            {
                UI_EventBox.AddEventText($"��{PlacementInfo.Place_Floor.Name_KR}���� {Name_KR} (��)�� {npc.Name_KR} {npc.Name_Index} ���� �¸��Ͽ����ϴ�!");
                Debug.Log("NPC �й�");
                break;
            }
        }
    }

    private void GiveAndTakeOnce(Monster monster, NPC npc)
    {
        monster.HP -= Mathf.Clamp((npc.ATK - monster.DEF), 1, monster.HP);

        npc.HP -= Mathf.Clamp((monster.ATK - npc.DEF), 1, npc.HP);
        //Debug.Log($"��Ʋ �� : {monster.name}�� ���� ü�� : {monster.HP} / {npc.name}�� ���� ü�� : {npc.HP}");
    }







    public void Recover(int mana)
    {
        //? ȸ��
        if (Main.Instance.Player_Mana >= mana)
        {
            Main.Instance.CurrentDay.SubtractMana(mana);
            HP = HP_Max;
            State = MonsterState.Standby;
            Debug.Log("ȸ������");
        }
        else
        {
            Debug.Log("��������");
        }
    }

    public void Training()
    {
        if (Main.Instance.Player_AP <= 0)
        {
            Debug.Log("�Ʒ�Ƚ�� ����");
            return;
        }
        if (Data.MAXLV <= LV)
        {
            Debug.Log("�ִ뷹����");
            return;
        }

        Main.Instance.Player_AP--;
        Debug.Log($"{Name_KR} �Ʒ�����");
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

        //? 1���� ũ�� �ϴ� Ȯ�������� �÷��ְ�
        while (probability >= 1)
        {
            value++;
            probability--;
        }

        //? 1���� �������� Ȯ������
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


