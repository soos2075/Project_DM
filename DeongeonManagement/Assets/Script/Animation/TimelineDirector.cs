using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineDirector : MonoBehaviour
{
    public GameObject player;

    Animator anim_Player;

    void Start()
    {
        anim_Player = player.GetComponent<Animator>();

    }


    public void PlayerMove()
    {
        anim_Player.Play("walk_l");
        StartCoroutine(PlayerMove_1());
    }
    IEnumerator PlayerMove_1()
    {
        float timer = 0;
        while (timer < 2)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            player.transform.Translate(Vector3.left * Time.unscaledDeltaTime * 1f);
        }
        anim_Player.Play("idle_l");

    }


    public void StartDialogue()
    {
        Managers.Dialogue.ShowDialogueUI("Ending2", player.transform);
    }



}
