using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IPlacementable
{
    protected void Awake()
    {
        
    }
    protected void Start()
    {
        Initialize_Status();

        SetSprite($"Sprite/Monster/{this.name}");

        hp_chance = hp_origin;
        atk_chance = atk_origin;
        def_chance = def_origin;
    }
    //protected void Update()
    //{
        
    //}


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





    #region For GUI Traing Etc
    public enum MonsterState
    {
        Standby,
        Placement,
        Injury,
    }
    public MonsterState State { get; set; }
    public Sprite Sprite { get; set; }

    public bool isTraining;

    protected void SetSprite(string _path)
    {
        Sprite = Managers.Resource.Load<Sprite>(_path);
    }
    #endregion








    #region Monster Status
    public string Name { get; set; }
    public int LV { get; set; }
    public int HP { get; set; }

    public int ATK { get; set; }
    public int DEF { get; set; }
    public int AGI { get; set; }
    public int LUK { get; set; }

    private readonly float hp_origin = 1.2f;
    private float hp_chance;

    private readonly float atk_origin = 0.5f;
    private float atk_chance;

    private readonly float def_origin = 0.25f;
    private float def_chance;

    public enum MonsterType
    {
        Normal_Move,
        Normal_Fixed,
        Boss,
    }

    public abstract MonsterType Type { get; set; }


    protected abstract void MonsterInit();
    protected abstract void Initialize_Status();

    protected void SetStatus(string name, int lv, int hp, int atk, int def, int agi, int luk)
    {
        Name = name; LV = lv;

        HP = hp;

        ATK = atk; DEF = def;
        AGI = agi; LUK = luk;
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
        Managers.Placement.PlacementClear(this);
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









    public void Training()
    {
        LevelUp();
        isTraining = true;
    }

    public void LevelUp()
    {
        LV++;

        HP += TryStatUp(hp_origin, ref hp_chance);
        ATK += TryStatUp(atk_origin, ref atk_chance);
        DEF += TryStatUp(def_origin, ref def_chance);
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



    public Coroutine moveCoroutine;





}


