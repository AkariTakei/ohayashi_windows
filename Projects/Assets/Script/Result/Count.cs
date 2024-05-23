using System.Collections;
using UnityEngine;


public class Count : MonoBehaviour
{
    //カウントアップを行うクラス

    private int m_Mode = 0;             // 0:動作していない 1:値を増加させる(動作中)
    private float m_Value = 0.0f;       // 現在値
    private float m_Start = 0.0f;       // 開始値
    private float m_Goal = 0.0f;        // 最終目的値
    private decimal m_PerTime = 0.0m;   // 値が"1"変化するのに必要な時間
    private decimal m_Time = 0.0m;      // 経過時間

    GameObject resultUI;

    public float Value
    {
        get
        {
            return m_Value;
        }
    }

    void Start()
    {
        resultUI = GameObject.Find("ResultUI");
        ResetVariable();
    }

    void Update()
    {
        // カウントアップ
        if (m_Mode > 0)
        {
            if (m_Value > m_Goal)
            {
                // 終了予告
                m_Value = m_Goal;
                return;
            }
            else if (m_Value == m_Goal)
            {
                // 終了
                m_Mode = 0;
                resultUI.GetComponent<IListenFinishable>().OnFinish(this.gameObject.name); // カウントアップが終了したことを通知する
                return;
            }
            m_Value = (float)((decimal)m_Start + m_Time / m_PerTime);   // ※１ 割る数(m_PerTime)が0だと例外発生する
        }
        // 動作していないとき
        else
        {
            return;
        }

        m_Time += (decimal)Time.deltaTime;  // 初回は"m_Value==m_Start"にしたいので、m_Timeは最後に更新する
    }



    private void ResetVariable()
    {
        m_Mode = 0;
        m_Value = 0.0f;
        m_Start = 0.0f;
        m_Goal = 0.0f;
        m_PerTime = 0.0m;
        m_Time = 0.0m;
    }

    // カウントアップ(またはダウン)を開始する
    public void CountToInt(int start, int goal, float time)
    {
        // 起動中ならば実行できないものとする
        if (IsWorking())
        {
            return;
        }

        // カウントアップ(またはダウン)の各種値を設定①
        if (start < goal)
        {
            m_Mode = 1;
        }
        else if (start > goal)
            m_Mode = -1;
        else
        {
            m_Mode = 0;
            resultUI.GetComponent<IListenFinishable>().OnFinish(this.gameObject.name); //もし開始値と目標値が同じなら終了を通知する
            return;
        }
        m_PerTime = (decimal)(goal - start) / (decimal)time;// 1.0秒毎の変化量
        m_PerTime = 1.0m / m_PerTime;                       // 値"1"(または-1)変化するのに必要な時間
        if (m_PerTime < 0)
            m_PerTime *= (-1.0m);                           // m_PerTimeは時間なので、"m_PerTime>=0"

        // m_PerTime==0.0mになると、※１で例外が発生するので実行せずに終了する。
        if (m_PerTime <= 0.0m)
        {
            ResetVariable();
        }

        // カウントアップ(またはダウン)の各種値を設定②
        m_Value = start;    // 現在値(開始値)を設定
        m_Start = start;    // 開始値を設定
        m_Goal = goal;      // 最終目的値を設定
        m_Time = 0.0m;      // 経過時間をリセット
    }

    public bool IsWorking()
    {
        return (m_Mode == 0) ? false : true;
    }

}