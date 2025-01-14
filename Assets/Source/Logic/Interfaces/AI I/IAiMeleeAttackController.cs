﻿using UnityEngine;

public interface IAiMeleeAttackController
{
    public void TryAttack();
    public void AllowAttack();
    public void DisallowAttack();
    public bool NeedToAttack();
}
