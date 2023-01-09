using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
}
