using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class ResultManager : MonoBehaviour
{
    //リザルト画面の管理をするクラス
    ResultUI resultUI;
    private string songName;

    private string instrumentName;

    private string levelName;
    private int combo;
    private int great;
    private int good;
    private int bad;



    private void Start()
    {
        resultUI = GameObject.Find("ResultUI").GetComponent<ResultUI>();
        resultUI.SetText(combo, great, good, bad, CaluculateScore(), CaluculateAccuracy());

        if (System.IO.File.Exists(Application.persistentDataPath + "/savedata.json") == false)
        {
            SaveDataManager.ResetData();
        }



        SaveDataManager.SaveData data = SaveDataManager.Load();

        for (int i = 0; i < data.songData.Length; i++)
        {
            if (data.songData[i].songName == GameManager.instance.GetSetSongName)
            {
                for (int j = 0; j < data.songData[i].instrumentData.Length; j++)
                {
                    if (data.songData[i].instrumentData[j].instrumentName == GameManager.instance.GetSetInstrument)
                    {
                        for (int k = 0; k < data.songData[i].instrumentData[j].levelData.Length; k++)
                        {
                            if (data.songData[i].instrumentData[j].levelData[k].levelName == GameManager.instance.GetSetMode)
                            {
                                data.songData[i].instrumentData[j].levelData[k].playNum++;
                                data.songData[i].instrumentData[j].levelData[k].combo = combo;
                                if (data.songData[i].instrumentData[j].levelData[k].score < CaluculateScore())
                                {
                                    data.songData[i].instrumentData[j].levelData[k].score = CaluculateScore();
                                    resultUI.SetHiScore(true);
                                }
                                if (bad == 0)
                                {
                                    data.songData[i].instrumentData[j].levelData[k].fullComboNum++;
                                }
                            }
                        }
                    }
                }
            }
        }
        SaveDataManager.Save(data);

        CaluculateScore();
    }

    public void SetResult(int combo, int great, int good, int bad)
    {
        this.combo = combo;
        this.great = great;
        this.good = good;
        this.bad = bad;
        Debug.Log("コンボ数 = " + combo + "Great数 = " + great + "Good数 = " + good + "Bad数 = " + bad);
    }

    private float CaluculateAccuracy()
    {
        //精度の計算
        float accuracy = ((float)great * 2 + good) / ((great + good + bad) * 2) * 100;
        //accuracyを小数点第2位までにする
        accuracy = Mathf.Round(accuracy * 100) / 100;
        return accuracy;
    }

    private int CaluculateScore()
    {
        int score = great * 330 + good * 100 + bad * 0;
        return score;
    }

    public void ChangeScene(string name)
    {
        if (name == "reStart")
        {
            SceneManager.LoadScene("GameScene");
        }

        else if (name == "next")
        {
            SceneManager.LoadScene("SelectMusicScene");
        }
    }
}
