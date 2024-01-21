using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour, Interface.IPlacementable
{
    protected void Awake()
    {
        PlacementType = Define.PlacementType.Monster;
    }
    protected void Start()
    {
        Initialize_Status();

        SetSprite($"Sprite/Monster/{this.name}");
        monsterRenderer = GetComponentInChildren<SpriteRenderer>();
        Disable();

        hp_chance = hp_origin;
        atk_chance = atk_origin;
        def_chance = def_origin;
    }
    protected void Update()
    {
        
    }


    #region Placementable
    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }
    public Define.PlacementType PlacementType { get; set; }

    SpriteRenderer monsterRenderer;



    public void Placement(BasementFloor place)
    {
        PlacementConfirm(place, place.GetRandomTile(this));
        Debug.Log($"{name} 가 {Place_Floor} - {Place_Tile.index} 에 배치됨.");
    }

    protected void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        State = MonsterState.Placement;

        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
        Visible();
    }

    public void PlacementClear()
    {
        //Debug.Log($"{name} 가 {Place_Floor} - {Place_Tile.index}에서 비활성화");
        State = MonsterState.Standby;

        Place_Floor.Size++;
        Place_Tile.ClearPlacement();
        Place_Floor = null;
        Place_Tile = null;
        Disable();
    }
    protected void Visible()
    {
        monsterRenderer.enabled = true;
    }

    protected void Disable()
    {
        monsterRenderer.enabled = false;
    }
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

    protected void SetStatus(string name, int lv, int hp, int atk, int def)
    {
        Name = name; LV = lv; HP = hp; ATK = atk; DEF = def;
    }

    #endregion




    Coroutine Cor_Battle;

    public virtual Coroutine Battle(NPC npc)
    {
        Cor_Battle = StartCoroutine(SetBattleConfigure(npc));
        return Cor_Battle;
    }

    protected IEnumerator SetBattleConfigure(NPC npc)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            GiveAndTakeOnce(this, npc);

            if (this.HP <= 0)
            {
                Debug.Log("몬스터 패배");
                break;
            }

            if (npc.HP <= 0)
            {
                Debug.Log("NPC 패배");
                break;
            }
        }
    }

    private void GiveAndTakeOnce(Monster monster, NPC npc)
    {
        Debug.Log("한대씩 주고받기");
        monster.HP -= Mathf.Clamp((npc.ATK - monster.DEF), 1, monster.HP);

        npc.HP -= Mathf.Clamp((monster.ATK - npc.DEF), 1, npc.HP);
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



    public Coroutine moveCoroutine;





}


