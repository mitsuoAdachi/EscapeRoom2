using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PresenterクラスよりUI周りの情報を取得しcanvasへ表示する
/// Hierarchy:"UIManager"にアタッチ
/// </summary>
public class View : MonoBehaviour
{
    [SerializeField]
    public Text[] txtViewNumbers;

    [SerializeField]
    private Image imgCenter;
    [SerializeField]
    private Image[] itemFolders;

    [SerializeField]
    private Text txtQueriSwitch;

    [SerializeField]
    private Text hintTelop;


    /// <summary>
    /// プレゼンターからの情報をテキストに反映
    /// </summary>
    /// <param name="index"></param>
    /// <param name="number"></param>
    public void ViewNumber(int index,int number)
    {
        txtViewNumbers[index].text = number.ToString();
    }

    /// <summary>
    /// 取得したアイテムのイメージを表示する
    /// </summary>
    /// <param name="itemSprite"></param>
    public void ViewDisplayItemImage(Sprite itemSprite)
    {
        imgCenter.sprite = itemSprite;

        //アイテムフォルダ欄の空いているimageに表示する
        for(int i = 0; i < itemFolders.Length; i++)
        {
            if (itemFolders[i].sprite == null)
            {
                itemFolders[i].sprite = itemSprite;
                return;
            }
        }
    }

    /// <summary>
    /// クエリコントローラーのON/OFFを表示する
    /// </summary>
    /// <param name="word"></param>
    public void ViewQueriSwitchText(string word)
    {
        txtQueriSwitch.text = word;
    }

    /// <summary>
    /// ヒントを表示する
    /// </summary>
    /// <param name="txtHint"></param>
    public void ViewHintTelop(string txtHint)
    {
        hintTelop.text = txtHint;
    }
}
