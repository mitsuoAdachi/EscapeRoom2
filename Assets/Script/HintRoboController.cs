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
    private Animator anime;

    [SerializeField]
    private ObservableEventTrigger roboTrigger;

    public  ReactiveProperty<string> HintTelop = new ReactiveProperty<string>();

    void Start()
    {
        ClickRobo();
    }

    /// <summary>
    /// ヒントロボをクリック時にHint()を呼び出す
    /// </summary>
    private void ClickRobo()
    {
        roboTrigger.OnPointerClickAsObservable()
            .Subscribe(_ => Hint())
            .AddTo(this);
    }

    /// <summary>
    /// その時に必要なヒントを表示する
    /// </summary>
    private void Hint()
    {
        HintTelop.Value = null;

        if (!gameManager.Locking[3])
            return;

        if (gameManager.Locking[0])
        {
            anime.SetTrigger("hint");

            //HintTelop.Value = "部屋にある色を数えて(細かいところは気にしないで)";
            HintTelop.Value = "部屋にある色を数えて";

            DisTelop();

            return;
        }

        if (gameManager.Locking[1])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "ジャンプの高さが違うよ";

            DisTelop();

            return;
        }

        if (gameManager.Locking[2])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "部屋にある色の数が変わったよ";

            DisTelop();

            return;
        }

        if (gameManager.Locking[3])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "ピンクの子がジャンプしなくなったよ";

            DisTelop();

            return;
        }

        if (!gameManager.Locking[3])
        {
            anime.SetTrigger("hint");

            HintTelop.Value = "宝箱のアイテムでガイコツを倒して！";

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
            UIManager.TxtTelop.text = null;
        });
    }
}
