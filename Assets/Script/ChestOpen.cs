using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

/// <summary>
/// タンスの引き出しの開閉にまつわる機能
/// Hierarchy:”Room" -> "Chest"にアタッチ
/// </summary>
public class ChestOpen : MonoBehaviour
{
    [SerializeField]
    private bool[] isOpen = new bool[4];

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private BoxCollider stopCollider;

    [SerializeField]
    private ClickManager click;


    /// <summary>
    /// 引き出しの開閉
    /// </summary>
    public void MovableDrawer(int index)
    {
        if (!isOpen[index])
        {
            click.Drawers[index].transform.DOLocalMoveX(-1.5f + index * 0.4f, 1);

            audioManager.PlaySE(6);

            isOpen[index] = true;
        }
        else
        {
            click.Drawers[index].transform.DOLocalMoveX(0, 1);

            audioManager.PlaySE(7);

            isOpen[index] = false;
        }

        //全てのisOpenがtrue(全ての引き出しが開けてある)であれば条件を満たす
        if (!isOpen[0]) return;
        if (!isOpen[1]) return;
        if (!isOpen[2]) return;
        if (!isOpen[3]) return;

        audioManager.PlaySE(8);

        stopCollider.enabled = false;      
    }
}
