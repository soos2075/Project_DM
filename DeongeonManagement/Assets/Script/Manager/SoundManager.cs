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
                go.AddComponent<SoundManager>();
                _instance = go.GetComponent<SoundManager>();
            }
            DontDestroyOnLoad(_instance);
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


    private void Awake()
    {
        DontDestroyOnLoad(this);

        for (int i = 0; i < Sound_Root.transform.childCount; i++)
        {
            audioSources[i] = Sound_Root.transform.GetChild(i).GetComponent<AudioSource>();
        }

        audioSources[(int)Define.AudioType.BGM].volume = PlayerPrefs.GetFloat("BGM_Volume", 0.7f);
        audioSources[(int)Define.AudioType.Effect].volume = PlayerPrefs.GetFloat("Effect_Volume", 0.7f);
    }


    private void Start()
    {
        
    }

    float timeCount;
    private void LateUpdate()
    {
        timeCount += Time.deltaTime;
        if (timeCount > 0.1f)
        {
            timeCount = 0;
            audioList.Clear();
        }
    }

    public void SetVolume(Define.AudioType type, float value)
    {
        audioSources[(int)type].volume = value;
    }

    public float GetVolume(Define.AudioType type)
    {
        return audioSources[(int)type].volume;
    }


    private void PlaySound (AudioClip clip, Define.AudioType type = Define.AudioType.Effect, float volume = 0.8f)
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
                    Debug.Log($"중복사운드 제거 : {clip.name}");
                    return;
                }
            }

            audioList.Add(clip);
            audioSources[(int)type].PlayOneShot(clip, volume);
        }
        else if (type == Define.AudioType.BGM)
        {
            audioSources[(int)type].loop = true;
            audioSources[(int)type].clip = clip;
            audioSources[(int)type].Play();
        }
    }

    public void PlaySound(string path, Define.AudioType type = Define.AudioType.Effect, float volume = 0.8f)
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
            PlaySound(clip, type, volume);
        }
    }


}
