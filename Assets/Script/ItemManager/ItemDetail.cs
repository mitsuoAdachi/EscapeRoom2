using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public enum ItemName
{
    鍵,
    クエリちゃんコントローラー,
    骸骨バスター,
}

public enum ItemType
{
    sprite,
    obj,
}

/// <summary>
/// 取得できるアイテムの情報を管理するクラス
/// Prefab:各種アイテムにアタッチ
/// </summary>
public class ItemDetail : MonoBehaviour
{
    [SerializeField]
    private int itemNo;
    public int ItemNo { get => itemNo; }

    [SerializeField]
    private ItemName itemName;
    public ItemName ItemName { get => itemName; }

    [SerializeField]
    private ItemType itemType;
    public ItemType ItemType { get => itemType; }

    [SerializeField]
    private Sprite itemSprite;
    public Sprite ItemSprite { get => itemSprite; }


    [SerializeField]
    private ObservableEventTrigger obj;

    private ItemManager itemManager;


    private void Start()
    {
        ReflectGetItem();
    }

    /// <summary>
    /// 所得可能アイテムをクリックで取得する
    /// </summary>
    private void ReflectGetItem()
    {
        obj.OnPointerDownAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(async _ => await itemManager.GetItem(obj))
            .AddTo(this);
    }

    /// <summary>
    /// ItemManagerコンポーネントを取得する
    /// </summary>
    /// <param name="itemManager"></param>
    public void SetupItemDetail(ItemManager itemManager)
    {
        this.itemManager = itemManager;

        Debug.Log(itemManager + "取得");
    }
}