using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMusicSound : MonoBehaviour
{
    // 選曲画面の音を管理するクラス
    AudioSource bgmAuidoSource;

    [SerializeField] float bgmVolume = 1.0f;

    void Start()
    {
        bgmAuidoSource = GetComponent<AudioSource>();
        VolumeChange("VolumeUp");
    }


    void VolumeChange(string val)
    {
        StartCoroutine(val);
    }

    IEnumerator VolumeDown()
    {
        while (bgmAuidoSource.volume > 0)
        {
            bgmAuidoSource.volume -= 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator VolumeUp()
    {
        while (bgmAuidoSource.volume < bgmVolume)
        {
            bgmAuidoSource.volume += 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
