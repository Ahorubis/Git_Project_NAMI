using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonCon : MonoBehaviour
{    
    private GameObject[] buttonArray;

    private Player player;
    private SystemCon system;

    private void Awake()
    {
        if (GameObject.Find("Player")) player = GameObject.Find("Player").GetComponent<Player>();
        system = GameObject.Find("SystemCon").GetComponent<SystemCon>();
    }

    public void ButtonIndex(int index)
    {
        SpriteRenderer spriteRen = player.GetComponent<SpriteRenderer>();

        int temp = index - 1;

        Destroy(player.GetComponent<PolygonCollider2D>());

        player.ButtonIndex = index;

        spriteRen.sprite = player.fishList[temp];
        spriteRen.color = Color.white;

        player.GetComponent<AudioSource>().PlayOneShot(player.selectFish);

        if (spriteRen.sprite == player.fishList[1]) player.transform.localScale = new Vector2(0.7f, 0.7f);
        else if (spriteRen.sprite != player.fishList[1]) player.transform.localScale = new Vector2(-0.7f, 0.7f);

        player.AddComponent<PolygonCollider2D>();
    }

    public void ChangeNext(string scene)
    {
        if (scene == "Exit") Application.Quit();
        else SceneCon.Scene.LoadScene(scene);
    }

    public void StopWatch(bool pauseTime)
    {
        player.StatusPause = pauseTime;
        system.TimeDelta(pauseTime);
    }

    public void PauseWindow(bool timePause)
    {
        player.StatusPause = timePause;
        player.SetWindow();
    }

    public void MakeObject(GameObject obj)
    {
        Instantiate(obj);
    }

    public void DestroyThisObject(GameObject obj)
    {
        Destroy(obj);
    }
}
