using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// ゲームのギミックの判定をするクラス
/// Hierarchy:"GameManager"にアタッチ
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Presenter presenter;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private Generator generator;
    [SerializeField]
    private AudioManager audioManager;
    [SerializeField]
    private CameraManager camManager;

    [SerializeField]
    private MeshRenderer sliderBoardMesh;
    [SerializeField]
    private Material[] sliderBoardMaterial;

    [SerializeField]
    private bool[] lockings;
    public bool[] Locking { get => Locking; }

    [SerializeField]
    private int[] correctNumbers;
    [SerializeField]
    private int[] correctSliders;

    [SerializeField]
    private Image imgGameClear;

    void Start()
    {
        //特定のUIに演出を加えつつ各種コンポーネントを取得する
        uiManager.FlashButton(uiManager.BtnReturn, this, audioManager);
    }

    /// <summary>
    /// NumbeBoardの施錠を正解の配列と一致するかで判定
    /// </summary>
    public void JudgeNumberBoard()
    {
        if (!lockings[0]) return;

        for (int i = 0; i < correctNumbers.Length; i++)
        {
            if (uiManager.displayNumbers[i].Value != correctNumbers[i]) return;            
        }

        Debug.Log("NumberBoard 開錠");

        audioManager.PlaySE(1);

        DOVirtual.DelayedCall( 3, () =>
        {
            //クエリちゃん達を生成する
            StartCoroutine(generator.GenerateQueriChans( this, uiManager, audioManager));
        });

        //SliderBoardのマテリアルを変更して次のギミックをわかりやすくする
        sliderBoardMesh.material = sliderBoardMaterial[0];

        lockings[0] = false;
    }

    /// <summary>
    /// (正解番号変更後)NumbeBoardの施錠を正解の配列と一致するかで判定
    /// </summary>
    /// <returns></returns>
    public void JudgeNumberBoard_2()
    {
        if (lockings[1]) return;
        else if(!lockings[2]) return;

        Debug.Log("JudgeNumberBoard_2　準備");

        for (int i = 0; i < correctNumbers.Length; i++)
        {
            if (uiManager.displayNumbers[i].Value != correctNumbers[i]) return;
        }

        Debug.Log("NumberBoard_2 開錠");

        audioManager.PlaySE(1);

        DOVirtual.DelayedCall(3, () =>
        {
            StartCoroutine(generator.DropQueriChanItem());
        });

        lockings[2] = false;
    }

    /// <summary>
    /// SliderBoardの施錠を正解の配列と一致するかで判定
    /// </summary>
    public void JudgeSliderBoard()
    {
        if (lockings[0]) return;
        if (!lockings[1]) return;

        Debug.Log("SliderBoard 施錠中");

        for (int i = 0; i < correctSliders.Length; i++)
        {
            if (uiManager.Sliders[i].value != correctSliders[i]) return;
        }

        Debug.Log("SliderBoard 開錠");

        audioManager.PlaySE(1);

        ChangeCorrectNumber();

        DOVirtual.DelayedCall( 3,() =>
        {
            //家(ミニチュア)を生成する
            StartCoroutine(generator.GenerateHouse());
        });

        //SliderBoardのマテリアルを変更して次のギミックをわかりやすくする
        sliderBoardMesh.material = sliderBoardMaterial[1];

        lockings[1] = false;
    }

    /// <summary>
    /// (正解番号変更後)SliderBoardの施錠を正解の配列と一致するかで判定
    /// </summary>
    public void JudgeSliderBoard_2()
    {
        if (lockings[2]) return;
        if (!lockings[3]) return;


        Debug.Log("SliderBoard_2 施錠中");

        for (int i = 0; i < correctSliders.Length; i++)
        {
            if (uiManager.Sliders[i].value != correctSliders[i]) return;
        }

        Debug.Log("SliderBoard_2 開錠");

        audioManager.PlaySE(1);

        DOVirtual.DelayedCall(3, () =>
        {
            //鍵が床"LookUp"の場所にドロップ
            StartCoroutine(generator.DropKey());
        });

        //SliderBoardのマテリアルを変更して次のギミックの場所をわかりやすくする
        sliderBoardMesh.material = sliderBoardMaterial[2];

        //テロップ切り替え
        presenter.DoorDisposable.Dispose();
        presenter.ClickDoor();
       
        lockings[3] = false;
    }

    /// <summary>
    /// NumberBoardの正解の番号を組み替える
    /// </summary>
    private void ChangeCorrectNumber()
    {
        correctNumbers[0] += -1;
        correctNumbers[1] += 1;
        correctNumbers[2] += 0;
        correctNumbers[3] += 1;
    }

    /// <summary>
    /// SliderBoardの正解の番号を変更する
    /// </summary>
    public void ChangeCorrectSlider()
    {
        correctSliders[2] = 0;
    }

    public IEnumerator GameClear()
    {
        imgGameClear.DOFade(0.7f, 5);
        imgGameClear.transform.DOLocalMoveY(imgGameClear.transform.position.y + 0.5f, 5);

        //薬莢音
        audioManager.PlaySE(19);

         yield return new WaitForSeconds(3);
        //歩く音
        audioManager.PlaySE(20);

        　yield return new WaitForSeconds(2);
        //ドア開閉１
        audioManager.PlaySE(21);

        yield return new WaitForSeconds(0.5f);
        //ドア開閉２
        audioManager.PlaySE(22);

        //歩く音徐々に遠くなる
        audioManager.SSEs[20].DOFade(0, 10);
    }
}
