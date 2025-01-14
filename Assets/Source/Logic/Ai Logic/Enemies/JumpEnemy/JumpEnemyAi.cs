using NTC.Global.Cache;
using UnityEngine;

public class JumpEnemyAi : MonoCache
{
    [SerializeField] private bool _detectingPlayer = true;
    [SerializeField] private float calmJumpInterval = 1f;
    [SerializeField] private float fightJumpInterval = 0.5f;

    private const float c_DistanceToCheckWalls = 150f;

    private float _timer;

    private bool _makingSecondJump;
    private bool _inFight;
    private bool _inStun;

    private JumpEnemyMover _mover;
    private PlayerLocator _playerLocator;
    private IInStun _stunController;

    private void Awake()
    {
        _mover = Get<JumpEnemyMover>();
        _playerLocator = Get<PlayerLocator>();
        _stunController = Get<IInStun>();
        _timer = calmJumpInterval;
    }

    protected override void OnEnabled()
    {
        _stunController.OnStun += HandleStun;
        _stunController.OnRecover += HandleRecover;
        _mover.OnLanding += HandleLanding;
    }

    protected override void OnDisabled()
    {
        _stunController.OnStun -= HandleStun;
        _stunController.OnRecover -= HandleRecover;
        _mover.OnLanding -= HandleLanding;
    }

    protected override void Run()
    {
        if (_inStun && !_mover.IsGrounded())
        {
            _makingSecondJump = false;
            _timer = GetCurrentJumpInterval();
        }
        _timer -= Time.deltaTime;

        if (_inStun || _timer > 0)
            return;

        var jumpDirection = _detectingPlayer && _playerLocator.IsPlayerVisible() && _makingSecondJump ? _playerLocator.GetDirectionToPlayerNorm() : GetRandomJumpDirection();
        if (_playerLocator.IsPlayerVisible())
            jumpDirection.y += 0.1f;
        //Log("Player visibility - " + _playerLocator.IsPlayerVisible());
        //Log("Making second jump - " + _makingSecondJump);
        //Log("Fighting - " + _inFight);
        Jump(jumpDirection);
    }

    private Vector3 GetRandomJumpDirection()
    {
        var direction = Vector3.zero;
        var tryCount = 5;

        for (var i = 0; i < tryCount; i++)
        {
            var x = Random.Range(-80, _mover.OnWall() ? 5 : 80);
            var y = Random.Range(-80, 80);
            var z = Random.Range(-80, 80);
            direction = Quaternion.Euler(x, y, z) * _mover.GetCurrentNormal();

            if (Physics.Raycast(transform.position, direction, c_DistanceToCheckWalls, Layers.Environment))
            {
                break;
            }

            if (i == tryCount - 1)
            {
                direction = Random.insideUnitSphere.normalized;
                direction.y = .1f;
            }
        }
        Log(direction.ToString());
        return direction;
    }

    private void Jump(Vector3 direction)
    {
        Log("Jumping in directoin - " + direction);
        _makingSecondJump = !_makingSecondJump;
        _mover.JumpToDirection(direction);
        _timer += 10;
    }

    private void HandleLanding()
    {
        _timer = GetCurrentJumpInterval();
    }

    private void HandleStun()
    {
        _inStun = true;
        _mover.StartStun();
    }

    private void HandleRecover()
    {
        _inStun = false;
        _mover.EndStun();
    }

    private float GetCurrentJumpInterval()
    {
        var multiplier = _makingSecondJump ? 0.5f : 1f;
        return _inFight ? fightJumpInterval * multiplier : calmJumpInterval * multiplier;
    }
}
