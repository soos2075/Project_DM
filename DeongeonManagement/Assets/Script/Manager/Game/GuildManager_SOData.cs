using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager_SOData
{
    public void Init()
    {
        Init_LocalData();
    }


    #region SO_Data

    SO_Guild_NPC[] so_data;
    Dictionary<int, SO_Guild_NPC> Guild_Dictionary { get; set; }
    public void Init_LocalData()
    {
        Guild_Dictionary = new Dictionary<int, SO_Guild_NPC>();
        so_data = Resources.LoadAll<SO_Guild_NPC>("Data/Guild");

        foreach (var item in so_data)
        {
            Guild_Dictionary.Add(item.Original_Index, item);
        }
    }




    public SO_Guild_NPC GetData(int _id)
    {
        SO_Guild_NPC npc = null;
        if (Guild_Dictionary.TryGetValue(_id, out npc))
        {
            return npc;
        }

        Debug.Log($"{_id}: Data Not Exist");
        return null;
    }


    public SO_Guild_NPC[] GetDataAll()
    {
        return so_data;
    }


    //public void Update_SO_GuildNPC()
    //{
    //    var currentData = EventManager.Instance.CurrentGuildData;
    //    if (currentData == null) return;


    //    SO_Guild_NPC tempData;
    //    foreach (var item in currentData)
    //    {
    //        if (Guild_Dictionary.TryGetValue(item.Original_Index, out tempData))
    //        {
    //            tempData.InstanceQuestList = item.InstanceQuestList;
    //            tempData.OptionList = item.OptionList;
    //        }
    //    }
    //}


    #endregion


}
