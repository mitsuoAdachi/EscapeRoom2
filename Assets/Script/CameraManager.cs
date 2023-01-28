using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

/// <summary>
/// ゲーム内のカメラを管理する
/// Hierarchy:CameraManagerにアタッチ
/// </summary>
public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera[] vCams;
    public CinemachineVirtualCamera[] VCams { get => vCams; }

    [SerializeField]
    private CinemachineVirtualCamera centerCam;

    [SerializeField]
    private UIManager uiManager;

    /// <summary>
    /// 中央にあるCenterCameraを上下も見えるようにする
    /// </summary>
    public void ChangePovValue()
    {
        //CenterCameraのPOV要素を取得する
        var pov = centerCam.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();

        //POVのValueRangeを調整する
        pov.m_VerticalAxis.m_MinValue = -70;
        pov.m_VerticalAxis.m_MaxValue =  70;
    }

    /// <summary>
    /// VirtualCameraのPriorityを変えてカメラを切り替える
    /// </summary>
    /// <param name="index"></param>
    public void ChangeCamera(int index)
    {
        //CenterCameraに戻るためのボタンを押せるようにする
        uiManager.BtnReturn.interactable = true;

        //CenterCameraに戻るためのボタンの点滅をリスタートする
        uiManager.Tween.Restart();

        //指定のVirtualCameraのPriorityを加算して優先順位を変える
        VCams[index].Priority += 10;
    }

    /// <summary>
    /// CenterCameraに戻る処理
    /// </summary>
    public void ReturnCamera()
    {
        uiManager.BtnReturn.interactable = false;

        uiManager.Tween.Pause();

        for (int i = 0; i < VCams.Length; i++)
        {
            VCams[i].Priority = 1;
        }
    }

}
