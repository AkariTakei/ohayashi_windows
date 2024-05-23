using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;

public class ResultUI : MonoBehaviour, IListenFinishable
{
    //リザルト画面のUIを管理するクラス
    [SerializeField] private GameObject greatObj;
    [SerializeField] private GameObject goodObj;
    [SerializeField] private GameObject badObj;
    [SerializeField] private GameObject comboObj;
    [SerializeField] private GameObject scoreObj;
    [SerializeField] private GameObject accuracyObj;
    [SerializeField] private GameObject comentText;
    [SerializeField] private Sprite[] levelSprite;
    private string[] level = { "easy", "normal", "hard" };
    [SerializeField] private Sprite[] MusicTitleSprite;
    private string[] MusicTitle = { "ninnba", "yatai", "nakanokiri", "ti-hyaitoro" };

    TextMeshProUGUI scoreText;
    TextMeshProUGUI greatText;
    TextMeshProUGUI goodText;
    TextMeshProUGUI badText;
    TextMeshProUGUI comboText;
    Count scoreCount;
    Count greatCount;
    Count goodCount;
    Count badCount;
    Count comboCount;

    private GameObject hiScore;
    private GameObject resultImage;
    private GameObject restartButton;
    private GameObject nextButton;

    int score;
    int combo;
    int great;
    int good;
    int bad;
    float accuracy;

    bool isHiScore = false;

    [SerializeField] UnityEvent reStartEvent;
    [SerializeField] UnityEvent nextEvent;
    Color color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 1f);


    public void SetHiScore(bool value)
    {
        isHiScore = value;
    }

    private void Awake()
    {
        scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        scoreCount = scoreObj.GetComponent<Count>();
        greatText = greatObj.GetComponent<TextMeshProUGUI>();
        greatCount = greatObj.GetComponent<Count>();
        goodText = goodObj.GetComponent<TextMeshProUGUI>();
        goodCount = goodObj.GetComponent<Count>();
        badText = badObj.GetComponent<TextMeshProUGUI>();
        badCount = badObj.GetComponent<Count>();
        comboText = comboObj.GetComponent<TextMeshProUGUI>();
        comboCount = comboObj.GetComponent<Count>();
        hiScore = GameObject.Find("hiscore");
        resultImage = GameObject.Find("resultImage");
        restartButton = GameObject.Find("reStart");
        nextButton = GameObject.Find("next");
        ResetResult();

    }

    private void Start()
    {
        GameObject levelObj = GameObject.Find("level");
        GameObject musicTitleObj = GameObject.Find("musicTitle");

        for (int i = 0; i < level.Length; i++)
        {
            if (level[i] == GameManager.instance.GetSetMode)
            {
                levelObj.GetComponent<Image>().sprite = levelSprite[i];
            }
        }

        levelObj.GetComponent<Image>().SetNativeSize();

        for (int i = 0; i < MusicTitle.Length; i++)
        {
            if (MusicTitle[i] == GameManager.instance.GetSetSongName)
            {
                musicTitleObj.GetComponent<Image>().sprite = MusicTitleSprite[i];
            }
        }

        musicTitleObj.GetComponent<Image>().SetNativeSize();

        CountTrigger(scoreCount, score, 1f, 0.5f);
    }

    private void Update()
    {
        CountAnimation(scoreCount, scoreText);

        CountAnimation(greatCount, greatText);

        CountAnimation(goodCount, goodText);

        CountAnimation(badCount, badText);

        CountAnimation(comboCount, comboText);


    }
    public void SetText(int combo, int great, int good, int bad, int score, float accuracy)
    {
        this.score = score;
        this.combo = combo;
        this.great = great;
        this.good = good;
        this.bad = bad;
        this.accuracy = accuracy;
        GameObject.Find("accuracy").GetComponent<TextMeshProUGUI>().text = accuracy.ToString() + "%";
    }

    public void PointerDownButton(GameObject obj)
    {
        Color inputColor = new Color(79 / 255f, 79 / 255f, 79 / 255f, 1f);
        obj.GetComponent<Image>().material.DOColor(inputColor, 0.1f).AsyncWaitForCompletion();
    }

    public async void PointerUpButton(GameObject obj)
    {
        Color upColor = new Color(135 / 255f, 135 / 255f, 135 / 255f, 1f);
        await obj.GetComponent<Image>().material.DOColor(upColor, 0.05f).AsyncWaitForCompletion();
        await obj.GetComponent<Image>().material.DOColor(color, 0.4f).AsyncWaitForCompletion();

        if (obj.name == "reStart")
        {
            reStartEvent.Invoke();
        }
        else if (obj.name == "next")
        {
            nextEvent.Invoke();
        }
    }

    private void ResetResult()
    {
        greatText.GetComponent<TextMeshProUGUI>().text = "0";
        goodText.GetComponent<TextMeshProUGUI>().text = "0";
        badText.GetComponent<TextMeshProUGUI>().text = "0";
        comboText.GetComponent<TextMeshProUGUI>().text = "0";
        scoreText.text = "0";
        GameObject.Find("accuracy").GetComponent<TextMeshProUGUI>().text = "0%";

        score = 0;
        combo = 0;
        great = 0;
        good = 0;
        bad = 0;
        accuracy = 0;
        isHiScore = false;
        hiScore.SetActive(false);
        accuracyObj.GetComponent<CanvasGroup>().alpha = 0;
        restartButton.SetActive(false);
        nextButton.SetActive(false);
    }

    private void CountAnimation(Count count, TextMeshProUGUI text)
    {

        if (count.IsWorking())
        {
            int value = (int)Mathf.Ceil(count.Value);
            text.text = value.ToString();
        }
    }

    private async void CountTrigger(Count count, int num, float time, float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        count.CountToInt(0, num, time);
    }

    private async void Animation(GameObject obj)
    {
        await obj.transform.DOScaleY(1.3f, 0.2f).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
        await obj.transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
    }

    public void OnFinish(string obj)
    {
        if (obj == "Score")
        {
            Animation(scoreObj);
            CountTrigger(greatCount, great, 0.5f, 0.3f);
        }
        else if (obj == "great_num")
        {
            Animation(greatObj);
            CountTrigger(goodCount, good, 0.5f, 0.1f);

        }
        else if (obj == "good_num")
        {
            Animation(goodObj);
            CountTrigger(badCount, bad, 0.5f, 0.1f);
        }
        else if (obj == "bad_num")
        {
            Animation(badObj);
            CountTrigger(comboCount, combo, 0.5f, 0.1f);
        }
        else if (obj == "combo_num")
        {
            Animation(comboObj);
            accuracyObj.GetComponent<RectTransform>().DOAnchorPosX(-404, 1f).SetRelative(true).SetEase(Ease.OutQuart);
            accuracyObj.GetComponent<CanvasGroup>().DOFade(1, 1.5f).SetEase(Ease.OutQuart);

            restartButton.SetActive(true);
            nextButton.SetActive(true);

            if (isHiScore == true)
            {
                HiScoreAnimation();

            }
        }
    }

    public void HiScoreAnimation()
    {
        hiScore.SetActive(true);
        hiScore.GetComponent<RectTransform>().DOLocalMoveY(20f, 0.4f)
        .SetRelative(true)
        .SetEase(Ease.OutQuad)
        .SetLoops(-1, LoopType.Yoyo);
    }
}
