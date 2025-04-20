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
        foreach (var item in so_data)
        {
            string[] datas = Managers.Data.GetTextData_Artifact(item.id);

            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];
            item.tooltip_Effect = datas[2];
        }
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
    public SO_Artifact GetData(int _id)
    {
        foreach (var item in so_data)
        {
            if (item.id == _id)
            {
                return item;
            }
        }
        Debug.Log($"{_id}: Data Not Exist");
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

                Artifact_AddCallback(artifacts[i].Indexer); //? 이건 항목에 따라 개별적용이니까 따로
            }
            Artifact_Effection(); //? 이건 어차피 한번만호출해도 전체적용되니까 마지막에만 해도댐
            ArtifactBox.Box_Update();
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
            AddCollectionPoint();
        }
        public void Sub()
        {
            Count--;
        }

        public void AddCollectionPoint()
        {
            var collection = CollectionManager.Instance.Get_Collection(Data);
            if (collection != null)
            {
                collection.AddPoint();
            }
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

    public int GetArtifactCountAll()
    {
        int artifactCountAll = 0;
        foreach (var item in artifacts)
        {
            artifactCountAll += item.Count;
        }
        return artifactCountAll;
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
    public bool Check_Artifact_Exist(ArtifactLabel label)
    {
        foreach (var item in artifacts)
        {
            if (item.Indexer == label && item.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void AddArtifact(ArtifactLabel label)
    {
        GetArtifact(label).Add();
        Artifact_Effection();
        Artifact_AddCallback(label);
        ArtifactBox.Box_Update();
    }
    public void SubtractArtifact(ArtifactLabel label)
    {
        GetArtifact(label).Sub();
        Artifact_Effection();
        ArtifactBox.Box_Update();
    }


    public void Add_RandomArtifact()
    {
        int random = Random.Range((int)ArtifactLabel.Harp, (int)ArtifactLabel.Dice + 1);
        AddArtifact((ArtifactLabel)random);


        Managers.UI.Popup_Reservation(() => {
            var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
            message.DelayTime = 0;
            //? 신규 아티팩트
            var arti = GameManager.Artifact.GetData(random);
            message.Message = $"{UserData.Instance.LocaleText("New")}{UserData.Instance.LocaleText("아티팩트")} : {arti.labelName}";
            Sprite sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.Artifact, arti.SLA_category, arti.SLA_label);
            message.Set_Image(sprite);

            //message.Message += $"\n\naa\n\nfwab";
        });
    }


    public void Check_10Treasure() //? 제 10대 비보를 다 모았는지
    {
        if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(11020))
        {
            return;
        }

        if (Check_Artifact_Exist(ArtifactLabel.Harp) && Check_Artifact_Exist((ArtifactLabel)201) && Check_Artifact_Exist((ArtifactLabel)202) &&
            Check_Artifact_Exist((ArtifactLabel)203) && Check_Artifact_Exist((ArtifactLabel)204) && Check_Artifact_Exist((ArtifactLabel)205) &&
            Check_Artifact_Exist((ArtifactLabel)206) && Check_Artifact_Exist((ArtifactLabel)207) && Check_Artifact_Exist((ArtifactLabel)208) &&
            Check_Artifact_Exist((ArtifactLabel)209))
        {
            EventManager.Instance.Add_GuildQuest_Special(11020);
        }
    }

    #endregion


    public ArtifactBox ArtifactBox { get { return GameObject.FindAnyObjectByType<ArtifactBox>(); } }


    #region 아티팩트 효과

    void Artifact_AddCallback(ArtifactLabel label)
    {
        switch (label)
        {
            case ArtifactLabel.ProofOfHero:
                Main.Instance.Player.GetComponent<Player>().Hero_Buff();
                break;
        }
    }

    void Artifact_Effection()
    {
        foreach (var item in GetCurrentArtifact())
        {
            switch (item.Indexer)
            {
                case ArtifactLabel.Harp:
                    GameManager.Buff.ManaAdd_Facility = item.Count * 1;
                    break;

                case ArtifactLabel.Hourglass:
                    GameManager.Buff.ManaAdd_Herb = item.Count * 2;
                    break;

                case ArtifactLabel.Lamp:
                    GameManager.Buff.ManaAdd_Mineral = item.Count * 2;
                    break;

                case ArtifactLabel.Mirror:
                    GameManager.Buff.ManaAdd_Portal = item.Count * 1;
                    break;

                case ArtifactLabel.Lyre:
                    GameManager.Buff.ManaAdd_Battle = item.Count * 5;
                    break;

                case ArtifactLabel.Pearl:
                    GameManager.Buff.ExpAdd_Battle = item.Count * 3;
                    break;

                case ArtifactLabel.Cup:
                    GameManager.Buff.ManaUp_Final = item.Count * 5;
                    break;

                case ArtifactLabel.Coin:
                    GameManager.Buff.GoldUp_Final = item.Count * 5;
                    break;

                //case ArtifactLabel.Cross:
                //    GameManager.Buff.HpUp_Unit = item.Count * 10;
                //    break;

                //case ArtifactLabel.Dice:
                //    GameManager.Buff.StatUp_Unit = item.Count * 5;
                //    break;
            }
        }
    }

    public void TurnStartEvent_Artifact()
    {
        int artifactCountAll = GetArtifactCountAll();
        if (artifactCountAll > 0)
        {
            Main.Instance.CurrentDay.AddMana(artifactCountAll * 50, Main.DayResult.EventType.Artifacts);
            Main.Instance.ShowDM(artifactCountAll * 50, Main.TextType.mana, ArtifactBox.transform, 2);
        }

        if (GetArtifact(ArtifactLabel.OrbOfPopularity).Count > 0)
        {
            Main.Instance.CurrentDay.AddPop(10);
            Main.Instance.ShowDM(10, Main.TextType.pop, ArtifactBox.transform, 2);
        }
        if (GetArtifact(ArtifactLabel.OrbOfDanger).Count > 0)
        {
            Main.Instance.CurrentDay.AddDanger(10);
            Main.Instance.ShowDM(10, Main.TextType.danger, ArtifactBox.transform, 2);
        }

        if (GetArtifact(ArtifactLabel.MarbleOfReassurance).Count > 0)
        {
            Main.Instance.CurrentDay.AddDanger(-5);
            Main.Instance.ShowDM(-5, Main.TextType.danger, ArtifactBox.transform, 2);
        }
        if (GetArtifact(ArtifactLabel.MarbleOfOblivion).Count > 0)
        {

            Main.Instance.CurrentDay.AddPop(-5);
            Main.Instance.ShowDM(-5, Main.TextType.pop, ArtifactBox.transform, 2);
        }

        Check_10Treasure();
    }

    #endregion
}

public enum ArtifactLabel
{
    //? 키아이템
    DungeonMaster_Temp = 0,

    Racing = 10,


    //? 진화재료 (효과없음)
    BananaBone = 100,


    //? 제 10대 비보
    Harp = 200,
    Hourglass,
    Lamp,
    Mirror,
    Lyre,
    Pearl,
    Cup,
    Coin,
    Cross,
    Dice = 209,


    //? 플레이어 강화
    ProofOfHero = 300,
    //? 모험가 약화
    TouchOfDecay = 301,

    //? 매일 인기도 / 위험도 +10
    OrbOfPopularity = 400,
    OrbOfDanger = 401,

    //? 매일 위험도 / 인기도 -5
    MarbleOfReassurance = 410,
    MarbleOfOblivion = 411,


}

public enum ArtifactGroup
{
    Buff,
    Evolution,
    KeyItem,
    Special,
}
