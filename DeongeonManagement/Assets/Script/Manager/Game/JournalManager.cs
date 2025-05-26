using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    #region Singleton
    private static JournalManager _instance;
    public static JournalManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<JournalManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@JournalManager" };
                _instance = go.AddComponent<JournalManager>();
            }
            DontDestroyOnLoad(_instance);
        }
    }


    private void Awake()
    {
        Initialize();
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion

    #region SO_Data
    SO_Journal[] so_data;

    public void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_Journal>("Data/Journal");
        foreach (var item in so_data)
        {
            string[] datas = Managers.Data.GetTextData_Journal(item.ID);

            if (datas == null)
            {
                Debug.Log($"{item.ID} : CSV Data Not Exist");
                continue;
            }

            item.title = datas[0];
            item.description = datas[1];
        }
    }

    public SO_Journal GetData(int _id)
    {
        foreach (var item in so_data)
        {
            if (item.ID == _id)
            {
                return item;
            }
        }
        Debug.Log($"{_id}: Data Not Exist");
        return null;
    }
    #endregion

    private void Start()
    {
        Managers.Data.OnAddressablesComplete += () => Init_LocalData();
    }



    #region Save Load

    public List<JournalData> Save_JournalData()
    {
        List<JournalData> saveData = new List<JournalData>();
        foreach (var item in CurrentJournalList)
        {
            saveData.Add(item.DeepCopy());
        }
        return saveData;
    }
    public void Load_JournalData(List<JournalData> data)
    {
        if (data != null)
        {
            var loadData = new List<JournalData>();
            foreach (var item in data)
            {
                loadData.Add(item.DeepCopy());
            }
            CurrentJournalList = loadData;
        }
    }

    #endregion



    //? 새 일지 알림용
    public bool Check_Journal
    {
        get 
            {
            foreach (var item in CurrentJournalList)
            {
                if (item.noticeCheck == false)
                {
                    return true;
                }
            }
            return false;
        } 
    }

    ////? 1회용 점쟁이 구슬 (이벤트 봤을 때)
    //public bool Next_RE_Info { get; set; }




    #region 등록된 일지 (실제로 진행되는 퀘스트와는 별개. 정보를 얻은 퀘스트만 등록)

    public class JournalData
    {
        public int ID;

        public int startDay;
        public int endDay;

        public bool noticeCheck;

        public JournalData(int _id)
        {
            ID = _id;

            var currentData = RandomEventManager.Instance.GetRandomEventData(_id);
            if (currentData != null)
            {
                startDay = currentData.startDay;
                endDay = currentData.endDay;
            }
        }


        //? 아래 세개는 사실 필요없을지도 모르겠음. (아래껄로 저장해버리면 런타임에서 언어바꿨을 때 번역이 안바뀜)
        public string title;
        public string detail;

        //? 이게 있으면 이걸 우선으로 출력하고, 없으면 그냥 startDay랑 endDay로 계산해서 출력하기
        public string dayInfo;


        public JournalData DeepCopy()
        {
            return (JournalData)this.MemberwiseClone();
        }
    }

    public List<JournalData> CurrentJournalList { get; set; } = new List<JournalData>();


    public void NewGame_Init()
    {
        CurrentJournalList = new List<JournalData>();
    }


    //? 기한 없는 퀘스트나 일지
    public void AddJournal(int _id)
    {
        foreach (var item in CurrentJournalList)
        {
            if (item.ID == _id)
            {
                return;
            }
        }

        CurrentJournalList.Add(new JournalData(_id));
    }
    public void AddJournal(int _id, int _startDay, int _endDay, string _dayInfo = "")
    {
        //var data = GetData(_id);
        var jour = new JournalData(_id);

        jour.startDay = _startDay;
        jour.endDay = _endDay;

        CurrentJournalList.Add(jour);
    }

    public void RemoveJournal(int _id)
    {
        foreach (var item in CurrentJournalList)
        {
            if (item.ID == _id)
            {
                CurrentJournalList.Remove(item);
                return;
            }
        }
    }


    #endregion


}
