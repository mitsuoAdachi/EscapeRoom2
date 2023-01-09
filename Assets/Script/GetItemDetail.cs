using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 取得したアイテムを表示する時にゆっくり回転させる
/// 取得アイテムにアタッチ
/// </summary>
public class GetItemDetail : MonoBehaviour
{
    private void Start()
    {
        this.transform.DORotate(new Vector3(0, -360, 0), 8f);
    }
}
