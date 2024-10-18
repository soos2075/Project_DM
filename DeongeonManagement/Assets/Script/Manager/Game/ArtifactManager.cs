using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager
{
    public void Init()
    {
        Init_LocalData();
        //Init_SLA();
        //Init_MonsterSlot();

        Init_ArtifactArray();
    }

    #region SO_Data
    SO_Artifact[] so_data;

    public void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_Artifact>("Data/Artifact");
        //foreach (var item in so_data)
        //{
        //    string[] datas = Managers.Data.GetTextData_Object(item.id);

        //    if (datas == null)
        //    {
        //        Debug.Log($"{item.id} : CSV Data Not Exist");
        //        continue;
        //    }

        //    item.labelName = datas[0];
        //    item.detail = datas[1];


        //    if (datas[2].Contains("@Op1::"))
        //    {
        //        string op1 = datas[2].Substring(datas[2].IndexOf("@Op1::") + 6, datas[2].IndexOf("::Op1") - (datas[2].IndexOf("@Op1::") + 6));
        //        item.evolutionHint = op1;
        //    }

        //    if (datas[2].Contains("@Op2::"))
        //    {
        //        string op2 = datas[2].Substring(datas[2].IndexOf("@Op2::") + 6, datas[2].IndexOf("::Op2") - (datas[2].IndexOf("@Op2::") + 6));
        //        item.evolutionDetail = op2;
        //    }
        //}
    }

    public SO_Artifact GetData(string _keyName)
    {
        foreach (var item in so_data)
        {
            if (item.keyName == _keyName)
            {
                return item;
            }
        }
        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }
    #endregion


    #region Save Load

    public List<Artifact> Save_ArtifactData()
    {
        List<Artifact> saveData = new List<Artifact>();
        foreach (var item in artifacts)
        {
            saveData.Add(item.DeepCopy());
        }

        return saveData;
    }
    public void Load_ArtifactData(List<Artifact> data)
    {
        if (data != null)
        {
            artifacts = new Artifact[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                artifacts[i] = data[i].DeepCopy();
                artifacts[i].SetData(so_data[i]);
            }
        }
    }

    #endregion

    #region 실제 인스턴스
    public class Artifact
    {
        [Newtonsoft.Json.JsonIgnore]
        public SO_Artifact Data;

        //? ID값 대체
        public ArtifactLabel Indexer;
        //? 실제 개수
        public int Count;

        public Artifact()
        {

        }
        public Artifact(SO_Artifact _data)
        {
            Data = _data;
            Indexer = (ArtifactLabel)_data.id;
        }

        public void Add()
        {
            Count++;
        }
        public void Sub()
        {
            Count--;
        }


        public Artifact DeepCopy()
        {
            var newArti = new Artifact();
            newArti.Indexer = Indexer;
            newArti.Count = Count;

            return newArti;
        }
        public void SetData(SO_Artifact _data)
        {
            Data = _data;
        }
    }

    Artifact[] artifacts;
    void Init_ArtifactArray()
    {
        //? SO_Artifact 개수만큼의 size로 어레이를 만듦
        int size = so_data.Length;
        artifacts = new Artifact[size];

        for (int i = 0; i < size; i++)
        {
            artifacts[i] = new Artifact(so_data[i]);
        }
    }

    public int DataSize() { return artifacts.Length; }

    public List<Artifact> GetCurrentArtifact()
    {
        List<Artifact> artiList = new List<Artifact>();

        foreach (var item in artifacts)
        {
            if (item.Count > 0)
            {
                artiList.Add(item);
            }
        }

        return artiList;
    }

    public Artifact GetArtifact(ArtifactLabel label)
    {
        foreach (var item in artifacts)
        {
            if (item.Indexer == label)
            {
                return item;
            }
        }
        return null;
    }
    public void AddArtifact(ArtifactLabel label)
    {
        GetArtifact(label).Add();
        Artifact_Effection();
    }
    public void SubtractArtifact(ArtifactLabel label)
    {
        GetArtifact(label).Sub();
        Artifact_Effection();
    }

    public void Add_RandomArtifact()
    {
        int random = Random.Range((int)ArtifactLabel.Harp, (int)ArtifactLabel.Dice + 1);
        AddArtifact((ArtifactLabel)random);
    }

    #endregion


    public Transform ArtifactBox;


    #region 아티팩트 효과

    void Artifact_Effection()
    {
        foreach (var item in GetCurrentArtifact())
        {
            switch (item.Indexer)
            {
                case ArtifactLabel.Harp:
                    GameManager.Buff.FacilityBonus = item.Count * 1;
                    break;

                case ArtifactLabel.Hourglass:
                    GameManager.Buff.HerbBonus = item.Count * 2;
                    break;

                case ArtifactLabel.Lamp:
                    GameManager.Buff.MineralBonus = item.Count * 2;
                    break;

                case ArtifactLabel.Mirror:
                    GameManager.Buff.PortalBonus = item.Count * 1;
                    break;

                case ArtifactLabel.Lyre:
                    GameManager.Buff.BattleBonus = item.Count * 5;
                    break;

                case ArtifactLabel.Pearl:
                    GameManager.Buff.ExpBonus = item.Count * 3;
                    break;

                case ArtifactLabel.Cup:
                    GameManager.Buff.ManaBonus = item.Count * 5;
                    break;

                case ArtifactLabel.Coin:
                    GameManager.Buff.GoldBonus = item.Count * 5;
                    break;

                case ArtifactLabel.Cross:
                    GameManager.Buff.PlayerHpBonus = item.Count * 5;
                    break;

                case ArtifactLabel.Dice:
                    GameManager.Buff.PlayerStatBonus = item.Count * 1;
                    break;
            }
        }
    }

    public void TurnStartEvent_Artifact()
    {
        int artifactCountAll = 0;
        foreach (var item in artifacts)
        {
            artifactCountAll += item.Count;
        }

        if (artifactCountAll > 0)
        {
            Main.Instance.CurrentDay.AddMana(artifactCountAll * 50, Main.DayResult.EventType.Artifacts);
            Main.Instance.ShowDM(artifactCountAll * 50, Main.TextType.mana, ArtifactBox);
        }
    }

    #endregion
}

public enum ArtifactLabel
{
    Harp = 2240,
    Hourglass,
    Lamp,
    Mirror,
    Lyre,
    Pearl,
    Cup,
    Coin,
    Cross,
    Dice,

}

public enum ArtifactGroup
{
    Buff,
    Evolution,
    KeyItem,
}
