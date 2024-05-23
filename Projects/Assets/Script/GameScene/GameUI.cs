using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    //ゲーム中のUIを管理するクラス

    private GameObject comboText;
    private GameObject comboObj;
    private GameObject Panel;

    private GameObject PauseCanvas;

    private GameObject countGroup;

    [SerializeField] UnityEvent PauseEvent;
    [SerializeField] UnityEvent UnPauseEvent;

    [SerializeField] private Sprite[] instrumentSprite;

    Color color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 1f);
    private void Awake()
    {
        comboText = GameObject.Find("ComboNum");
        comboObj = GameObject.Find("Combo");
        Panel = GameObject.Find("Panel");
        PauseCanvas = GameObject.Find("PauseCanvas");
        countGroup = GameObject.Find("CountGroup");
        comboText.SetActive(false);
        comboObj.SetActive(false);
        Panel.SetActive(false);
        PauseCanvas.SetActive(false);
        countGroup.SetActive(false);
    }

    void Start()
    {
        //選択した楽器によって画像を変更
        if (GameManager.instance.GetSetInstrument == "tuke")
        {
            GameObject.Find("Instrument").GetComponent<Image>().sprite = instrumentSprite[0];
        }
        else if (GameManager.instance.GetSetInstrument == "ookan")
        {
            GameObject.Find("Instrument").GetComponent<Image>().sprite = instrumentSprite[1];
        }
        else if (GameManager.instance.GetSetInstrument == "kane")
        {
            GameObject.Find("Instrument").GetComponent<Image>().sprite = instrumentSprite[2];
        }

        GameObject.Find("Instrument").GetComponent<Image>().SetNativeSize();

    }



    public void SetText(int combo)
    {
        //10コンボ以上の時のみ表示
        if (combo < 10)
        {
            comboText.SetActive(false);
            comboObj.SetActive(false);
        }
        else
        {
            comboText.SetActive(true);
            comboText.GetComponent<TextMeshProUGUI>().text = combo.ToString();
            comboObj.SetActive(true);
        }
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

        if (obj.name == "StopButton")
        {
            Pause();
        }

        if (obj.name == "restart")
        {
            GameManager.instance.GetSetStart = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (obj.name == "backTitle")
        {
            GameManager.instance.GetSetStart = false;
            SceneManager.LoadScene("SelectMusicScene");
        }

        if (obj.name == "back")
        {
            UnPause();
        }
    }

    void Pause()
    {
        Panel.SetActive(true);
        PauseCanvas.SetActive(true);
        GameManager.instance.GetSetPause = true;
        PauseEvent.Invoke(); //MusicManagerのPauseメソッドを呼ぶ
    }

    async void UnPause()
    {
        Panel.SetActive(false);
        PauseCanvas.SetActive(false);
        countGroup.SetActive(true);
        TextMeshProUGUI countText = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
        //3秒カウントダウン
        for (int i = 3; i > 0; i--)
        {
            countText.text = i.ToString();
            countGroup.GetComponent<CanvasGroup>().alpha = 1;
            Debug.Log(i);
            await UniTask.Delay(700);
            await countGroup.GetComponent<CanvasGroup>().DOFade(0, 0.3f).AsyncWaitForCompletion();
        }
        countGroup.SetActive(false);
        GameManager.instance.GetSetPause = false;
        UnPauseEvent.Invoke(); //MusicManagerのUnPauseメソッドを呼ぶ
    }
}
