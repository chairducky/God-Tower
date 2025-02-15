﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using NTC.Global.Cache;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class RoomDoor : MonoCache
{
    [SerializeField] private RoomParent connectedRoom;
    [SerializeField] private bool trackEnemies = true;
    [SerializeField] private EnemyGroup[] groupsToTrack;
    [SerializeField] private GameObject[] objectsToEnableOnOpen;

    [SerializeField] private float height = 250;
    [SerializeField] private float openSpeed = 300f;
    [SerializeField] private float closeSpeed = 300f;

    private int _enemiesCount;
    
    private bool _closed = true;

    private float _speed = 1f;
    
    private Vector3 desiredPosition;

    private AudioSource _audioSource;

    public UnityEvent OnOpen = new();

    public static event Action OnRoomCompleted;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        desiredPosition = transform.position;
    }

    protected override void OnEnabled()
    {
		/*
        if (!trackEnemies)
            return;

        if (connectedRoom)
        {
            groupsToTrack = connectedRoom.GetComponentsInChildren<EnemyGroup>();
        }

        _enemiesCount = 0;

        for (var i = 0; i < groupsToTrack.Length; i++)
        {
            var enemiesInGroup = groupsToTrack[i].GetComponentsInChildren<BaseAiController>();
            for (var j = 0; j < enemiesInGroup.Length; j++)
            {
                if (enemiesInGroup[j].TryGetComponent<IHealthHandler>(out var health))
                {
                    _enemiesCount++;
                    health.OnDied += HandleEnemyDied;
                }
            }
        }
		*/
    }

    protected override void Run()
    {
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, _speed * Time.deltaTime);
    }

    public void Open()
    {
        OnOpen.Invoke();
        OnRoomCompleted?.Invoke();
        desiredPosition += Vector3.up * height;
        _speed = closeSpeed;
        _closed = false;

        for (var i = 0; i < objectsToEnableOnOpen.Length; i++)
        {
            if (objectsToEnableOnOpen[i])
                objectsToEnableOnOpen[i].SetActive(true);
        }

        if (connectedRoom)
        {
            var timeTotem = connectedRoom.GetComponentInChildren<TimeTotem>();

            if (timeTotem)
            {
                timeTotem.StopCanceling();
            }
        }

        _audioSource.Play();
    }

    public void Close()
    {
        if (_closed)
            return;
        _speed = closeSpeed;
        desiredPosition -= Vector3.up * height;
        _closed = true;
    }

    private void HandleEnemyDied()
    {
        _enemiesCount--;
        if (_enemiesCount <= 0)
            Open();
    }
}
