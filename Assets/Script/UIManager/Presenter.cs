
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// Modelでの変更要素をViewへ伝えるためのハブとなるクラス
/// Hierarchy:"UIManager"にアタッチ
/// </summary>
public class Presenter : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private View view;

    [SerializeField]
    private BoxCollider stopCollider;

    [SerializeField]
    private ObservableEventTrigger centerImageTrigger;

    [SerializeField]
    private ObservableEventTrigger queriSwitchTrigger;
    public ObservableEventTrigger QueriSwitchTrigger { get => queriSwitchTrigger; }

    [SerializeField]
    private ObservableEventTrigger[] btnTriggers;
    [SerializeField]
    private ObservableEventTrigger[] sliderTriggers;

    [SerializeField]
    private ObservableEventTrigger[] changeCameraTriggers;
    public ObservableEventTrigger[] ChangeCameraTriggers { get => changeCameraTriggers; }

    [SerializeField]
    public List<ObservableEventTrigger> telopTriggerList = new List<ObservableEventTrigger>();

    private SingleAssignmentDisposable queriDisposable = new SingleAssignmentDisposable();
    public SingleAssignmentDisposable QueriDisposable { get => queriDisposable; }

    private SingleAssignmentDisposable doorDisposable = new SingleAssignmentDisposable();
    public SingleAssignmentDisposable DoorDisposable { get => doorDisposable; }

    [SerializeField]
    private HintRoboController hintRobo;

    void Start()
    {
        ReflectNumberBoard();
        ReflectSliderBoard();
        ReflectDisplayImageSprite();
        ReflectQueriSwitchText();
        ReflectHintRoboTelop();

        ClickChangeCamera();
        ClickReturnCamera();
        ClickTelopText();
        ClickDisCenterImage();

        CollisionStopCollider();
    }

    /// <summary>
    /// NumberBoard内の各オブジェクトのクリック時にモデルでの数値の変化をViewクラスへ伝える
    /// </summary>
    private void ReflectNumberBoard()
    {
        for (int i = 0; i < btnTriggers.Length; i++)
        {
            int index = i;

            btnTriggers[index].OnPointerDownAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => uiManager.ChangeNumberModel(index))
                .AddTo(this);

            uiManager.displayNumbers[index].Subscribe(x => view.ViewNumber(index, x))
                .AddTo(this);
        }

        //ついでにUIManagerへPresenterクラスを渡す
        uiManager.SetupUIManager2(this);
    }

    /// <summary>
    /// 各スライダー(ClickTrigger)をクリック時にモデルでの変化を反映する
    /// </summary>
    private void ReflectSliderBoard()
    {
        for (int i = 0; i < sliderTriggers.Length; i++)
        {
            int index = i;

            sliderTriggers[index].OnPointerDownAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => uiManager.ChangeSliderValueModel(index))
                .AddTo(this);
        }
    }

    /// <summary>
    /// centerImageのスプライトの変更をViewに反映させる
    /// </summary>
    private void ReflectDisplayImageSprite()
    {
        uiManager.displayItemSprite.Subscribe(x => view.ViewDisplayItemImage(x))
            .AddTo(this);
    }

    /// <summary>
    /// txtQueriSwitchの変更をViewに反映させる
    /// </summary>
    private void ReflectQueriSwitchText()
    {
        uiManager.txtQueriSwitch.Subscribe(x => view.ViewQueriSwitchText(x))
            .AddTo(this);
    }

    /// <summary>
    /// ヒントロボのテキスト情報をViewに反映させる
    /// </summary>
    private void ReflectHintRoboTelop()
    {
        hintRobo.HintTelop.Subscribe(x => view.ViewHintTelop(x))
            .AddTo(this);
    }

    /// <summary>
    /// クリック時に任意のVirtualCameraの優先順位を変更する処理を反映する
    /// </summary>
    private void ClickChangeCamera()
    {
        for (int i = 0; i < changeCameraTriggers.Length; i++)
        {
            int index = i;

            changeCameraTriggers[index].OnPointerDownAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => uiManager.ChangeCamera(index))
                .AddTo(this);
        }
    }

    /// <summary>
    /// クリック時にカメラをCenterCameraに戻す処理を反映(いろんな機能を試すためにあえてButtonメソッドで
    /// </summary>
    private void ClickReturnCamera()
    {
        uiManager.BtnReturn.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => uiManager.ReturnCamera())
            .AddTo(this);
    }

    /// <summary>
    /// 特定のオブジェクトをクリック時にテロップを表示する
    /// </summary>
    private void ClickTelopText()
    {
        //棚上クエリちゃんテロップを設定
        queriDisposable.Disposable = telopTriggerList[0].OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => uiManager.DisplayTelopModel(0,3))
            .AddTo(this);

        //ドアのテロップを設定。RunTime中にイベント内容を変更するためDisposable型の変数に代入しておく
        doorDisposable.Disposable = telopTriggerList[1].OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => uiManager.DisplayTelopModel(1, 3))
            .AddTo(this);
    }

    /// <summary>
    /// 鍵を手に入れた後のドアをクリックした時のテロップ
    /// </summary>
    public void ClickDoor()
    {
        telopTriggerList[1].OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => uiManager.DisplayTelopModel(10, 3))
            .AddTo(this);
    }

    /// <summary>
    /// centerImage表示時、クリックすると非表示になる
    /// </summary>
    private void ClickDisCenterImage()
    {
        centerImageTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => uiManager.DisCenterImage())
            .AddTo(this);
    }

    /// <summary>
    /// クエリちゃんロゴをクリック時に操作を切り替える
    /// </summary>
    public void ClickSwitchQueriControl()
    {
        queriSwitchTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(2))
            .Subscribe(_ => uiManager.SwitchQueriController())
            .AddTo(this);
    }

    /// <summary>
    /// 条件をクリアしていない場合テロップで注記を出す
    /// </summary>
    private void CollisionStopCollider()
    {
        stopCollider.OnCollisionEnterAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(4))
            .Subscribe(_ => uiManager.DisplayTelopModel(5,4))
            .AddTo(this);     
    }
}
