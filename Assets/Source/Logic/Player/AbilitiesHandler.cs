﻿using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;


public class AbilitiesHandler : MonoCache
{
    [SerializeField] private LayerMask layersToSteal;
    [SerializeField] private Transform camRotationTarget;
    [SerializeField] private Transform rightHandShootPoint, leftHandShootPoint;
    [SerializeField] private float stealDistance = 10;
    [SerializeField] private float stealRadius = 0.5f;
    
    private IActiveAbility _leftAbility, _rightAbility;

    public Action OnNewAbility;
    public Action OnNewRightAbility;
    public Action OnNewLeftAbility;
    
    private void Start()
    {
        CheckForAbilitiesOnStart();
    }

    protected override void OnEnabled()
    {
        OnNewRightAbility += HandleNewRightAbility;
        OnNewLeftAbility += HandleNewLeftAbility;
    }
    
    public void SetNewRightAbility(GameObject newAbilityPrefab)
    {
        SetNewAbility(out _rightAbility, newAbilityPrefab, rightHandShootPoint);
        OnNewRightAbility?.Invoke();
    }

    public void SetNewLeftAbility(GameObject newAbilityPrefab)
    {
        SetNewAbility(out _leftAbility, newAbilityPrefab, leftHandShootPoint);
        OnNewLeftAbility?.Invoke();
    }

    public void SetLeftAbilityInput(bool input)
    {
        _leftAbility?.SetInput(input);
    }

    public void SetRightAbilityInput(bool input)
    {
        _rightAbility?.SetInput(input);
    }

    public void SetRightStealInput(bool input)
    {
        if (!input || _rightAbility != null)
            return;

        var robbedAbility = CheckForStealAbility();
        if (robbedAbility != null)
        {
            SetNewRightAbility(robbedAbility);
        }
    }

    public void SetLeftStealInput(bool input)
    {
        if (!input || _leftAbility != null)
            return;

        var robbedAbility = CheckForStealAbility();
        if (robbedAbility != null)
        {
            SetNewLeftAbility(robbedAbility);
        }
    }

    private GameObject CheckForStealAbility()
    {
        if (Physics.SphereCast(camRotationTarget.position, stealRadius, camRotationTarget.forward, out var hit, stealDistance,
                layersToSteal))
        {
            if (hit.transform.TryGetComponent<IGiveAbility>(out var giver) && giver.CanGiveAbility())
            {
                return giver.GetAbilityPrefab();
            }
        }

        return null;
    }

    private void SetNewAbility(out IActiveAbility abilitySide, GameObject newAbilityPrefab, Transform shootPoint)
    {
        if (newAbilityPrefab.GetComponent<IActiveAbility>() == null)
            Debug.LogError("No ability on \"ability\" prefab");
        
        var ability = Instantiate(newAbilityPrefab, transform);
        abilitySide = ability.GetComponent<IActiveAbility>();
        abilitySide.SetRotationTarget(camRotationTarget);
        abilitySide.SetShootPoint(shootPoint);
    }

    private void HandleNewRightAbility()
    {
        OnNewAbility?.Invoke();
        _rightAbility.OnEmpty += () => HandleRightAbilityEmpty();
    }

    private void HandleRightAbilityEmpty()
    {
        _rightAbility = null;
    }

    private void HandleNewLeftAbility()
    {
        OnNewAbility?.Invoke();
        _leftAbility.OnEmpty += HandleLeftAbilityEmpty;
    }

    private void HandleLeftAbilityEmpty()
    {
        _leftAbility = null;
    }

    private void CheckForAbilitiesOnStart()
    {
        var ability = GetComponentInChildren<IActiveAbility>();

        if (ability != null)
        {
            _rightAbility = ability;
            OnNewRightAbility?.Invoke();
        }
    }

    public IActiveAbility GetRightAbility() => _rightAbility;
    public IActiveAbility GetLeftAbility() => _leftAbility;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(camRotationTarget.position, stealRadius);
        Gizmos.DrawLine(camRotationTarget.position, camRotationTarget.forward * stealDistance);
    }
}