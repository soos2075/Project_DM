using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Summon : UI_PopUp
{

    enum Summon
    {
        Slime,
        Goblin,
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Summon));
    }

    void Start()
    {
        Init();

        GetButton((int)Summon.Slime).gameObject.AddUIEvent((data) => MonsterSummon("Monster/Slime"));
        GetButton((int)Summon.Goblin).gameObject.AddUIEvent((data) => MonsterSummon("Monster/Goblin"));
    }

    void Update()
    {
        
    }


    public void MonsterSummon(string _name)
    {
        if (Main.Instance.MaximumCheck())
        {
            var monster = Managers.Resource.Instantiate(_name, Main.Instance.transform);
            Main.Instance.AddMonster(monster.GetOrAddComponent<Monster>());
        }
    }
}
