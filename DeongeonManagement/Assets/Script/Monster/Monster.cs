using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Monster : MonoBehaviour, Interface.IPlacementable
{
    protected void Awake()
    {
        PlacementType = Define.PlacementType.Monster;
    }
    protected void Start()
    {
        Initialize();

        SetSprite($"Sprite/Monster/{this.name}");
        monsterSprite = GetComponentInChildren<SpriteRenderer>();
        monsterSprite.enabled = false;

        hp_chance = hp_origin;
        atk_chance = atk_origin;
        def_chance = def_origin;
    }
    protected void Update()
    {
        
    }


    #region Monster Status Property

    public Sprite Sprite { get; set; }
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

    #endregion

    public enum MonsterState
    {
        Standby,
        Placement,
        Injury,
    }
    public MonsterState State { get; set; }
    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }
    public Define.PlacementType PlacementType { get; set; }

    public bool isTraining;

    SpriteRenderer monsterSprite;


    protected virtual void Initialize()
    {
        SetStatus("Slime", 10, 1);
    }

    protected void SetStatus(string _name, int _hp, int _lv)
    {
        Name = _name;
        HP = _hp;
        LV = _lv;
    }

    protected void SetSprite(string _path)
    {
        Sprite = Managers.Resource.Load<Sprite>(_path);
    }





    public void Summon()
    {

    }

    private void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        State = MonsterState.Placement;

        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
        monsterSprite.enabled = true;
    }


    public void Placement(BasementFloor place)
    {
        //Debug.Log($"{name} 가 {place} 에 배치됨.");
        PlacementConfirm(place, place.GetRandomTile(this));

        moveCoroutine = StartCoroutine(MoveCor());
    }


    public void PlacementClear()
    {
        //Debug.Log($"{name} 가 대기상태로 들어감.");
        State = MonsterState.Standby;

        Place_Floor.Size++;
        Place_Tile.ClearPlacement();
        Place_Floor = null;
        Place_Tile = null;
        monsterSprite.enabled = false;

        StopCoroutine(moveCoroutine);
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







    #region MonsterMove

    Coroutine moveCoroutine;

    IEnumerator MoveCor()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            MoveAround();
        }
    }

    void MoveAround()
    {
        BasementTile newTile;

        int dir = Random.Range(0, 5);
        switch (dir)
        {
            case 0:
                newTile = Place_Floor.MoveUp(this, Place_Tile);
                break;

            case 1:
                newTile = Place_Floor.MoveDown(this, Place_Tile);
                break;

            case 2:
                newTile = Place_Floor.MoveLeft(this, Place_Tile);
                break;

            case 3:
                newTile = Place_Floor.MoveRight(this, Place_Tile);
                break;

            default:
                newTile = null;
                break;
        }

        if (newTile != null)
        {
            Place_Tile.ClearPlacement();
            PlacementConfirm(Place_Floor, newTile);
        }
    }


    #endregion


}


