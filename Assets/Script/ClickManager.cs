using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <summary>
/// クリックイベントを管理
/// Hierarchy -> ClickManagerにアタッチ
/// </summary>
public class ClickManager : MonoBehaviour
{
    [SerializeField]
    public List<ObservableEventTrigger> telopTriggerList = new List<ObservableEventTrigger>();

    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private ItemManager itemManager;
    [SerializeField]
    private Generator generator;
    [SerializeField]
    private HintRoboController hintRobo;
    [SerializeField]
    private ChestOpen chestOpen;

    [SerializeField]
    private TreasureChest treasureChest;

    private QueriChanController queriPink;

    [SerializeField]
    private ObservableEventTrigger roboTrigger;

    [SerializeField]
    private ObservableEventTrigger centerImageTrigger;

    [SerializeField]
    private ObservableEventTrigger[] drawers;
    public ObservableEventTrigger[] Drawers { get => drawers; }

    [SerializeField]
    private ObservableEventTrigger queriLogoTrigger;
    public ObservableEventTrigger QueriLogoTrigger { get => queriLogoTrigger; }

    [SerializeField]
    private ObservableEventTrigger[] changeCameraTriggers;
    public ObservableEventTrigger[] ChangeCameraTriggers { get => changeCameraTriggers; }

    private SingleAssignmentDisposable treasureThestDispose = new();
    public SingleAssignmentDisposable ChestDispose { get => treasureThestDispose; }


    private SingleAssignmentDisposable queriDisposable = new SingleAssignmentDisposable();
    public SingleAssignmentDisposable QueriDisposable { get => queriDisposable; }

    private SingleAssignmentDisposable doorDisposable = new SingleAssignmentDisposable();
    public SingleAssignmentDisposable DoorDisposable { get => doorDisposable; }


    // Start is called before the first frame update
    void Start()
    {
        ClickChangeCamera();
        ClickReturnCamera();
        ClickTelopText();
        ClickDisCenterImage();
        ClickRobo();
        ClickOpenChest();
        ClickResetPanel();
        ClickItemFolder();
    }


    //Start()で実行↓

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
            .Subscribe(_ => uiManager.DisplayTelopModel(0, 3))
            .AddTo(this);

        //ドアのテロップを設定。RunTime中にイベント内容を変更するためDisposable型の変数に代入しておく
        doorDisposable.Disposable = telopTriggerList[1].OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => uiManager.DisplayTelopModel(1, 3))
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
    /// ヒントロボをクリック時にHint()を呼び出す
    /// </summary>
    private void ClickRobo()
    {
        roboTrigger.OnPointerClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(3))
            .Subscribe(_ => hintRobo.Hint())
            .AddTo(this);
    }

    /// <summary>
    /// 引き出しをクリック時にMovableDrawerを呼び出す
    /// </summary>
    private void ClickOpenChest()
    {
        for (int i = 0; i < drawers.Length; i++)
        {
            int index = i;

            drawers[index].OnPointerDownAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => chestOpen.MovableDrawer(index))
                .AddTo(this);
        }
    }

    /// <summary>
    /// クリックで元に戻る(カメラ・ポーズ)処理の登録
    /// </summary>
    private void ClickResetPanel()
    {
        itemManager.ResetPanel.OnPointerDownAsObservable()
            .Subscribe(_ => itemManager.DisplayResetPanel());
    }

    /// <summary>
    /// アイテムフォルダをクリック時にReadyUseItemメソッドを呼び出す
    /// </summary>
    public void ClickItemFolder()
    {
        for (int i = 0; i < itemManager.ItemFolderTriggers.Length; i++)
        {
            int index = i;

            itemManager.ItemFolderTriggers[index].OnPointerDownAsObservable()
                        .ThrottleFirst(TimeSpan.FromSeconds(1))
                        .Subscribe(_ => itemManager.ReadyUseItem(index))
                        .AddTo(this);
        }
    }


    //ゲーム途中から実行↓


    /// <summary>
    /// 宝箱出現時、TreasureChestクラスを取得する
    /// </summary>
    /// <param name="treasureChest"></param>
    public void SetupClickManager1(TreasureChest treasureChest)
    {
        this.treasureChest = treasureChest;
    }

    /// <summary>
    /// queriHedgear(アイテム)をドロップ時、PinkクエリちゃんのQueriChanControllerを取得する
    /// </summary>
    /// <param name="queriPink"></param>
    public void SetupClickManager2(QueriChanController queriPink)
    {
        this.queriPink = queriPink;
    }

    /// <summary>
    /// 家出現後、クリック時にテロップを表示する
    /// </summary>
    public void ClickHouse()
    {
        generator.HouseTrigger.OnPointerDownAsObservable()
            .Subscribe(_ => uiManager.DisplayTelopModel(8, 3));
    }

    /// <summary>
    /// 宝箱出現後、宝箱をクリック時にテロップを出す
    /// </summary>
    public void ClickTreasureChest1()
    {
        //RunTime時にクリックイベント内容を変更するためデリゲートしておく
        treasureThestDispose.Disposable = treasureChest.OpenChestTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(3))
            .Subscribe(_ => uiManager.DisplayTelopModel(11, 3))
            .AddTo(this);
    }

    /// <summary>
    /// 宝箱出現後、宝箱から出現したアイテムをクリックで取得する
    /// </summary>
    public void ClickTreasureChestItem()
    {
        treasureChest.TreasureChestItemTrigger.OnPointerDownAsObservable()
            .Subscribe(_ => itemManager.GetItem(treasureChest.TreasureChestItemTrigger));
    }


    /// <summary>
    /// queriHedgear(アイテム)をドロップ後、クリック時にテロップを表示する
    /// </summary>
    public void ClickQueriPinkTelop()
    {
        queriPink.QueriPinkTelopTrigger.OnPointerDownAsObservable()
            .Subscribe(_ => uiManager.DisplayTelopModel(7, 3))
            .AddTo(this);
    }

    /// <summary>
    /// queriHedgear(アイテム)をドロップ後、Movableクエリちゃんをクリック時にUseItem()メソッドを呼び出す
    /// </summary>
    public void ClickMovableQueriChan()
    {
        QueriDisposable.Dispose();

        itemManager.QueriMovableTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => itemManager.UseItem())
            .AddTo(this);
    }

    /// <summary>
    /// カギ出現後、宝箱をクリックした時にUseItem()を呼び出す
    /// </summary>
    public void ClickTreasureChest2()
    {
        treasureChest.OpenChestTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(4))
            .Subscribe(_ => itemManager.UseItem())
            .AddTo(this);
    }

    /// <summary>
    /// カギ出現後、ドアをクリックした時のテロップを切り替える
    /// </summary>
    public void ClickDoor()
    {
        telopTriggerList[1].OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ => uiManager.DisplayTelopModel(10, 3))
            .AddTo(this);
    }

    /// <summary>
    /// Movableクエリちゃん起動後クエリロゴをクリックでPlayerと操作を切り替える
    /// </summary>
    public void ClickQueriController()
    {
        queriLogoTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(2))
            .Subscribe(_ => uiManager.SwitchQueriController())
            .AddTo(this);
    }
}