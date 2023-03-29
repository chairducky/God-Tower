﻿using System;
using UnityEngine;

public interface IActiveAbility
{
    public event Action OnPerform;
    public event Action OnStartHolding;
    public event Action OnDumpLoaded;
    public event Action OnEmpty;
    
    public void ChargeAbility();
    public void PerformAbility();
    public void DumpAbility();
    public void PerformWithDelay(float delay);
    public void SetInput(bool value);
    public void SetRotationTarget(Transform rotationTarget);
    public void SetShootPoint(Transform shootPoint);
    public float GetRemainingLifetime();
    public float GetMaxLifetime();
    public Vector3 GetPerformDirection();
    public Vector3 GetStartPoint();
}