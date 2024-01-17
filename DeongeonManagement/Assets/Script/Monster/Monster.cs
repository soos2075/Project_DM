using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Monster : MonoBehaviour
{
    protected void Awake()
    {

    }
    protected void Start()
    {
        Initialize();
        SetSprite($"Sprite/Monster/{this.name}");
    }
    protected void Update()
    {

    }


    #region Monster Status Property

    public Sprite Sprite { get; set; }
    public string Name { get; set; }
    public int HP { get; set; }
    public int LV { get; set; }


    #endregion

    protected virtual void Initialize()
    {
        SetStatus("Slime", 10, 1);
    }

    protected void SetStatus(string _name, int _hp, int _lv)
    {
        Name = _name;
        HP = _hp;
        LV = _lv;
    }

    protected void SetSprite(string _path)
    {
        Sprite = Managers.Resource.Load<Sprite>(_path);
    }





    public void Summon()
    {

    }

    public void Batch()
    {

    }

    public void Training()
    {

    }
}


