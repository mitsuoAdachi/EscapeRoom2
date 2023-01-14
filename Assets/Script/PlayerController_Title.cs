using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public IEnumerator Shooting()
    {
        //発砲エフェクト
        gunParticle.Play();

        gunSE.Play();

        yield return new WaitForSeconds(0.1f);

        //弾丸生成
        GameObject bullet = Instantiate(bulletPrefab, bulletTran, false);

        //Transform型をVector3型にキャストする
        Vector3 target = targetTran.transform.position;

        //生成された弾丸が目標に向かってまっすぐ飛ぶ
        bullet.transform.DOMove(target, 4);

        yield return new WaitForSeconds(1f);

        titleScene.FadeInScreen(fadeInTexture);

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("MainScene");
    }
}
