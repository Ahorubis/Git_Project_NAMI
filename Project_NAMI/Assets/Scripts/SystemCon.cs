using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCon : MonoBehaviour
{
    private static SystemCon systemCon;

    private void Awake()
    {
        if (systemCon != null) Destroy(this.gameObject);

        else
        {
            systemCon = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public static SystemCon System
    {
        get
        {
            if (systemCon == null) return null;
            return systemCon;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneCon.Scene.LoadScene("MainScene");
    }

    public void PauseMode(bool pause)
    {
        if (pause) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void TimeDelta(bool pause)
    {
        if (pause) Time.timeScale = 0;
        else Time.timeScale = 1;
    }
}
