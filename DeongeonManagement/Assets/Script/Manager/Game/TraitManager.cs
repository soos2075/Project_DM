using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager
{
    public void Init()
    {
        Init_LocalData();
    }

    #region SO_Data
    SO_Trait[] so_data;
    Dictionary<TraitGroup, SO_Trait> Trait_Dictionary { get; set; }

    public void Init_LocalData()
    {
        Trait_Dictionary = new Dictionary<TraitGroup, SO_Trait>();
        so_data = Resources.LoadAll<SO_Trait>("Data/Trait");

        foreach (var item in so_data)
        {
            //(string, string) datas = ("", "");
            string[] datas = Managers.Data.GetTextData_Trait(item.id);

            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];
            item.Acquire = datas[2];

            Trait_Dictionary.Add(item.trait, item);
        }
    }


    public SO_Trait GetData(TraitGroup _keyName)
    {
        SO_Trait trait = null;
        if (Trait_Dictionary.TryGetValue(_keyName, out trait))
        {
            return trait;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }



    public void CreateTraitBar(TraitGroup _keyName, Transform parents)
    {
        var trait = Managers.Resource.Instantiate("UI/PopUp/Element/TraitBar", parents);
        var Traitbar = trait.GetComponent<UI_TraitBar>();
        Traitbar.Init_TraitBar(GetData(_keyName));
    }


    #endregion


}
