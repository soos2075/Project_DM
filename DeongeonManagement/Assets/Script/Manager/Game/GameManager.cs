using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { Init(); return _instance; } }



    PlacementManager _placement = new PlacementManager();
    ContentManager _content = new ContentManager();
    FacilityManager _facility = new FacilityManager();
    TechnicalManager _technical = new TechnicalManager();
    MonsterManager _monster = new MonsterManager();
    NPCManager _npc = new NPCManager();
    TraitManager _trait = new TraitManager();
    PixelHeroManager _pixel = new PixelHeroManager();
    BuffManager _buff = new BuffManager();
    ArtifactManager _artifact = new ArtifactManager();



    public static PlacementManager Placement { get { return Instance._placement; } }
    public static ContentManager Content { get { return Instance._content; } }
    public static FacilityManager Facility { get { return Instance._facility; } }
    public static TechnicalManager Technical { get { return Instance._technical; } }
    public static MonsterManager Monster { get { return Instance._monster; } }
    public static NPCManager NPC { get { return Instance._npc; } }
    public static TraitManager Trait { get { return Instance._trait; } }
    public static PixelHeroManager Pixel { get { return Instance._pixel; } }
    public static BuffManager Buff { get { return Instance._buff; } }
    public static ArtifactManager Artifact { get { return Instance._artifact; } }



    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@GameManager");
                _instance = go.AddComponent<GameManager>();
                //DontDestroyOnLoad(go);
            }
        }
    }


    void Awake()
    {
        Init();
        _placement.Init();
        _content.Init();
        _facility.Init();
        _technical.Init();
        _monster.Init();
        _npc.Init();
        _trait.Init();
        _pixel.Init();
        _buff.Init();
        _artifact.Init();
    }

    void Start()
    {
        
    }
}
