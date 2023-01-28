using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System;

public class HintRoboController : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private UIManager UIManager;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Animator anime;

    [SerializeField]
    private ObservableEventTrigger roboTrigger;

    public  ReactiveProperty<string> HintTelop = new ReactiveProperty<string>();


    /// <summary>
    /// その時に必要なヒントを表示する
    /// </summary>
    public void Hint()
    {
        //素材のAudioClipの再生時間が長いので１秒だけ再生されるようにした
        audioManager.SSEs[24].DOFade(1, 0);
        audioManager.PlaySE(24);
        audioManager.SSEs[24].DOFade(0, 1);

        if (gameManager.Lockings[0])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "部屋にある色の数をかぞえてみよう！";

            DisTelop();

            return;
        }

        if (gameManager.Lockings[1])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "クエリちゃんのジャンプの高さが違うよ！";

            DisTelop();

            return;
        }

        if (gameManager.Lockings[2])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "部屋にある色の数が変わったよ！";

            DisTelop();

            return;
        }

        if (gameManager.Lockings[3])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "ピンクの子がジャンプしなくなったよ！";

            DisTelop();

            return;
        }

        if (!gameManager.Lockings[3])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "宝箱のアイテムでガイコツを倒そう！！";

            DisTelop();
        }
    }

    /// <summary>
    /// ３秒経過でテロップを非表示
    /// </summary>
    private void DisTelop()
    {
        DOVirtual.DelayedCall(3, () =>
        {
            HintTelop.Value = null;
        });
    }
}
