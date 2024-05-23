using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    public string name;
    public int maxBlock;
    public int BPM;
    public int offset;
    public Note[] notes;
}

[Serializable]
public class Note
{
    public int type;
    public int num;
    public int block;
    public int LPB;
}

public class NotesManager : MonoBehaviour
{
    //ノーツの生成を管理するクラス

    public int noteNum; //総ノーツ数
    private string songName; //曲名

    public List<int> LaneNum = new List<int>(); //どのブロックにノーツが来るか

    private int preLaneNum; //1つ前のブロックの番号
    public List<int> NoteType = new List<int>(); //ノーツの種類
    public List<float> NotesTime = new List<float>(); //ノーツが判定線と重なる時間

    public List<GameObject> NotesObj = new List<GameObject>(); //ノーツのオブジェクト

    private float NotesSpeed;
    [SerializeField] GameObject[] noteObj = new GameObject[2];

    [SerializeField] GameObject rightJudge;
    [SerializeField] GameObject leftJudge;

    [SerializeField] GameObject[] ninnbaNotes; // 小太鼓用譜面
    [SerializeField] GameObject[] ninnba2Notes; //　大太鼓用譜面
    [SerializeField] GameObject[] ninnba3Notes; // 鉦すり用譜面
    [SerializeField] GameObject[] yataiNotes;
    [SerializeField] GameObject[] yatai2Notes;
    [SerializeField] GameObject[] yatai3Notes;
    [SerializeField] GameObject[] nakanokiriNotes;
    [SerializeField] GameObject[] nakanokiri2Notes;
    [SerializeField] GameObject[] nakanokiri3Notes;
    [SerializeField] GameObject[] tihyaitoroNotes;
    [SerializeField] GameObject[] tihyaitoro2Notes;
    [SerializeField] GameObject[] tihyaitoro3Notes;
    Vector2 localRightPos;
    Vector2 localLeftPos;

    void Start()
    {
        if (GameManager.instance != null)
        {
            NotesSpeed = GameManager.instance.GetNoteSpeed;
            noteNum = 0;
            songName = GameManager.instance.GetSetSongName;
            localRightPos = rightJudge.transform.position;
            localLeftPos = leftJudge.transform.position;


            //選んだ曲に対応する譜面を読み込む
            if (GameManager.instance.GetSetInstrument == "tuke")
            {
                switch (songName)
                {
                    case "ninnba":
                        Load(songName, ninnbaNotes);
                        break;
                    case "yatai":
                        Load(songName, yataiNotes);
                        break;
                    case "nakanokiri":
                        Load(songName, nakanokiriNotes);
                        break;
                    case "ti-hyaitoro":
                        Load(songName, tihyaitoroNotes);
                        break;
                }
            }

            else if (GameManager.instance.GetSetInstrument == "ookan")
            {
                switch (songName)
                {
                    case "ninnba":
                        Load(songName + "2", ninnba2Notes);
                        break;
                    case "yatai":
                        Load(songName + "2", yatai2Notes);
                        break;
                    case "nakanokiri":
                        Load(songName + "2", nakanokiri2Notes);
                        break;
                    case "ti-hyaitoro":
                        Load(songName + "2", tihyaitoro2Notes);
                        break;
                }
            }

            else if (GameManager.instance.GetSetInstrument == "kane")
            {
                switch (songName)
                {
                    case "ninnba":
                        Load(songName + "3", ninnba3Notes);
                        break;
                    case "yatai":
                        Load(songName + "3", yatai3Notes);
                        break;
                    case "nakanokiri":
                        Load(songName + "3", nakanokiri3Notes);
                        break;
                    case "ti-hyaitoro":
                        Load(songName + "3", tihyaitoro3Notes);
                        break;
                }
            }

        }
    }


    private void Load(string SongName, GameObject[] songNotes)
    {
        //ノーツ情報を読み込み、ノーツを生成する

        string inputString = Resources.Load<TextAsset>(SongName).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString); //デシリアル化

        noteNum = inputJson.notes.Length;


        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB); //1拍の間隔
            float beatSec = kankaku * (float)inputJson.notes[i].LPB; //1拍の秒数
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset * 0.00001f; //ノーツが判定線と重なる時間
            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);
            float y = NotesTime[i] * NotesSpeed;
            if (i + 1 < inputJson.notes.Length && inputJson.notes[i + 1].block == 2)
            {
                preLaneNum = inputJson.notes[i].block;
            }
            if (inputJson.notes[i].block == 0) //右手で叩くノーツ
            {
                NotesObj.Add(Instantiate(songNotes[i], new Vector2(localRightPos.x, y + localRightPos.y), Quaternion.identity));
                NotesObj[i].transform.SetParent(rightJudge.transform, true);
                NotesObj[i].transform.localScale = new Vector2(0.5f, 0.5f);
            }
            else if (inputJson.notes[i].block == 1) //左手で叩くノーツ
            {
                NotesObj.Add(Instantiate(songNotes[i], new Vector2(localLeftPos.x, y + localLeftPos.y), Quaternion.identity));
                NotesObj[i].transform.SetParent(leftJudge.transform, true);
                NotesObj[i].transform.localScale = new Vector2(0.5f, 0.5f);
            }

            else
            {
                //叩かないノーツは視線の移動を増やさないよう一つ前に生成されたノーツの位置に生成する
                if (preLaneNum == 0)
                {
                    NotesObj.Add(Instantiate(songNotes[i], new Vector2(localRightPos.x, y + localRightPos.y), Quaternion.identity));
                }
                else
                {
                    NotesObj.Add(Instantiate(songNotes[i], new Vector2(localLeftPos.x, y + localLeftPos.y), Quaternion.identity));
                }
                NotesObj[i].transform.SetParent(leftJudge.transform, true);
                NotesObj[i].transform.localScale = new Vector2(0.6f, 0.6f);
            }


        }
    }


}
