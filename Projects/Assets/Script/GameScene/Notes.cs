using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    //ノーツを動かすクラス

    float NoteSpeed;
    bool isPlay = false;

    void Start()
    {
        NoteSpeed = GameManager.instance.GetNoteSpeed;
    }
    void Update()
    {
        if (GameManager.instance.GetSetStart == true && GameManager.instance.GetSetPause == false)
        {
            if (isPlay == false)
            {
                isPlay = true;
            }
            transform.position -= transform.up * Time.deltaTime * NoteSpeed;
        }
    }
}
