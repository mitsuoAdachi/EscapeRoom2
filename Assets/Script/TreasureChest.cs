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
    public ObservableEventTrigger OpenChestTrigger { get => openChestTrigger; }

    private SingleAssignmentDisposable chestDispose = new();
    public SingleAssignmentDisposable ChestDispose { get => chestDispose; }

    [SerializeField]
    private ObservableEventTrigger treasureChestItemTrigger;
    public ObservableEventTrigger TreasureChestItemTrigger { get => treasureChestItemTrigger; }

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
