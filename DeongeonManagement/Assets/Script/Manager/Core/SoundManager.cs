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

    Dictionary<string, AudioClip> clipCaching = new Dictionary<string, AudioClip>();

    [SerializeField]
    List<AudioClip> audioList = new List<AudioClip>(); //? 0.1초 안에 플레이요청들어온 clip목록. 동일clip은 2개까지만 허용함 -> 허용안함

    private void Start()
    {
        for (int i = 0; i < Sound_Root.transform.childCount; i++)
        {
            audioSources[i] = Sound_Root.transform.GetChild(i).GetComponent<AudioSource>();
        }

        audioSources[(int)Define.AudioType.BGM].volume = UserData.Instance.GetDataFloat(PrefsKey.Volume_BGM, 0.7f);
        audioSources[(int)Define.AudioType.Effect].volume = UserData.Instance.GetDataFloat(PrefsKey.Volume_Effect, 0.7f);

        PlaySound("BGM/StartScene", Define.AudioType.BGM);
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

    public void SetVolume(Define.AudioType type, float value)
    {
        audioSources[(int)type].volume = value;


        switch (type)
        {
            case Define.AudioType.Effect:
                UserData.Instance.SetData(PrefsKey.Volume_Effect, value);
                break;
            case Define.AudioType.BGM:
                UserData.Instance.SetData(PrefsKey.Volume_BGM, value);
                break;
        }

    }

    public float GetVolume(Define.AudioType type)
    {
        return audioSources[(int)type].volume;
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
            audioSources[(int)type].PlayOneShot(clip); //? 볼륨 입력값 따로 받을거면 스위치로 아예 나누는게 맞을듯
        }
        else if (type == Define.AudioType.BGM)
        {
            audioSources[(int)type].loop = true;
            audioSources[(int)type].clip = clip;
            audioSources[(int)type].Play();
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
        audioSources[(int)type].Stop();
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
