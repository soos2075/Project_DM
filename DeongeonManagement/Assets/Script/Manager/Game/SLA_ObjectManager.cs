using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SLA_ObjectManager : MonoBehaviour
{
    private static SLA_ObjectManager _instance;
    public static SLA_ObjectManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }


    public Transform Root { get { return Init_Root(); } }
    Transform _root;
    Transform Init_Root()
    {
        if (_root != null)
        {
            return _root;
        }
        else
        {
            GameObject obj = GameObject.Find("@SLA_Objects_Root");
            if (obj == null)
            {
                obj = new GameObject { name = "@SLA_Objects_Root" };
            }
            _root = obj.transform;
            return _root;
        }
    }




    public GameObject Interaction_Origin;
    Dictionary<string, SpriteResolver> objectDict = new Dictionary<string, SpriteResolver>();





    SpriteResolver GetRegistObject(string _name)
    {
        SpriteResolver resolver;
        if (objectDict.TryGetValue(_name, out resolver))
        {
            return resolver;
        }
        Debug.Log($"No Object : {_name}");
        return resolver;
    }



    public void SetLabel(string _name, string _categoryName, string _labelName)
    {
        if (objectDict.ContainsKey(_name))
        {
            GetRegistObject(_name).SetCategoryAndLabel(_categoryName, _labelName);
        }
    }

    public void CreateObject(string _name, Vector3 pos)
    {
        var newObj = Instantiate(Interaction_Origin, pos, Quaternion.identity, Root);
        newObj.name = _name;
        objectDict.Add(_name, newObj.GetComponentInChildren<SpriteResolver>());
    }
}
