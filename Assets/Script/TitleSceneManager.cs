using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;


/// <summary>
/// タイトルシーン用のクラス。スタートボタンとフェードインの設定に関する処理。
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private ObservableEventTrigger startButton;

    [SerializeField]
    private Image imgTitle;

    [SerializeField]
    private AudioSource startSE;

    [SerializeField]
    private Animator playerAnime;

    [SerializeField]
    private Fade fade;

    [SerializeField]
    private FadeImage fadeImg;


    void Start()
    {
        FlashImage();

        ClickTitle();
    }

    /// <summary>
    /// スタートボタン押下時の処理
    /// </summary>
    /// <returns></returns>
    private async UniTask GameStart()
    {
        startSE.Play();

        ShakeImage();

        await UniTask.Delay(2000);

        playerAnime.SetTrigger("gunshoot");
    }

    /// <summary>
    /// スタートボタンにクリック機能を付与する
    /// </summary>
    private void ClickTitle()
    {
        startButton.OnPointerDownAsObservable()
            .Subscribe(async _ => await GameStart())
            .AddTo(this);          
    }

    /// <summary>
    /// タイトルを点滅させる
    /// </summary>
    /// <param name="txt"></param>
    private void FlashImage()
    {
        imgTitle.DOFade(1, 1)
        .SetLoops(-1, LoopType.Yoyo)
        .SetLink(imgTitle.gameObject);
    }

    /// <summary>
    /// タイトルを揺らす
    /// </summary>
    private void ShakeImage()
    {
        imgTitle.DOFade(1, 0);

        imgTitle.transform.DOShakeScale(1, 2)
        .SetLink(imgTitle.gameObject);
    }

    public void FadeInScreen(Texture texture)
    {
        fadeImg.UpdateMaskTexture(texture);

        fade.FadeIn(1.5f);
    }
}
