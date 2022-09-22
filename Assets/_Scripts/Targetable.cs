using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public Vector3 AimPositionOffset;
    public delegate void TargetHitEvent(float damage_);

    private TargetHitEvent _hitEvents;
    private static List<Targetable> _allTargets = new List<Targetable>();
    private bool _isActive = true;

    public static List<Targetable> GetAllTargets()
    {
        return Targetable._allTargets;
    }

    public bool IsActive
    {
        get
        {
            return this._isActive;
        }
    }

    public Vector3 AimPosition
    {
        get
        {
            return this.transform.position + this.AimPositionOffset;
        }
    }

    private void Awake()
    {
        Targetable._allTargets.Add(this);
    }

    private void OnDestroy()
    {
        Targetable._allTargets.Remove(this);
    }

    public void AddHitEvent(TargetHitEvent action_)
    {
        this._hitEvents += action_;
    }

    public void SetActive(bool flag_)
    {
        this._isActive = flag_;
    }

    public void Hit(float damage_)
    {
        this._hitEvents(damage_);
    }
}
