using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

/// <summary>
/// 取得アイテムの取得・使用などを管理するクラス
/// Hierarchy:"ItemManager"にアタッチ
/// </summary>
public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private ClickManager click;
    [SerializeField]
    private Generator generator;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private AudioManager audioManager;
    [SerializeField]
    private CameraManager camManager;
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private QueriChanController movableQueri;

    [SerializeField]
    private Text txtQueriLogo;

    [SerializeField]
    private ObservableEventTrigger[] itemFolderTriggers;
    public ObservableEventTrigger[] ItemFolderTriggers { get => itemFolderTriggers; }
    [SerializeField]
    private Image[] itemFolders;

    [SerializeField]
    private GetItemDetail getItemDetailPrfab;
    [SerializeField]
    private Transform[] getItemSpritePrfabeTrans;

    [SerializeField]
    private GameObject getItem;

    [SerializeField]
    private bool queriHeadgear_UseReady;
    public bool QueriHeadgear_UseReady { get => queriHeadgear_UseReady; }
    [SerializeField]
    private bool treasureChestKey_UseReady;
    public bool TreasureChestKey_UseReady { get => treasureChestKey_UseReady; }
    [SerializeField]
    private bool skeletonBuster_UseReady;
    public bool SkeletonBuster_UseReady { get => skeletonBuster_UseReady; }


    [SerializeField]
    private GameObject setQueriHedgear;

    [SerializeField]
    private ObservableEventTrigger queriMovableTrigger;
    public ObservableEventTrigger QueriMovableTrigger { get => queriMovableTrigger; set => queriMovableTrigger = value; }

    [SerializeField]
    private ObservableEventTrigger resetPanel;
    public ObservableEventTrigger ResetPanel { get=>resetPanel; }

    private TreasureChest treasureChest;


    /// <summary>
    /// 各コンポーネントを必要なクラスへ渡す
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="itemDetail"></param>
    public void SetupItemManager1(ItemDetail itemDetail)
    {
        itemDetail.SetupItemDetail(this); 
    }

    public void SetupItemManager2(TreasureChest treasureChest)
    {
        this.treasureChest = treasureChest;
    }

    /// <summary>
    /// アイテムを取得
    /// </summary>
    /// <param name="obj"></param>
    public async UniTask GetItem(ObservableEventTrigger obj)
    {
        //クリックしたアイテムのItemDetailコンポーネントを取得する
        if (obj.gameObject.TryGetComponent(out ItemDetail itemDetail))
        {
            //アイテムタイプで処理を分ける
            if (itemDetail.ItemType == ItemType.sprite)
            {
                GetItemSprite(itemDetail,obj);
            }

            if (itemDetail.ItemType == ItemType.obj)
            {
                await GetItemObj(itemDetail, obj);
            }
        }
    }

    /// <summary>
    /// spriteタイプのアイテムを取得時の処理
    /// </summary>
    /// <param name="itemDetail"></param>
    /// <param name="obj"></param>
    private void GetItemSprite(ItemDetail itemDetail, ObservableEventTrigger obj)
    {
        //取得したアイテムを消す
        obj.gameObject.SetActive(false);

        //表示するアイテムの情報を更新する
        uiManager.displayItemSprite.Value = itemDetail.ItemSprite;

        //画面中央に取得したアイテムを表示する
        uiManager.ImagCenter.enabled = true;

        //該当するテロップを表示する
        uiManager.DisplayTelopModel(itemDetail.ItemNo, 3);

        audioManager.PlaySE(9);

    }
    /// <summary>
    /// objタイプのアイテムを取得時の処理
    /// </summary>
    /// <param name="itemDetail"></param>
    /// <param name="obj"></param>
    private async UniTask GetItemObj(ItemDetail itemDetail, ObservableEventTrigger obj)
    {
        audioManager.PlaySE(9);

        //対象が持つItemDetailクラスへItemManagerクラスを渡す
        itemDetail.SetupItemDetail(this);

        //取得したアイテムを消す
        obj.gameObject.SetActive(false);

        movableQueri.RigidQueri.isKinematic = true;

        //GetItemのポーズ
        movableQueri.Anime.speed = 0.7f;
        movableQueri.Anime.SetTrigger("getItem");

        //GetItemCameraに切り替える
        camManager.VCams[8].Priority += 10;

        await UniTask.Delay(3000);

        audioManager.PlaySE(13);

        //GetItemのポーズのまま維持
        movableQueri.Anime.speed = 0;

        //取得アイテムを表示
        getItem.SetActive(true);

        //表示するアイテムの情報を更新する
        uiManager.displayItemSprite.Value = itemDetail.ItemSprite;

        //該当するテロップを表示する
        uiManager.DisplayTelopModel(itemDetail.ItemNo, 3);

        await UniTask.Delay(3000);

        //クリックで元に戻る(カメラ・ポーズ) resetPanelの表示
        resetPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// クリックで元に戻る(カメラ・ポーズ)
    /// </summary>
    public void DisplayResetPanel()
    {
        getItem.SetActive(false);

        camManager.VCams[8].Priority -= 20;

        movableQueri.Anime.ResetTrigger("getItem");

        movableQueri.Anime.speed = 1;

        movableQueri.RigidQueri.isKinematic = false;

        resetPanel.gameObject.SetActive(false);
    }


    /// <summary>
    /// アイテムを選択時に使用状態を切り替える
    /// </summary>
    public void ReadyUseItem(int index)
    {
        queriHeadgear_UseReady = false;
        treasureChestKey_UseReady = false;

        //アイテムフォルダに何も入っていなかったらreturnする
        if (itemFolders[index].sprite == null)return;

            //選択したアイテムリストの[0]番目のアイテム(queriHeadgear)だったら使用できる状態にboolを切り替える
            if(itemFolders[index].sprite == generator.getItemList[0].ItemSprite)
            {
                queriHeadgear_UseReady = true;
            }
            else queriHeadgear_UseReady = false;


        if (generator.getItemList[1] == null) return;

            //選択したアイテムリストの[1]番目のアイテム(TreasureChestKey)だったら使用できる状態にboolを切り替える
            if (itemFolders[index].sprite == generator.getItemList[1].ItemSprite)
            {
                treasureChestKey_UseReady = true;
            }
            else treasureChestKey_UseReady = false;


        if (generator.getItemList[2] == null) return;

            //選択したアイテムリストの[2]番目のアイテム(skeletonBuster)だったら使用できる状態にboolを切り替える
            if (itemFolders[index].sprite == generator.getItemList[2].ItemSprite)
            {
                skeletonBuster_UseReady = true;
            }
            else skeletonBuster_UseReady = false;

    }

    /// <summary>
    /// アイテムを使用時の処理
    /// </summary>
    public async UniTask UseItem()
    {
        //クエリちゃんコントローラー使用時
        if (queriHeadgear_UseReady)
        {
            audioManager.PlaySE(10);

            setQueriHedgear.SetActive(true);

            DisItemFolder(generator.getItemList[0].ItemSprite);

            //操作をクエリちゃんに切り替える
            await movableQueri.SteeringQueriChan();

            //クエリちゃんロゴにON/OFFのテキストを表示(点滅)
            uiManager.FlashText(txtQueriLogo);

            //クエリちゃんロゴをクリックで操作を切り替える
            click.ClickQueriController();

            uiManager.SetupUIManager2(click);

            queriHeadgear_UseReady = false;
        }

        //鍵を使用時
        if(treasureChestKey_UseReady)
        {
            await treasureChest.OpenTreasureChest();

            DisItemFolder(generator.getItemList[1].ItemSprite);
        }

        //ガイコツバスターを使用時
        if (skeletonBuster_UseReady)
        {
            await player.GunAction();

            DisItemFolder(generator.getItemList[2].ItemSprite);
        }
    }

    /// <summary>
    /// 使用したアイテムをアイテムフォルダから消す
    /// </summary>
    private void DisItemFolder(Sprite disSprite)
    {
        for(int i = 0; i < itemFolders.Length; i++)
        {
            if(itemFolders[i].sprite == disSprite)
            {
                itemFolders[i].sprite = null;
            }
        }
    }
}


/// <summary>
/// 取得できるアイテムをクリック時に呼び出されるメソッド
/// </summary>
/// <param name="obj"></param>
//public void GetItem(ObservableEventTrigger obj)
//{
//    //クリックしたアイテムのItemDetailコンポーネントを取得する
//    if (obj.gameObject.TryGetComponent(out ItemDetail itemDetail))
//    {
//        if (itemDetail.ItemType == ItemType.sprite)
//        {
//            //取得したアイテムを消す
//            obj.gameObject.SetActive(false);

//            //取得したアイテムをアイテムフォルダ欄にImageとして生成する
//            GetItemImage getItemImage = Instantiate(getItemDetailPrfab, getItemSpritePrfabeTrans[itemCounter], false);

//            //生成したImageに取得アイテムのスプライトの情報を要素を追加する
//            getItemImage.SetupGetItemDetail(itemDetail.ItemSprite);

//            //取得したアイテムを数える
//            itemCounter++;

//            //中央に表示するアイテムの情報を更新する
//            uiManager.displayItemSprite.Value = itemDetail.ItemSprite;

//            //画面中央に取得したアイテムを表示する
//            uiManager.ImagCenter.enabled = true;

//            //該当するテロップを表示する
//            uiManager.DisplayTelopModel(itemDetail.ItemNo, 3);

//            audioManager.PlaySE(9);
//        }

//        if (itemDetail.ItemType == ItemType.obj)
//        {
//            //取得したアイテムを消す
//            obj.gameObject.SetActive(false);

//            //表示するアイテムの情報を更新する
//            uiManager.displayItemSprite.Value = itemDetail.ItemSprite;

//            //GetItemCameraに切り替える
//            camManager.VCams[7].Priority += 10;

//            //GetItemのポーズ
//            movableQueri.Anime.SetTrigger("getItem");

//            //該当するテロップを表示する
//            uiManager.DisplayTelopModel(itemDetail.ItemNo, 3);

//            audioManager.PlaySE(9);

//            DOVirtual.DelayedCall(3, () =>
//            {
//                //クリックで元に戻る(カメラ・ポーズ)
//                if (Input.GetMouseButtonDown(0))
//                {
//                    camManager.VCams[7].Priority -= 10;

//                    movableQueri.Anime.ResetTrigger("getItem");
//                }
//            });
//        }
//    }
//}

