using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Monster), true)]
public class MonsterEditor : Editor
{
    Monster value;

    private void OnEnable()
    {
        value = (Monster)target;
    }

    public override void OnInspectorGUI()
    {
        //EditorGUILayout.BeginHorizontal();

        base.OnInspectorGUI();


    //    value.Sprite = (Sprite)EditorGUILayout.ObjectField("이미지", value._data[index].fishSprite, typeof(Sprite), true
    //, GUILayout.MinWidth(250), GUILayout.MinHeight(250));
        //value.Name = (string)EditorGUILayout.TextField("이름", value.Name);
        //value.HP = (int)EditorGUILayout.IntField("HP", value.HP);
        //value.LV = (int)EditorGUILayout.IntField("LV", value.LV);

        EditorGUILayout.ObjectField("이미지", value.Data.sprite, typeof(Sprite), true);

        EditorGUILayout.LabelField("이름", value.Name);
        EditorGUILayout.LabelField("HP", value.HP.ToString());
        EditorGUILayout.LabelField("LV", value.LV.ToString());
        EditorGUILayout.LabelField("ATK", value.ATK.ToString());
        EditorGUILayout.LabelField("DEF", value.DEF.ToString());
        EditorGUILayout.LabelField("AGI", value.AGI.ToString());
        EditorGUILayout.LabelField("LUK", value.LUK.ToString());


        //EditorGUILayout.EndHorizontal();
    }
}

