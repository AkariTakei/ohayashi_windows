using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //ゲーム全体を管理するクラス
    public static GameManager instance = null;
    public float noteSpeed;
    [SerializeField] bool Start;
    float startTime;
    [SerializeField] string songName;
    [SerializeField] string instrument;
    string mode;

    bool isPause = false;

    float pauseTime; //ポーズした時間を格納

    void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (isPause == true)
        {
            //ポーズ中は時間をカウントする
            pauseTime += Time.deltaTime;
        }

        if (Start == false)
        {
            isPause = false;
            pauseTime = 0;
        }

    }


    public float GetNoteSpeed
    {
        get { return noteSpeed; }
    }

    public bool GetSetStart
    {
        get { return Start; }
        set { Start = value; }
    }

    public float GetSetStartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }

    public string GetSetSongName
    {
        get { return songName; }
        set { songName = value; }
    }

    public string GetSetInstrument
    {
        get { return instrument; }
        set { instrument = value; }
    }

    public string GetSetMode
    {
        get { return mode; }
        set { mode = value; }
    }

    public bool GetSetPause
    {
        get { return isPause; }
        set { isPause = value; }
    }

    public float GetPauseTime
    {
        get { return pauseTime; }
    }
}
