using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SelectMusicManager : MonoBehaviour
{
    //選曲画面のUIを管理するクラス
    [SerializeField] private string musicTitle;

    [SerializeField] private string instrument;
    [SerializeField] private string mode;

    [SerializeField] UnityEvent ChengeTextEvent;


    void Start()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/savedata.json") == false)
        {
            SaveDataManager.ResetData();
        }
    }
    public void OnClick()
    {
        GameManager.instance.GetSetSongName = musicTitle;
        GameManager.instance.GetSetInstrument = instrument;
        GameManager.instance.GetSetMode = mode;
        Invoke("ChangeScene", 0.5f);

    }

    void ChangeScene()
    {
        SceneManager.LoadScene("GameScene");
    }


    public string GetSetMusicTitle
    {
        get { return musicTitle; }
        set
        {
            musicTitle = value;
            ChengeTextEvent.Invoke();
        }
    }

    public string GetSetInstrument
    {
        get { return instrument; }
        set
        {
            instrument = value;
            ChengeTextEvent.Invoke();
        }
    }

    public string GetSetMode
    {
        get { return mode; }
        set
        {
            mode = value;
            ChengeTextEvent.Invoke();
        }
    }


}
