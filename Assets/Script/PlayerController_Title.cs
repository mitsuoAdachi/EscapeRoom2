using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;


/// <summary>
/// タイトルシーン用のプレイヤーの挙動に関するクラス
/// Hierarchy -> Playerにアタッチ
/// </summary>
public class PlayerController_Title : MonoBehaviour
{
    [SerializeField]
    private TitleSceneManager titleScene;

    [SerializeField]
    private ParticleSystem gunParticle;

    [SerializeField]
    private AudioSource gunSE;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform bulletTran;

    [SerializeField]
    private Transform targetTran;

    [SerializeField]
    private Texture fadeInTexture;

    /// <summary>
    /// AnimationEventで実行。弾丸がカメラに向かってくる演出でMainSceneに遷移する
    /// </summary>
    /// <returns></returns>
    public async UniTask Shooting()
    {
        //発砲エフェクト
        gunParticle.Play();

        gunSE.Play();

        await UniTask.Delay(100);

        //弾丸生成
        GameObject bullet = Instantiate(bulletPrefab, bulletTran, false);

        //Transform型をVector3型にキャストする
        Vector3 target = targetTran.transform.position;

        //生成された弾丸が目標に向かってまっすぐ飛ぶ
        bullet.transform.DOMove(target, 4);

        await UniTask.Delay(1000);

        titleScene.FadeInScreen(fadeInTexture);

        await UniTask.Delay(5000);

        SceneManager.LoadScene("MainScene");
    }
}
