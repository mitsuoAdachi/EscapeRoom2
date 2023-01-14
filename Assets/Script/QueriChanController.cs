using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
///　ユニティちゃんに関する行動、影響に関するクラス
/// Hierarchy:QueriChans -> Query-Chan-Movable、Prefab:各種ユニティちゃんオブジェクトにアタッチ
/// </summary>
public class QueriChanController : MonoBehaviour
{
    [SerializeField]
    private ObservableEventTrigger jumpTrigger;

    private SingleAssignmentDisposable queriPinkDispose = new();
    public SingleAssignmentDisposable QueriPinkDispose { get => queriPinkDispose; }

    [SerializeField]
    private ObservableEventTrigger telopTrigger;

    [SerializeField]
    private Animator anime;
    public Animator Anime { get => anime; }

    [SerializeField]
    private AudioManager audioManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private CameraManager camManager;
    [SerializeField]
    private ItemManager itemManager;
    [SerializeField]
    private Presenter presenter;

    [SerializeField]
    private GameObject queriChanHead;

    [SerializeField]
    private Rigidbody rigidQueri;
    public Rigidbody RigidQueri { get => rigidQueri; }

    [SerializeField]
    private bool Ismovable;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float limitSpeed;
    [SerializeField]
    private float rotSpeed;

    private Vector3 latestPos;


    private void Start()
    {
        JumpClickTrigger();
    }

    /// <summary>
    /// クエリちゃん生成時にUIManagerコンポーネントを取得する
    /// </summary>
    /// <param name="uiManager"></param>
    public void SetupQueriChanController(UIManager uiManager,AudioManager audioManager)
    {
        this.uiManager = uiManager;
        this.audioManager = audioManager;
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    public void Jump()
    {
            anime.SetTrigger("jump");

            audioManager.PlaySE(2);
    }

    /// <summary>
    /// クリックした時にJump()メソッドを実行する。queriChanHeadが破棄されるまで。
    /// </summary>
    private void JumpClickTrigger()
    {
        if (jumpTrigger == null) return;

        //RunTime中にクリックイベントを切り替えるためdelegateしておく
        queriPinkDispose.Disposable = jumpTrigger.OnPointerDownAsObservable()
                .Subscribe(_ => Jump())
                .AddTo(this);
    }

    /// <summary>
    /// クリック時にテロップが出るようにする
    /// </summary>
    public void ClickQueriPinkTelop()
    {
        telopTrigger.OnPointerDownAsObservable()
            .Subscribe(_ => uiManager.DisplayTelopModel(7, 3))
            .AddTo(this);  
    }

    /// <summary>
    /// クエリちゃんの頭の装飾を消す
    /// </summary>
    public void DestroyQueriHeadgear()
    {
        Destroy(queriChanHead);
    }

    /// <summary>
    /// クエリちゃんの移動操作
    /// </summary>
    public void OnMovable()
    {
        Ismovable = true;
    }
    private void Update()
    {
        if (!Ismovable) return;

        //移動
        var _horizontal = Input.GetAxis("Horizontal2");
        var _vertical = Input.GetAxis("Vertical2");

        //移動スピード制限
        float MoveX = Mathf.Clamp(_horizontal, -limitSpeed, limitSpeed);
        float MoveZ = Mathf.Clamp(_vertical, -limitSpeed, limitSpeed);

        //Y軸に対する入力をカメラ奥へのベクトルに変換
        var changeVector = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

        //力を加える
        //rigidQueri.AddForce(changeVector * new Vector3(MoveX, 0, MoveZ) * speed, ForceMode.VelocityChange);
        transform.position += changeVector * new Vector3(MoveX, 0, MoveZ) * speed;

        //移動方向を向く
        Vector3 diff = transform.position - latestPos;
        latestPos = transform.position;

        diff.y = 0; 　//縦回転しないようにする

        if (diff.sqrMagnitude > 0.003f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diff), Time.deltaTime * rotSpeed);

            //走るアニメーション
            anime.SetFloat("running", 0.5f);
        }
        else  anime.SetFloat("running", 0);
    }

    /// <summary>
    /// 操作をクエリちゃんに切り替えとその時の演出
    /// </summary>
    /// <returns></returns>
    public IEnumerator SteeringQueriChan()
    {
        //特定のクリックイベントが発生しないようにする
        for(int i = 0; i < presenter.telopTriggerList.Count; i++)
        {
            presenter.telopTriggerList[i].enabled = false;
        }

        itemManager.QueriMovableTrigger.enabled = false;

        //カメラを中央に戻すボタンを使用不可に
        uiManager.BtnReturn.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        //カメラ切り替え
        camManager.VCams[8].Priority += 10;

        //テロップ表示
        uiManager.DisplayTelopModel(3,4);

        //決めポーズ
        anime.SetTrigger("getItem");

        //効果音
        audioManager.PlaySE(11);

        //BGM切り替え
        audioManager.ChangeBGM(1);

        //プレイヤーが見える状態にする
        uiManager.Player.SetActive(true);

        yield return new WaitForSeconds(4);

        //クエリちゃん操作用カメラ切り替え
        camManager.VCams[8].Priority -= 10;
        camManager.VCams[9].Priority += 10;

        for (int i = 0; i < 7; i++)
        {
            camManager.VCams[i].Priority = 1;
        }

        //クエリちゃんの操作に切り替える
        OnMovable();

        //操作説明テロップ
        uiManager.DisplayTelopModel(4,5);
    }
}
