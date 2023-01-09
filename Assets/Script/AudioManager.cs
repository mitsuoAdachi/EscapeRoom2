using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 効果音を管理
/// Hierarchy:"AudioManager"にアタッチ
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] BGMs;
    private int bgmNo;

    [SerializeField]
    private AudioSource[] SEs;
    public AudioSource[] SSEs { get => SEs; }


    private void Start()
    {
        PlayBGM(0);
    }

    /// <summary>
    /// BGMを流す
    /// </summary>
    /// <param name="index"></param>
    public void PlayBGM(int index)
    {
        BGMs[index].Play();

        bgmNo = index;
    }

    /// <summary>
    /// BGMを変える
    /// </summary>
    /// <param name="index"></param>
    public void ChangeBGM(int index)
    {
        //今流れているBGMをフェードアウト
        BGMs[bgmNo].DOFade(0, 5);

        //次に流すBGMをフェードイン
        BGMs[index].Play();
        BGMs[index].DOFade(0.5f, 5);

        bgmNo = index;
    }

    /// <summary>
    /// 効果音の操作
    /// </summary>
    /// <param name="index"></param>
    public void PlaySE(int index)
    {
        SEs[index].Play();
    }
}
