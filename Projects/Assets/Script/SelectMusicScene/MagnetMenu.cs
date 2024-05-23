using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class MagnetMenu : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler

{
    //曲スクロール部分の動きを制御するクラス
    SelectMusicManager selectMusicManager;
    // 吸いつく位置の中心座標
    [SerializeField] private Vector2 magnetPosition;

    [SerializeField] GameObject[] musicTitle;

    // 等間隔に並べる要素と要素の間隔
    [SerializeField] private float itemDistance;
    // 制御対象の子要素
    [SerializeField] List<RectTransform> items;
    // 初期表示したときに中央に表示する値
    [SerializeField] int centerElemIndex;
    [SerializeField] private GameObject selectItem;

    // アニメーション中かどうかのフラグ
    // true : 実行中 / false : それ以外
    [SerializeField] private bool isDragging;

    [SerializeField] private GameObject startButton;

    private void Awake()
    {
        this.updateItemsScale();

        for (int i = 0; i < items.Count; i++)
        {
            if (i < musicTitle.Length)
            {
                //itemの子要素にmusicTitle[i]を生成
                GameObject obj = Instantiate(musicTitle[i], items[i].transform);
                obj.name = obj.name.Replace("(Clone)", "");
            }

            else if (i >= musicTitle.Length)
            {
                //itemの子要素にmusicTitle[i]を生成
                GameObject obj = Instantiate(musicTitle[i - musicTitle.Length], items[i].transform);
                obj.name = obj.name.Replace("(Clone)", "");
            }

        }

    }

    private void OnDestroy()
    {
        this.tweenList.KillAllAndClear();
    }

    private void Start()
    {
        selectMusicManager = GameObject.Find("SelectMusic").GetComponent<SelectMusicManager>();
        // 初期表示したときに中央に表示する値を設定
        float centerPosX = this.magnetPosition.x;
        centerPosX -= this.itemDistance * this.centerElemIndex;
        foreach (var item in this.items)
        {
            RectTransform rect = item;
            var pos = rect.anchoredPosition;
            pos.x = centerPosX;
            rect.anchoredPosition = pos;
            centerPosX += this.itemDistance;
        }

        selectItem = items[centerElemIndex].gameObject;
        selectMusicManager.GetSetMusicTitle = selectItem.transform.GetChild(0).name;

        updateItemsScale();
        ResetPos();

    }

    private void Update()
    {
        if (!this.isDragging)
        {
            return;
        }
        this.updateItemsScale();
    }

    private void updateItemsScale()
    {
        foreach (var item in this.items)
        {
            float distance = Mathf.Abs(item.GetAnchoredPosX());
            float scale = Mathf.Clamp(1.0f - distance / (170.0f * 4.0f), 0.65f, 1.0f);
            item.SetLocalScaleXY(scale);
        }
    }

    public void OnDrag(PointerEventData e)
    {
        // 操作量に応じてX方向に移動する
        float delta_x = e.delta.x;

        ResetPos();


        foreach (var item in this.items)
        {
            RectTransform rect = item;
            var pos = rect.anchoredPosition;
            pos.x += delta_x;
            rect.anchoredPosition = pos;

        }

    }

    public void OnBeginDrag(PointerEventData e)
    {
        this.isDragging = true;
        this.tweenList.KillAllAndClear();
        startButton.SetActive(false);
    }

    public void OnEndDrag(PointerEventData e)
    {
        // 移動目標量を計算
        RectTransform rect = this.pickupNearestRect();
        float tartgetX = -rect.GetAnchoredPosX();

        for (int i = 0; i < this.items.Count; i++)
        {
            RectTransform item = this.items[i];

            Tween t =
                item.DOAnchorPosX(item.GetAnchoredPosX()
                    + tartgetX, 0.3f).SetEase(Ease.OutSine);
            if (i <= this.items.Count)
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(t);
                seq.AppendCallback(this.onCompleted);
                this.tweenList.Add(seq);
            }
            else
            {
                this.tweenList.Add(t);
            }
        }

        startButton.SetActive(true);
    }

    private void onCompleted() => this.isDragging = false;
    private List<Tween> tweenList = new List<Tween>();

    // マグネット中心に最も近い要素を選択する
    private RectTransform pickupNearestRect()
    {
        RectTransform nearestRect = null;
        foreach (var rect in this.items)
        {
            if (nearestRect == null)
            {
                nearestRect = rect; // 初回選択
            }
            else
            {
                if (Mathf.Abs(rect.GetAnchoredPosX())
                    < Mathf.Abs(nearestRect.GetAnchoredPosX()))
                {
                    nearestRect = rect; // より中心に近いほうを選択
                }
            }
        }
        selectItem = nearestRect.gameObject;
        selectMusicManager.GetSetMusicTitle = selectItem.transform.GetChild(0).name;

        // VibrationMng.ShortVibration(); //スマホのバイブレーションを鳴らす(PCの場合はコメントアウト)
        return nearestRect;
    }

    Transform childObj;

    private void ResetPos()
    {
        if (items[0].GetAnchoredPosX() < -500f)
        {
            // items[0] の RectTransform を一時保管
            RectTransform tempRect = items[0];
            // リストから items[0] を削除
            items.RemoveAt(0);
            // items[5] と 30 の間隔を開けて、一時保管しておいた RectTransform を移動
            tempRect.anchoredPosition = new Vector2(items[5].GetAnchoredPosX() + itemDistance, tempRect.anchoredPosition.y);
            // 一時保管しておいた RectTransform を新たにリストに追加
            items.Add(tempRect);

            SetMusicTitle(5, tempRect);
        }

        if (items[6].GetAnchoredPosX() > 500f)
        {
            // items[6] の RectTransform を一時保管
            RectTransform tempRect = items[6];
            // リストから items[6] を削除
            items.RemoveAt(6);
            // items[0] と間隔を開けて、一時保管しておいた RectTransform を移動
            tempRect.anchoredPosition = new Vector2(items[0].GetAnchoredPosX() - itemDistance, tempRect.anchoredPosition.y);
            // 一時保管しておいた RectTransform をitems[0]の前にリストに追加
            items.Insert(0, tempRect);
            SetMusicTitle(1, tempRect);
        }
    }

    private void SetMusicTitle(int compareNum, RectTransform tempRect)
    {
        foreach (Transform child in tempRect.transform)
        {
            Destroy(child.gameObject);
        }

        int i = 0;  // musicTitleのインデックス

        // items[5]の一個下の子要素に対して調査
        childObj = items[compareNum].GetChild(0);  // 一個下の子要素を取得

        // musicTitleの各要素と比較
        while (i < musicTitle.Length)
        {
            if (childObj.name == musicTitle[i].name)
            {
                // 一致する要素が見つかった場合
                break;
            }

            i++;  // 次のmusicTitle要素を調べるためにインデックスを増やす
        }

        if (compareNum == 5)
        {
            if (i < musicTitle.Length - 1)
            {
                //itemの子要素にmusicTitle[i]を生成
                GameObject obj = Instantiate(musicTitle[i + 1], items[compareNum + 1].transform);
                obj.name = obj.name.Replace("(Clone)", "");
            }

            else if (i == musicTitle.Length - 1)
            {
                //itemの子要素にmusicTitle[i]を生成
                GameObject obj = Instantiate(musicTitle[i - (musicTitle.Length - 1)], items[compareNum + 1].transform);
                obj.name = obj.name.Replace("(Clone)", "");

            }
        }

        else if (compareNum == 1)
        {
            if (i > 0 && compareNum <= musicTitle.Length - 1)
            {
                //itemの子要素にmusicTitle[i]を生成
                GameObject obj = Instantiate(musicTitle[i - 1], items[0].transform);
                obj.name = obj.name.Replace("(Clone)", "");
            }

            else if (i == 0)
            {
                //itemの子要素にmusicTitle[i]を生成
                GameObject obj = Instantiate(musicTitle[musicTitle.Length - 1], items[0].transform);
                obj.name = obj.name.Replace("(Clone)", "");

            }
        }
    }


}

public static class List_Tween_Extension
{
    // リスト内のすべてのアニメーションを停止する
    public static void KillAllAndClear(this List<Tween> self)
    {
        self.ForEach(tween => tween.Kill());
        self.Clear();
    }
}

public static class RectTransformExtension
{
    // Xのアンカー位置を取得する
    public static float GetAnchoredPosX(this RectTransform self)
    {
        return self.anchoredPosition.x;
    }
    // オブジェクトの拡大率の設定
    public static void SetLocalScaleXY(this Transform self, float xy)
    {
        Vector3 scale = self.localScale;
        scale.x = xy;
        scale.y = xy;
        self.localScale = scale;
    }
}


