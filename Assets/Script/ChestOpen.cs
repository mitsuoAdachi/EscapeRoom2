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
    private ObservableEventTrigger[] drawers;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private BoxCollider stopCollider;


    private void Start()
    {
        ReflectOpenChest();
    }

    /// <summary>
    /// 引き出しをクリック時にMovableDrawerを呼び出す
    /// </summary>
    private void ReflectOpenChest()
    {
        for (int i = 0; i < drawers.Length; i++)
        {
            int index = i;

            drawers[index].OnPointerDownAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => MovableDrawer(index))
                .AddTo(this);
        }
    }

    /// <summary>
    /// 引き出しの開閉
    /// </summary>
    private void MovableDrawer(int index)
    {
        if (!isOpen[index])
        {
            drawers[index].transform.DOLocalMoveX(-1.5f + index * 0.4f, 1);

            audioManager.PlaySE(6);

            isOpen[index] = true;
        }
        else
        {
            drawers[index].transform.DOLocalMoveX(0, 1);

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
