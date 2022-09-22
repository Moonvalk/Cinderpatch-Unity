using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FarmTileState
{
    Untilled,
    Tilled,
}

[System.Serializable]
public struct FarmTileTexture
{
    public Texture Image;
    public FarmTileState State;
}

public class FarmTile : MonoBehaviour
{
    public bool Occupied = false;

    protected FarmPatch _patch;
    protected FarmTileState _currentState = FarmTileState.Untilled;
    protected MeshRenderer _meshRenderer;

    public FarmTileState State
    {
        get
        {
            return this._currentState;
        }
    }

    private void Awake()
    {
        this._meshRenderer = GetComponent<MeshRenderer>();
        this._meshRenderer.material = new Material(this._meshRenderer.material);
    }

    public void SetManager(FarmPatch patch_)
    {
        this._patch = patch_;
        this.updateTexture();
    }

    public void SetState(FarmTileState state_)
    {
        if (this._currentState == state_)
        {
            return;
        }
        this._currentState = state_;
        this.updateTexture();
    }

    protected void updateTexture()
    {
        this._meshRenderer.material.mainTexture = this._patch.GetTexture(this._currentState);
    }

    public void SpawnPumpkin(PumpkinType type_, PumpkinState state_)
    {
        this.SetState(FarmTileState.Tilled);
        this.Occupied = true;
        GameObject newPumpkin = Instantiate(type_.PumpkinObject, transform.position + new Vector3(0f, 0.05f, -0.25f), Quaternion.Euler(0f, 0f, 0f));
        Pumpkin controller = newPumpkin.GetComponent<Pumpkin>();
        controller.SetTile(this);
        controller.SetState(state_);
    }

    public void SetOccupied(bool flag_)
    {
        this.Occupied = flag_;
    }
}
