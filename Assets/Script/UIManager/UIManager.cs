using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cinemachine;
using DG.Tweening;

/// <summary>
/// UIにまつわる機能を管理するクラス
/// Hierarchy:"UIManager"にアタッチ
/// </summary>
public class UIManager : MonoBehaviour
{
    private Tween tween;

    private GameManager gameManager;
    private AudioManager audioManager;
    private ItemManager itemManager;
    private Presenter presenter;

    [SerializeField]
    private CameraManager camManager;

    public IntReactiveProperty[] displayNumbers;
    public StringReactiveProperty[] displayTelops;

    public ReactiveProperty<string> txtQueriSwitch = new ReactiveProperty<string>();
    [SerializeField]
    private bool queriswitch;

    [SerializeField]
    private QueriChanController queriController;

    [SerializeField]
    private Text txtTelop;
    public Text TxtTelop { get => txtTelop; }

    public ReactiveProperty<Sprite> displayItemSprite;
    [SerializeField]
    private Image imgCenter;
    public Image ImagCenter { get => imgCenter; }

    [SerializeField]
    private Button btnReturn;
    public　Button BtnReturn { get => btnReturn; }

    [SerializeField]
    private Slider[] sliders;
    public Slider[] Sliders { get => sliders; }

    [SerializeField]
    private GameObject player;
    public GameObject Player { get => player; }

    /// <summary>
    /// テロップ表示で使用する為にItemManagerクラスを事前に取得する
    /// </summary>
    /// <param name="itemManager"></param>
    public void SetupUIManager1(ItemManager itemManager)
    {
        this.itemManager = itemManager;
    }

    /// <summary>
    /// Presenterクラスを取得する
    /// </summary>
    /// <param name="presenter"></param>
    public void SetupUIManager2(Presenter presenter)
    {
        this.presenter = presenter;
    }

    /// <summary>
    /// NumberBoardの数値の変化を設定
    /// </summary>
    /// <param name="index"></param>
    public void ChangeNumberModel(int index)
    {
        displayNumbers[index].Value += 1;

        if(displayNumbers[index].Value > 9)
        {
            displayNumbers[index].Value = 0;
        }

        audioManager.PlaySE(3);

        gameManager.JudgeNumberBoard();
        gameManager.JudgeNumberBoard_2();
    }

    /// <summary>
    /// スライダーをクリックした時の設定
    /// </summary>
    /// <param name="index"></param>
    public void ChangeSliderValueModel(int index)
    {
        sliders[index].value += 1;

        if(sliders[index].value > 5)
        {
            sliders[index].value = 0;
        }

        audioManager.PlaySE(4);

        gameManager.JudgeSliderBoard();
        gameManager.JudgeSliderBoard_2();
    }

    /// <summary>
    /// テロップを表示する
    /// </summary>
    /// <param name="index"></param>
    public void DisplayTelopModel(int index,int seconds)
    {
        txtTelop.text = displayTelops[index].Value;

        DOVirtual.DelayedCall(seconds, () =>
        {
            txtTelop.text = null;
        });
    }

    /// <summary>
    /// 表示されたアイテムイメージを消す
    /// </summary>
    public void DisCenterImage()
    {
        imgCenter.enabled = false;
        displayItemSprite.Value = null;
    }

    /// <summary>
    /// クエリチャン操作のON/OFF
    /// </summary>
    /// <param name="on_off"></param>
    public void SwitchQueriController()
    {
        if (!queriswitch)
        {
            //クエリちゃんを移動不可に
            queriController.RigidQueri.isKinematic = true;

            //テキストをONに変更
            txtQueriSwitch.Value = "ON";

            //クリック機能をオンにする
            for(int i = 1; i < presenter.telopTriggerList.Count; i++)
            {
                presenter.telopTriggerList[i].enabled = true;
            }
            for (int i = 0; i < presenter.ChangeCameraTriggers.Length; i++)
            {
                presenter.ChangeCameraTriggers[i].enabled = true;
            }

            //カメラを中央に戻すボタンを使用可に
            //btnReturn.enabled = true;
            btnReturn.gameObject.SetActive(true);

            //プレイヤーを非表示
            player.SetActive(false);

            //カメラを中央へ戻す
            camManager.VCams[9].Priority -= 10;

            //BGM切り替え
            audioManager.ChangeBGM(0);

            queriswitch = true;
        }

        else
        {
            //クエリちゃんを移動可に
            queriController.RigidQueri.isKinematic = false;

            //テキストをONに変更
            txtQueriSwitch.Value = "OFF";

            //クリック機能をオフにする
            for (int i = 0; i < presenter.telopTriggerList.Count; i++)
            {
                presenter.telopTriggerList[i].enabled = false;
            }
            for (int i = 0; i < presenter.ChangeCameraTriggers.Length; i++)
            {
                presenter.ChangeCameraTriggers[i].enabled = false;
            }

            //カメラを中央に戻すボタンを使用可に
            btnReturn.gameObject.SetActive(false);

            //プレイヤーを表示
            player.SetActive(true);

            //カメラをクエリちゃんPOVカメラへ戻す
            camManager.VCams[9].Priority += 10;

            //BGM切り替え
            audioManager.ChangeBGM(1);

            queriswitch = false;
        }
    }

    /// <summary>
    /// VirtualCameraの優先順位を調整
    /// </summary>
    /// <param name="index"></param>
    public void ChangeCamera(int index)
    {
        btnReturn.interactable = true;

        tween.Restart();

        camManager.VCams[index].Priority += 10;
    }

    /// <summary>
    /// CenterCameraに戻る処理
    /// </summary>
    public void ReturnCamera()
    {
        btnReturn.interactable = false;

        tween.Pause();

        for(int i = 0; i < camManager.VCams.Length; i++)
        {
            camManager.VCams[i].Priority = 1;
        }
    }

    /// <summary>
    /// UI(ボタン)を点滅させる(gameManagerとAudioManagerコンポーネントをついでに取得する)
    /// </summary>
    /// <param name="btn"></param>
    public void FlashButton(Button btn,GameManager gameManager,AudioManager audioManager)
    {
        this.gameManager = gameManager;
        this.audioManager = audioManager;

        tween = btn.image.DOFade(1, 0.3f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(btn.gameObject);
        tween.Pause();
    }

    /// <summary>
    /// UI(テキスト)を点滅させる
    /// </summary>
    /// <param name="txt"></param>
    public void FlashText(Text txt)
    {
        tween = txt.DOFade(1, 0.3f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetLink(txt.gameObject);
    }
}
