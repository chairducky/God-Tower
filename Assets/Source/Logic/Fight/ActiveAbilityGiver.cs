﻿using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class ActiveAbilityGiver : MonoCache, IGiveAbility
{
    [SerializeField] private bool canGiveAbility;
    [SerializeField] private GameObject abilityPrefab;
    [SerializeField] private StackedAbility stackedAbilityPrefab;
    [SerializeField] private AudioClip stealClip;
    [SerializeField] private AudioSource stealSoundPrefab;

    private ITrackingGiveAbilityOpportunity _tracker;
    private IHealthHandler _healthHandler;
    
    private void Awake()
    {
        if (abilityPrefab.GetComponent<IActiveAbility>() == null)
        {
            Debug.LogError("No ability in ability prefab on ability giver wtf");
        }

        _tracker = Get<ITrackingGiveAbilityOpportunity>();
        _healthHandler = Get<IHealthHandler>();
    }

    protected override void OnEnabled()
    {
        _tracker.OnCanGiveAbility += SetCanGive;
        _tracker.OnNotCanGiveAbility += SetCanNotGive;
    }

    protected override void OnDisabled()
    {
        _tracker.OnCanGiveAbility -= SetCanGive;
        _tracker.OnNotCanGiveAbility += SetCanNotGive;
    }

    public GameObject GetAbilityPrefab()
    {
        if (_healthHandler != null)
            _healthHandler.Die(true);

        if (stealSoundPrefab)
        {
            var sound = NightPool.Spawn(stealSoundPrefab, transform.position);
            sound.clip = stealClip;
            sound.Play();
        }
        
        return abilityPrefab;
    }

    public StackedAbility GetStackedAbilityPrefab()
    {
        if (_healthHandler != null)
            _healthHandler.Die(true);

        if (stealSoundPrefab)
        {
            var sound = NightPool.Spawn(stealSoundPrefab, transform.position);
            sound.clip = stealClip;
            sound.Play();
        }
        canGiveAbility = false;
        return stackedAbilityPrefab;
    }

    public bool CanGiveAbility()
    {
        return canGiveAbility;
    }

    private void SetCanGive()
    {
        canGiveAbility = true;
    }

    private void SetCanNotGive()
    {
        canGiveAbility = false;
    }
}
