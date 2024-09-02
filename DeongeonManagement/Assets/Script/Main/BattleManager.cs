using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region singleton
    private static BattleManager _instance;
    public static BattleManager Instance { get { Init(); return _instance; } }

    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<BattleManager>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@BattleManager");
                _instance = go.AddComponent<BattleManager>();
                DontDestroyOnLoad(go);
            }
        }
    }
    #endregion


    void Start()
    {
        
    }


    public void TurnStart()
    {
        BattleCount = 10;
        BattleList.Clear();
    }

    public Material flash_Monster;
    public Material flash_NPC;



    public int BattleCount { get; set; } = 10;
    public List<BattleField> BattleList = new List<BattleField>();


    public Transform[] FloorBase;

    public Coroutine ShowBattleField(NPC _npc, Monster _monster, out BattleField.BattleResult result)
    {
        // 배틀필드 위치 중복되지 않게 정하는 부분
        //Vector3 bfPos = SetPosition(_monster);

        //int whileCount = 0;
        //while (FieldOverlapCheck(bfPos) == false && whileCount < 20)
        //{
        //    bfPos = SetPosition(_monster);
        //    whileCount++;
        //}


        int slotIndex = 0;
        Vector3 bfPos = SetPos_Field(_monster.PlacementInfo.Place_Floor.FloorIndex, out slotIndex);


        var bf = Managers.Resource.Instantiate("Battle/BattleField").GetComponent<BattleField>();
        bf.sort = BattleCount;
        BattleCount += 2;
        bf.transform.position = bfPos;
        bf.floorIndex = _monster.PlacementInfo.Place_Floor.FloorIndex;
        bf.slotIndex = slotIndex;

        var line = Managers.Resource.Instantiate("Battle/Line").GetComponent<LineRenderer>();
        line.SetPosition(0, _monster.transform.position);
        line.SetPosition(1, bfPos);

        SetFieldColor(bf, line, _monster, _npc);

        bf.SetHPBar(_npc.HP, _npc.HP_MAX, _monster.HP, _monster.HP_Max);


        result = bf.Battle(_npc, _monster, flash_NPC, flash_Monster);
        BattleList.Add(bf);
        return StartCoroutine(Battle(bf, line));
    }

    void SetFieldColor(BattleField field, LineRenderer line, Monster monster, NPC npc)
    {
        var floor = monster.PlacementInfo.Place_Floor.FloorIndex;
        //Debug.Log(floor + "@@");
        switch (floor)
        {
            case 0:
            case 3:
            case 6:
                field.sprite_BG.sprite = field.field_1;
                break;

            case 1:
            case 4:
            case 7:
                field.sprite_BG.sprite = field.field_2;
                break;

            case 2:
            case 5:
            case 8:
                field.sprite_BG.sprite = field.field_3;
                break;

            default:
                field.sprite_BG.sprite = field.field_1;
                break;
        }

        var npcType = npc.GetType();

        if (npcType == typeof(Herbalist) || npcType == typeof(Miner))
        {
            Color color = new Color32(243, 185, 211, 255);

            line.startColor = color;
            line.endColor = color;
            field.sprite_border.color = color;
        }
        else if (npcType == typeof(Adventurer) || npcType == typeof(Elf) || npcType == typeof(Wizard))
        {
            Color color = new Color32(0, 120, 60, 255);

            line.startColor = color;
            line.endColor = color;
            field.sprite_border.color = color;
        }
        else if (npcType == typeof(QuestHunter))
        {
            Color color = new Color32(0, 134, 209, 255);

            line.startColor = color;
            line.endColor = color;
            field.sprite_border.color = color;
        }
        else if (npcType == typeof(EventNPC))
        {
            Color color = new Color32(255, 242, 93, 255);

            line.startColor = color;
            line.endColor = color;
            field.sprite_border.color = color;
        }
        // 이거보다 더 위험한 타입은 red쓰면 될듯
    }



    IEnumerator Battle(BattleField _field, LineRenderer _line)
    {
        //Time.timeScale = 0;
        yield return _field.BattlePlay();
        RemoveCor(_field);
        Managers.Resource.Destroy(_field.gameObject);
        Managers.Resource.Destroy(_line.gameObject);
        //Time.timeScale = 1;
    }



    public void RemoveCor(BattleField _field)
    {
        AddPos_Field(_field.floorIndex, _field.slotIndex);
        BattleList.Remove(_field);
    }

    Vector3 SetPosition(Monster _monster)
    {
        Vector3 bfPos = _monster.PlacementInfo.Place_Floor.transform.position;

        float direction = _monster.PlacementInfo.Place_Tile.worldPosition.x - bfPos.x;
        if (direction >= 0)
        {
            bfPos += new Vector3(Mathf.Clamp(Random.Range(3f, 10f) + direction, 5.0f, 11.0f), Random.Range(-3f, 3f), 0);
        }
        else
        {
            bfPos += new Vector3(Mathf.Clamp(Random.Range(-3f, -10f) + direction, -11.0f, -5.0f), Random.Range(-3f, 3f), 0);
        }

        bfPos.x = Mathf.Clamp(bfPos.x, -13, 13);

        return bfPos;
    }

    bool FieldOverlapCheck(Vector3 _field)
    {
        Vector3[] vectors = new Vector3[BattleList.Count];

        for (int i = 0; i < vectors.Length; i++)
        {
            vectors[i] = BattleList[i].transform.position;
        }

        foreach (var item in vectors)
        {
            float minX = item.x - 2.75f;
            float maxX = item.x + 2.75f;

            float minY = item.y - 1.5f;
            float maxY = item.y + 1.5f;

            if (minX < _field.x && _field.x < maxX)
            {
                if (minY < _field.y && _field.y < maxY)
                {
                    // 위치가 겹침(x,y 가 1by1안에 안에 들어옴)
                    //Debug.Log(_field + "@@@겹침");

                    return false;
                }
            }
        }
        return true;
    }


    Vector3 SetPos_Field(int floorIndex, out int slotNumber)
    {
        bool[] checkList = floor_egg;
        Vector3[] posList = Floor_Egg_Battle_Pos;

        switch (floorIndex)
        {
            case 0:
                checkList = floor_egg;
                posList = Floor_Egg_Battle_Pos;
                break;

            case 1:
                checkList = floor_1;
                posList = Floor_1_Battle_Pos;
                break;

            case 2:
                checkList = floor_2;
                posList = Floor_2_Battle_Pos;
                break;

            case 3:
                checkList = floor_3;
                posList = Floor_3_Battle_Pos;
                break;
        }


        int ranPick = GetRandomField(checkList);
        slotNumber = ranPick;
        checkList[ranPick] = true;
        return posList[ranPick] + FloorBase[floorIndex].position;

        //for (int i = 0; i < checkList.Length; i++)
        //{
        //    if (!checkList[i])
        //    {
        //        checkList[i] = true;
        //        slotNumber = i;
        //        return posList[i] + FloorBase[floorIndex].position;
        //    }
        //}
        //slotNumber = -1;
        //return Vector3.zero + FloorBase[floorIndex].position;
    }


    int GetRandomField(bool[] target)
    {
        int ranValue = Random.Range(0, target.Length);
        int counter = 0;
        while (counter < 100)
        {
            if (!target[ranValue])
            {
                return ranValue;
            }
            counter++;
        }
        return 0;
    }


    void AddPos_Field(int floorIndex, int slotIndex)
    {
        switch (floorIndex)
        {
            case 0:
                floor_egg[slotIndex] = false;
                break;

            case 1:
                floor_1[slotIndex] = false;
                break;

            case 2:
                floor_2[slotIndex] = false;
                break;

            case 3:
                floor_3[slotIndex] = false;
                break;

            case 4:
                floor_4[slotIndex] = false;
                break;
        }
    }




    bool[] floor_egg = new bool[Floor_Egg_Battle_Pos.Length];
    bool[] floor_1 = new bool[Floor_1_Battle_Pos.Length];
    bool[] floor_2 = new bool[Floor_2_Battle_Pos.Length];
    bool[] floor_3 = new bool[Floor_3_Battle_Pos.Length];
    bool[] floor_4 = new bool[Floor_4_Battle_Pos.Length];


    static readonly Vector3[] Floor_Egg_Battle_Pos = new Vector3[]
    {
        new Vector3(6, 1.5f, 0), new Vector3(1, 1.5f, 0), new Vector3(-4, 1.5f, 0), new Vector3(-9, 1.5f, 0),
        new Vector3(-11, -1.5f, 0), new Vector3(-11, -4.5f, 0),new Vector3(-11, -7.5f, 0),
        new Vector3(6, -10.5f, 0), new Vector3(1, -10.5f, 0), new Vector3(-4, -10.5f, 0), new Vector3(-9, -10.5f, 0)
    };

    static readonly Vector3[] Floor_1_Battle_Pos = new Vector3[]
    {
        new Vector3(-7, 7, 0), new Vector3(-2, 7, 0), new Vector3(3, 7, 0), new Vector3(8, 7, 0),
        new Vector3(-7, -7, 0), new Vector3(-2, -7, 0), new Vector3(3, -7, 0), new Vector3(8, -7, 0)
    };

    static readonly Vector3[] Floor_2_Battle_Pos = new Vector3[]
    {
        new Vector3(12, 9.5f, 0), new Vector3(7, 9.5f, 0), new Vector3(2, 9.5f, 0), 
        new Vector3(-3, 9.5f, 0), new Vector3(-8, 9.5f, 0), new Vector3(-13, 9.5f, 0),
        new Vector3(12, -9.5f, 0), new Vector3(7, -9.5f, 0), new Vector3(2, -9.5f, 0),
        new Vector3(-3, -9.5f, 0), new Vector3(-8, -9.5f, 0), new Vector3(-13, -9.5f, 0)
    };

    static readonly Vector3[] Floor_3_Battle_Pos = new Vector3[]
    {
        new Vector3(15, 7, 0), new Vector3(15, 4, 0), new Vector3(15, 1, 0),
        new Vector3(15, -2, 0), new Vector3(15, -5, 0), new Vector3(15, -8, 0),
        new Vector3(10, -9.5f, 0), new Vector3(5, -9.5f, 0), new Vector3(0, -9.5f, 0), new Vector3(-5, -9.5f, 0)
    };

    static readonly Vector3[] Floor_4_Battle_Pos = new Vector3[]
    {
        new Vector3(-14, 9, 0), new Vector3(-14, 6, 0),new Vector3(-14, 3, 0),
        new Vector3(-14, 0, 0),new Vector3(-14, -3, 0),new Vector3(-14, -6, 0),
        new Vector3(14, 9, 0), new Vector3(14, 6, 0),new Vector3(14, 3, 0),
        new Vector3(14, 0, 0),new Vector3(14, -3, 0),new Vector3(14, -6, 0)
    };

}
