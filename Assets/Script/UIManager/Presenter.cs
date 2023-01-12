
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
