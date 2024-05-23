using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    //判定を行うクラス

    [SerializeField] private GameObject[] judgeObj = new GameObject[3]; //判定オブジェクト
    [SerializeField] NotesManager notesManager;
    GameUI gameUI;

    Animator animator;

    int deletedNotesNum = 0; //削除したノーツの数

    int[] judge = new int[3]; //良、可、不可の数

    private int maxCombo;
    private int combo;

    Vector2 localRightPos; //右の判定枠の座標
    Vector2 localLeftPos; //左の判定枠の座標

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            judge[i] = 0;
        }

        maxCombo = 0;
        combo = 0;

        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        animator = GameObject.Find("Instrument").GetComponent<Animator>();

        localRightPos = GameObject.Find("right").transform.position;
        localLeftPos = GameObject.Find("left").transform.position;
    }

    void Update()
    {
        if (GameManager.instance.GetSetStart && notesManager.noteNum > deletedNotesNum && GameManager.instance.GetSetPause == false)
        {

            if (Input.GetKeyDown(KeyCode.J))
            {
                animator.SetTrigger("Down");
                if (notesManager.LaneNum[0] == 0)
                {
                    Judgement(GetABS(((Time.time - GameManager.instance.GetPauseTime) - GameManager.instance.GetSetStartTime) - notesManager.NotesTime[0]), notesManager.LaneNum[0]); //どれぐらいずれているか
                    return;
                }

            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("Down");
                if (notesManager.LaneNum[0] == 1)
                {
                    Judgement(GetABS(((Time.time - GameManager.instance.GetPauseTime) - GameManager.instance.GetSetStartTime) - notesManager.NotesTime[0]), notesManager.LaneNum[0]);
                    return;

                }
            }

            //判定に関係のないノーツは判定枠に入ったら削除
            if ((Time.time - GameManager.instance.GetPauseTime) - GameManager.instance.GetSetStartTime > notesManager.NotesTime[0] && notesManager.LaneNum[0] == 2)
            {
                deleteData();
                deleteNotesObj();
                return;
            }

            //不可判定
            if ((Time.time - GameManager.instance.GetPauseTime) - GameManager.instance.GetSetStartTime > notesManager.NotesTime[0] + 0.1f)
            {
                message(2, notesManager.LaneNum[0]);
                deleteData();
                judge[2]++;
                deleteNotesObj();

                if (combo > maxCombo)
                {
                    maxCombo = combo;
                }
                combo = 0;
                gameUI.SetText(combo);
            }
        }

    }

    void Judgement(float timeLag, int num)
    {
        if (timeLag <= 0.05) //誤差が0.05秒以下なら良判定
        {
            judge[0]++;
            message(0, num);
            deleteData();
            deleteNotesObj();
            combo++;
        }

        else
        {
            if (timeLag <= 0.1) //誤差が0.1秒以下なら可判定
            {
                judge[1]++;
                message(1, num);
                deleteData();
                deleteNotesObj();
                combo++;
            }
        }
        gameUI.SetText(combo);
    }

    float GetABS(float num) //引数の絶対値を返す
    {
        if (num >= 0)
        {
            return num;
        }

        else
        {
            return -num;
        }
    }

    void deleteData()
    {
        notesManager.NotesTime.RemoveAt(0);
        notesManager.LaneNum.RemoveAt(0);
        notesManager.NoteType.RemoveAt(0);
    }

    void deleteNotesObj()
    {
        Destroy(notesManager.NotesObj[0]);
        notesManager.NotesObj.RemoveAt(0);
        deletedNotesNum++;
    }

    void message(int judge, int num)
    {
        //判定オブジェクトを生成
        if (num == 0)
        {
            Instantiate(judgeObj[judge], new Vector2(localRightPos.x + 1, localRightPos.y), Quaternion.identity);
        }

        else if (num == 1)
        {
            Instantiate(judgeObj[judge], new Vector2(localLeftPos.x - 1, localLeftPos.y), Quaternion.identity);
        }
    }

    public int[] GetJudge
    {
        get { return judge; }
    }

    public int GetCombo
    {
        get
        {
            if (judge[2] == 0)
            {
                maxCombo = combo;
            }
            return maxCombo;
        }
    }


    //以下はスマホ用のバイブレーションの処理(PC版ではコメントアウト)

    /*public void RightTouch()
    {
        VibrationMng.ShortVibration(); 
        animator.SetTrigger("Down");
        if (notesManager.LaneNum[0] == 0)
        {
            Judgement(GetABS((Time.time - GameManager.instance.GetSetStartTime) - notesManager.NotesTime[0]), notesManager.LaneNum[0]); //どれぐらいずれているか
            return;
        }

    }

    public void LeftTouch()
    {
        VibrationMng.ShortVibration();  
        animator.SetTrigger("Down");
        if (notesManager.LaneNum[0] == 1)
        {
            Judgement(GetABS((Time.time - GameManager.instance.GetSetStartTime) - notesManager.NotesTime[0]), notesManager.LaneNum[0]);
            return;

        }

    }
    */

}
