using Cysharp.Threading.Tasks;
using NTC.Global.Pool;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class HomingStackedAbility : StackedAbility
{
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private VisualEffect vfx;

    private static Hashtable _targetsHashes = new();
    private Collider[] _targets = new Collider[10];

    public override void PerformOnImpact(Vector3 position)
    {
        var stackedCount = transform.parent.GetComponentsInChildren<HomingStackedAbility>().Length;

        Array.Clear(_targets, 0, _targets.Length);

        Physics.OverlapSphereNonAlloc(position, 30 + stackedCount * 10, _targets, targetLayers);

        var targetPos = position + Vector3.up * 100;

        for (var i = 0; i < _targets.Length; i++)
        {
            if (!_targets[i])
                break;

            var hash = _targets[i].GetHashCode();

            if (_targetsHashes.ContainsKey(hash))
                continue;

            var health = _targets[i].GetComponentInChildren<ITakeHit>();
//            if (health == null || health.InStun())
  //              continue;

            health.TakeHit(20, transform.position, "Homing laser");

            targetPos = _targets[i].transform.position;

            _targetsHashes.Add(hash, _targets[i]);
            RemoveHashFromTable(hash);

            break;
        }

        var effect = NightPool.Spawn(vfx, position);
        effect.SetVector3("Pos1", position);
        effect.SetVector3("Pos2", (position + targetPos) * 0.5f * UnityEngine.Random.Range(0.8f, 1.2f));
        effect.SetVector3("Pos3", (position + targetPos) * 0.5f * UnityEngine.Random.Range(0.7f, 1.3f));
        effect.SetVector3("Pos4", targetPos);

        TimeController.Instance.AddTimeStopDuration(0.01f);
        base.PerformOnImpact(position);

        _performed = true;
    }

    public static async UniTask RemoveHashFromTable(int hash)
    {
        await UniTask.Delay(1000);

        _targetsHashes.Remove(hash);
    }

    public override AbilityTypes GetStackedAbilityType()
    {
        return AbilityTypes.Homing;
    }
}