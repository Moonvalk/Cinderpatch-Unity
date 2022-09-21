using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPerspective : MonoBehaviour
{
    public CameraTargetPoint Target;
    public BoxCollider Collider;
    public bool Enabled = true;

    private void Start()
    {
        if (!this.Enabled)
        {
            this.Disable();
        }
    }

    public void Enable()
    {
        this.Enabled = true;
        this.Collider.enabled = true;
    }

    public void Disable()
    {
        this.Enabled = false;
        this.Collider.enabled = false;
    }
}
