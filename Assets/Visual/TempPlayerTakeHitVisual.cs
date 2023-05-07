﻿using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TempPlayerTakeHitVisual : MonoCache
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip takeDamageClip;
    [SerializeField] private Volume volume;
    [SerializeField] private ShakePreset camShake;

    protected override void OnEnabled()
    {
        Get<IHealthHandler>().OnHit += HandleTakeHit;
    }

    private void HandleTakeHit()
    {
        CameraService.Instance.ShakeCamera(camShake);
        source.PlayOneShot(takeDamageClip);
        ChangePostProcess();

    }

    private async UniTask ChangePostProcess()
    {
        Vignette vignette;
        
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.color.value = Color.red;
        }

        await UniTask.Delay(300);
        
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.color.value = Color.black;
        }
    }
}