using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ランタイム時に生成されるオブジェクトに関する処理をするクラス
/// Hierarchy:"GameManager"にアタッチ
/// </summary>
public class Generator : MonoBehaviour
{
    private GameManager gameManager;
    private UIManager uiManager;
    private AudioManager audioManager;

    [SerializeField]
    private CameraManager camManager;
    [SerializeField]
    private ItemManager itemManager;
    [SerializeField]
    private ClickManager click;

    [SerializeField]
    private QueriChanController[] queriChans;
    public List<QueriChanController> queriChanList = new List<QueriChanController>();

    [SerializeField]
    private Transform[] queriTrans;
    [SerializeField]
    private ParticleSystem queriAppearParticle;

    [SerializeField]
    private ObservableEventTrigger housePrefab;
    [SerializeField]
    private Transform houseTran;
    [SerializeField]
    private ParticleSystem houseAppearParticle;
    [SerializeField]
    private Transform houseAppearParTran;
    private ObservableEventTrigger houseTrigger;
    public ObservableEventTrigger HouseTrigger { get => houseTrigger; }

    [SerializeField]
    private TreasureChest tresureChestPrefab;
    [SerializeField]
    private Transform chestTran;

    private TreasureChest tresureChest;

    [SerializeField]
    private ItemDetail queriHeadgearPrefab;
    [SerializeField]
    private Transform queriHeadgearTran;
    [SerializeField]
    private ParticleSystem parDisappear;

    [SerializeField]
    private ItemDetail keyPrefab;
    [SerializeField]
    private Transform keyTran;
    public List<ItemDetail> getItemList = new List<ItemDetail>();


    /// <summary>
    /// クエリちゃんを順番に生成する
    /// </summary>
    /// <returns></returns>
    public IEnumerator GenerateQueriChans(GameManager gameManager,UIManager uiManager,AudioManager audioManager)
    {
        //各クラスを取得しておく
        this.gameManager = gameManager;
        this.uiManager = uiManager;
        this.audioManager = audioManager;

        //UIManagerクラスにItemManagerクラスの情報を渡しておく
        uiManager.SetupUIManager1(itemManager);

        //カメラを切り替える
        camManager.VCams[2].Priority += 10;

        yield return new WaitForSeconds(2);

        for (int i = 0;i < queriChans.Length; i++)
        {
            //クエリちゃんを生成時のエフェクト
            Instantiate(queriAppearParticle, queriTrans[i], false);

            audioManager.PlaySE(0);

            yield return new WaitForSeconds(0.3f);

            //コンポーネント(自作クラス)でインスタンス化する
            QueriChanController queri = Instantiate(queriChans[i], queriTrans[i], false);

            queriChanList.Add(queri);

            //各種コンポーネントを取得
            queri.SetupQueriChanController(uiManager,audioManager);

            queri.Jump();

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2);

        //切り替えたカメラを元のカメラに戻す
        camManager.VCams[2].Priority -= 10;
    }

    /// <summary>
    /// 家プレハブを生成する
    /// </summary>
    /// <returns></returns>
    public IEnumerator GenerateHouse()
    {
        camManager.VCams[5].Priority += 10;

        yield return new WaitForSeconds(2);

        //家を出現するエフェクトを生成
        Instantiate(houseAppearParticle, houseAppearParTran, false);

        yield return new WaitForSeconds(0.1f);

        audioManager.PlaySE(0);

        yield return new WaitForSeconds(0.5f);

        //家を生成
        houseTrigger = Instantiate(housePrefab, houseTran, false);

        //宝箱を生成
        tresureChest = Instantiate(tresureChestPrefab, chestTran, false);

        //TrsureChestコンポーネントを取得する
        itemManager.SetupItemManager2(tresureChest);
        click.SetupClickManager1(tresureChest);

        //TresureChestクラスでItemManagerコンポーネントを取得する
        tresureChest.SetupTresureChest(uiManager,itemManager, audioManager);

        //テロップのON/OFFを切り替えるためのリストに追加
        click.telopTriggerList.Add(houseTrigger);

        //クリック時にテロップを表示する
        click.ClickHouse();
        click.ClickTreasureChest1();
        click.ClickTreasureChestItem();

        yield return new WaitForSeconds(2);

       camManager.VCams[5].Priority -= 10;
    }

    /// <summary>
    /// Query-Chan-Pinkが付けているヘッドギアが消えて足元に落ちる演出
    /// </summary>
    /// <returns></returns>
    public IEnumerator DropQueriChanItem()
    {
        //指定のカメラに切り替える
        camManager.VCams[7].Priority += 10;

        yield return new WaitForSeconds(2);

        //アイテムが消える時のエフェクト
        parDisappear.Play();

        yield return new WaitForSeconds(1);

        //Query-Chan-Pinkの頭の装飾の情報を取得して破棄
        QueriChanController queriCon = queriChanList[2].GetComponent<QueriChanController>();
        queriCon.DestroyQueriHeadgear();

        click.SetupClickManager2(queriCon);

        //Query-Chan-Pinkをクリック時にテロップが出る内容に変更
        queriCon.QueriPinkDispose.Dispose();
        click.ClickQueriPinkTelop();


        //棚上のクエリちゃんのクリックイベントを切り替える
        click.ClickMovableQueriChan();

        //ジャンプできなくなったことによりSliderBoardの正解番号が変わる
        gameManager.ChangeCorrectSlider();

        //ヘッドギアを生成する
        ItemDetail queriHedgear = Instantiate(queriHeadgearPrefab, queriHeadgearTran, false);

        audioManager.PlaySE(5);

        //ItemManagerクラスにヘッドギアのItemDetailクラスを渡す
        itemManager.SetupItemManager1(queriHedgear);

        yield return new WaitForSeconds(2);

        //カメラを切り替える
        camManager.VCams[7].Priority -= 10;
    }

    /// <summary>
    /// 鍵を生成
    /// </summary>
    public IEnumerator DropKey()
    {
        camManager.VCams[10].Priority += 20;

        yield return new WaitForSeconds(2);

        //鍵生成
        ItemDetail key = Instantiate(keyPrefab, keyTran, false);

        //生成時にItemManagerコンポーネントを取得する
        key.SetupItemDetail(itemManager);

        //centerCameraを上下に見れるようにする
        camManager.ChangePovValue();

        //宝箱のクリックイベントを切り替える
        click.ChestDispose.Dispose();
        click.ClickTreasureChest2();
        click.ClickDoor();

        audioManager.PlaySE(8);

        yield return new WaitForSeconds(2);

        camManager.VCams[10].Priority -= 20;
    }
}
