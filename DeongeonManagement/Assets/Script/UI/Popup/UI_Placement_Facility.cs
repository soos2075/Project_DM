using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Facility : UI_PopUp
{

    enum Buttons
    {
        Return,

        Clear,
        Herb_Low,
        Herb_Middle,
        Herb_High,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => ClosePopUp());

        GetButton((int)Buttons.Herb_Low).gameObject.AddUIEvent(data => MultipleInstantiate(3, "Herb_Low"));
        GetButton((int)Buttons.Herb_Middle).gameObject.AddUIEvent(data => MultipleInstantiate(5, "Herb_Low"));
        GetButton((int)Buttons.Herb_High).gameObject.AddUIEvent(data => MultipleInstantiate(7, "Herb_High"));
    }

    void Start()
    {
        Init();
    }


    void MultipleInstantiate(int quantity, string prefab)
    {
        for (int i = 0; i < quantity; i++)
        {
            var obj = Managers.Resource.Instantiate($"Facility/{prefab}").GetComponent<Facility>();

            obj.Placement(Main.Instance.CurrentFloor);

            Main.Instance.CurrentFloor.facilityList.Add(obj);
        }
    }

}
