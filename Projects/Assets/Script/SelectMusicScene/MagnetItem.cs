using UnityEngine;

// 制御対象の子要素を表すコンポーネント
public class MagnetItem : MonoBehaviour
{
    //制御できる子要素を明示するために定義する
    public RectTransform RectTransform => this.transform as RectTransform;
}
