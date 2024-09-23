using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{

    //? 지금 오브젝트 풀링을 안하고있어서 얜 따로 필요가 없는데 필요하면 다시 쓰면 댐
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }



    Dictionary<string, GameObject> gameObjectCaching = new Dictionary<string, GameObject>();
    public GameObject Load(string path)
    {
        GameObject obj = null;

        gameObjectCaching.TryGetValue(path, out obj);

        if (obj == null)
        {
            obj = Resources.Load<GameObject>(path);
            gameObjectCaching.Add(path, obj);
        }

        return obj;
    }



    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }

}
