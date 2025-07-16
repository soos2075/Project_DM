using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<SoundManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@SoundManager" };
                _instance = go.AddComponent<SoundManager>();
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

    GameObject Sound_Root //? 
    {
        get
        {
            GameObject root = GameObject.Find("@Sound_Root");
            if (root == null)
            {
                root = new GameObject { name = "@Sound_Root" };

                GameObject effect = new GameObject { name = "Effect" };
                effect.transform.parent = root.transform;
                effect.AddComponent<AudioSource>();

                GameObject bgm = new GameObject { name = "BGM" };
                bgm.transform.parent = root.transform;
                AudioSource bas = bgm.AddComponent<AudioSource>();
            }
            DontDestroyOnLoad(root);
            return root;
        }
    }

    AudioSource[] audioSources = new AudioSource[2];
    AudioSource[] AudioSources
    {
        get
        {
            if (audioSources[0] == null)
            {
                for (int i = 0; i < Sound_Root.transform.childCount; i++)
                {
                    audioSources[i] = Sound_Root.transform.GetChild(i).GetComponent<AudioSource>();
                }
            }
            return audioSources;
        }
        set { audioSources = value; }
    }

    Dictionary<string, AudioClip> clipCaching = new Dictionary<string, AudioClip>();

    [SerializeField]
    List<AudioClip> audioList = new List<AudioClip>(); //? 0.1초 안에 플레이요청들어온 clip목록. 동일clip은 2개까지만 허용함 -> 허용안함


    private void Start()
    {
        //AudioSources[(int)Define.AudioType.BGM].volume = UserData.Instance.GetDataFloat(PrefsKey.Volume_BGM, 0.7f);
        //AudioSources[(int)Define.AudioType.Effect].volume = UserData.Instance.GetDataFloat(PrefsKey.Volume_Effect, 0.7f);

        //PlaySound("BGM/_Title_Arcade", Define.AudioType.BGM);

        StartCoroutine(AfterAllStartsComplete());
    }

    private IEnumerator AfterAllStartsComplete()
    {
        yield return null;

        //AudioSources[(int)Define.AudioType.BGM].volume = UserData.Instance.CurrentPlayerData.option.Volume_BGM;
        //AudioSources[(int)Define.AudioType.Effect].volume = UserData.Instance.CurrentPlayerData.option.Volume_Effect;

        SetVolume_Offset(Define.AudioType.BGM, UserData.Instance.CurrentPlayerData.option.Volume_BGM);
        SetVolume_Offset(Define.AudioType.Effect, UserData.Instance.CurrentPlayerData.option.Volume_Effect);


        //PlaySound("BGM/_Title_Arcade", Define.AudioType.BGM);
        //PlaySound("New_BGM/Welcome, Heroes", Define.AudioType.BGM);
        Play_Main_Default();
    }

    public void Play_Main_Default()
    {
        if (UserData.Instance.CurrentPlayerData.EndingClearNumber() >= System.Enum.GetValues(typeof(Endings)).Length)
        {
            PlaySound("New_BGM/Beyond the Dungeon", Define.AudioType.BGM);
        }
        else
        {
            PlaySound("New_BGM/Welcome, Heroes", Define.AudioType.BGM);
        }
    }




    float timeCount;
    private void LateUpdate()
    {
        timeCount += Time.unscaledDeltaTime;
        if (timeCount > 0.1f)
        {
            timeCount = 0;
            audioList.Clear();
        }
    }


    void SetVolume_Offset(Define.AudioType type, float value)
    {
        switch (type)
        {
            case Define.AudioType.Effect:
                AudioSources[(int)type].volume = value;
                break;

            case Define.AudioType.BGM:
                AudioSources[(int)type].volume = value * 0.4f;
                break;
        }
    }



    public void SetVolume(Define.AudioType type, float value)
    {
        SetVolume_Offset(type, value);
        //AudioSources[(int)type].volume = value;


        switch (type)
        {
            case Define.AudioType.Effect:
                //UserData.Instance.SetData(PrefsKey.Volume_Effect, value);
                UserData.Instance.CurrentPlayerData.option.Volume_Effect = value;
                break;
            case Define.AudioType.BGM:
                //UserData.Instance.SetData(PrefsKey.Volume_BGM, value);
                UserData.Instance.CurrentPlayerData.option.Volume_BGM = value;
                break;
        }

    }

    public float GetVolume(Define.AudioType type)
    {
        return AudioSources[(int)type].volume;
    }


    private void PlaySound (AudioClip clip, Define.AudioType type = Define.AudioType.Effect, float volume = 0.7f)
    {
        if (type == Define.AudioType.Effect)
        {
            if (audioList.Count >= 1)
            {
                int sameCount = 0;
                for (int i = 0; i < audioList.Count; i++)
                {
                    if (clip == audioList[i])
                    {
                        sameCount++;
                    }
                }

                if (sameCount >= 1)
                {
                    //Debug.Log($"중복사운드 제거 : {clip.name}");
                    return;
                }
            }

            audioList.Add(clip);
            AudioSources[(int)type].PlayOneShot(clip); //? 볼륨 입력값 따로 받을거면 스위치로 아예 나누는게 맞을듯
        }
        else if (type == Define.AudioType.BGM)
        {
            AudioSources[(int)type].loop = true;
            AudioSources[(int)type].clip = clip;
            AudioSources[(int)type].Play();
        }
    }

    public void PlaySound(string path, Define.AudioType type = Define.AudioType.Effect)
    {
        AudioClip clip;
        clipCaching.TryGetValue(path, out clip); //? 캐싱된게 있는지 먼저 검색

        if (clip == null) //? 없으면 리소스에서 가져오기 + 사전에 추가
        {
            clip = Resources.Load<AudioClip>($"Sounds/{path}");
            clipCaching.Add(path, clip); 
        }


        if (clip == null) //? 리소스에서도 못가져왔으면 리턴
        {
            Debug.Log($"{path} 를 찾을 수 없습니다");
            return;
        }
        else
        {
            PlaySound(clip, type);
        }
    }


    public void StopMusic(Define.AudioType type = Define.AudioType.BGM)
    {
        AudioSources[(int)type].Stop();
    }
    public void ReStartMusic(Define.AudioType type = Define.AudioType.BGM)
    {
        AudioSources[(int)type].Play();
    }


    public void Reset_MainBGM()
    {
        if (AudioSources[(int)Define.AudioType.BGM].clip.name != "Let_s build")
        {
            ChangeBGM("Let_s build", 2.0f);
        }
    }

    public void ChangeBGM(string _name, float duration)
    {
        StartCoroutine(FadeChangeBGM(_name, duration));
    }



    IEnumerator FadeChangeBGM(string _name, float duration)
    {
        float startVolume = AudioSources[(int)Define.AudioType.BGM].volume;

        float elapsedTime = 0;

        while (duration > elapsedTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float currentValue = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            AudioSources[(int)Define.AudioType.BGM].volume = currentValue;
            yield return null;
        }

        AudioSources[(int)Define.AudioType.BGM].volume = 0;

        yield return new WaitForSecondsRealtime(1.0f);

        PlaySound($"New_BGM/{_name}", Define.AudioType.BGM);
        AudioSources[(int)Define.AudioType.BGM].volume = startVolume;
    }


    public void ReplaceSound(string _clipPath)
    {
        StartCoroutine(WaitForSound(_clipPath));
    }

    IEnumerator WaitForSound(string _clipPath)
    {
        yield return new WaitForEndOfFrame();
        if (audioList.Count == 0)
        {
            PlaySound($"SFX/{_clipPath}");
        }
        //else
        //{
        //    Debug.Log("이미 사운드 존재함");
        //}
    }
}
