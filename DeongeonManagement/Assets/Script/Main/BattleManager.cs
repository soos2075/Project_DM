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



    public int BattleCount { get; set; } = 10;
    public List<BattleField> BattleList = new List<BattleField>();


    public Coroutine ShowBattleField(NPC _npc, Monster _monster, out BattleField.BattleResult result)
    {
        Vector3 bfPos = _monster.PlacementInfo.Place_Floor.transform.position;

        float direction = _monster.PlacementInfo.Place_Tile.worldPosition.x - bfPos.x;
        if (direction >= 0)
        {
            bfPos += new Vector3(Mathf.Clamp(Random.Range(3f, 8f) + direction, 5.0f, 8.0f) , Random.Range(-2.0f, 2.0f), 0);
        }
        else
        {
            bfPos += new Vector3(Mathf.Clamp(Random.Range(-3f, -8f) + direction, -5.0f, -8.0f), Random.Range(-2.0f, 2.0f), 0);
        }

        var bf = Managers.Resource.Instantiate("Battle/BattleField").GetComponent<BattleField>();
        bf.sort = BattleCount;
        BattleCount += 2;
        bf.transform.position = bfPos;

        var line = Managers.Resource.Instantiate("Battle/Line").GetComponent<LineRenderer>();
        line.SetPosition(0, _monster.transform.position);
        line.SetPosition(1, bfPos);

        bf.SetHPBar(_npc.HP, _npc.HP_MAX, _monster.HP, _monster.HP_Max);


        result = bf.Battle(_npc, _monster);
        BattleList.Add(bf);
        return StartCoroutine(Battle(bf, line));
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
        BattleList.Remove(_field);
    }
}
