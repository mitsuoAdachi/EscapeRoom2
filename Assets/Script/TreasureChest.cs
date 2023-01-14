using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class TreasureChest : MonoBehaviour
{
    [SerializeField]
    private ObservableEventTrigger openChestTrigger;

    private SingleAssignmentDisposable chestDispose = new();
    public SingleAssignmentDisposable ChestDispose { get => chestDispose; }

    [SerializeField]
    private ObservableEventTrigger treasureChestItemTrigger;

    private UIManager uiManager;
    private ItemManager itemManager;
    private AudioManager audioManager;

    [SerializeField]
    private Animator openChest;

    [SerializeField]
    private ParticleSystem chestParticle;
    [SerializeField]
    private ParticleSystem gunParticle;

    [SerializeField]
    private GameObject newItem;


    /// <summary>
    /// 各コンポーネントを取得
    /// </summary>
    /// <param name="itemManager"></param>
    /// <param name="audioManager"></param>
    public void SetupTresureChest(UIManager uiManager,ItemManager itemManager,AudioManager audioManager)
    {
        this.uiManager = uiManager;
        this.itemManager = itemManager;
        this.audioManager = audioManager;

        ClickTreasureChest1();
        ClickTreasureChestItem();
    }

    /// <summary>
    /// 宝箱をクリック時にテロップを出す
    /// </summary>
    private void ClickTreasureChest1()
    {
        //RunTime時にクリックイベント内容を変更するためデリゲートしておく
        chestDispose.Disposable = openChestTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(3))
            .Subscribe(_ => uiManager.DisplayTelopModel(11,3))
            .AddTo(this);
    }

    /// <summary>
    /// 宝箱をクリックした時にOpenTreasureChest()を呼び出す
    /// </summary>
    public void ClickTreasureChest2()
    {
        openChestTrigger.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(4))
            .Subscribe(_ => itemManager.UseItem())
            .AddTo(this);
    }

    /// <summary>
    /// 宝箱から出現したアイテムをクリックで取得する
    /// </summary>
    private void ClickTreasureChestItem()
    {
        treasureChestItemTrigger.OnPointerDownAsObservable()
            .Subscribe(_ => itemManager.GetItem(treasureChestItemTrigger));
    }

    /// <summary>
    /// 宝箱オープン、アイテム出現
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenTreasureChest()
    {
        if(!itemManager.TreasureChestKey_UseReady) yield break;

        //エフェクト演出
        chestParticle.Play();

        audioManager.PlaySE(12);

        yield return new WaitForSeconds(1);

        //宝箱を開く
        openChest.SetTrigger("open");

        yield return new WaitForSeconds(2);

        gunParticle.Play();

        //アイテムが飛び出し回転する
        var tweener1 = newItem.transform.DOLocalMoveY(0.7f, 0.5f);
        var tweener2 = newItem.transform.DORotate(new Vector3(0, -360, 0), 8f);

        DOTween.Sequence().Append(tweener1).Append(tweener2);
    }

}
