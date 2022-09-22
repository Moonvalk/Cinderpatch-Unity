using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Utilities;
using Moonvalk.Animation;

public enum PumpkinState
{
    Sapling,
    Grown,
}

public class Pumpkin : MonoBehaviour
{
    public PumpkinType Type;
    protected FarmTile _tile;
    protected PumpkinState _currentState = PumpkinState.Grown;
    protected Spring _growSpring;
    protected float _pumpkinScale = 1f;

    private void Start()
    {
        Targetable targetable = this.GetComponent<Targetable>();
        targetable.AddHitEvent(this.pumpkinHit);

        this._pumpkinScale = 0.5f;
        this._growSpring = new Spring(() => ref this._pumpkinScale);
        this._growSpring.Tension(50f).Dampening(4f);
        this._growSpring.To(1f);
    }

    private void FixedUpdate()
    {
        this.updateScale();
    }

    public void SetTile(FarmTile tile_)
    {
        this._tile = tile_;
    }

    public void SetState(PumpkinState state_)
    {
        if (this._currentState == state_)
        {
            return;
        }
        this._currentState = state_;
        this.updateTextureForState();
        this.updateTargetForState();

        if (this._currentState == PumpkinState.Sapling)
        {
            MicroTimer growTimer = new MicroTimer(() => {
                this.SetState(PumpkinState.Grown);
                this._pumpkinScale = 0.5f;
                this._growSpring.To(1f);
                this.updateScale();
            });
            growTimer.Start(10f);
        }
    }

    protected void updateScale()
    {
        transform.localScale = new Vector3(this._pumpkinScale, this._pumpkinScale, this._pumpkinScale);
    }

    protected void pumpkinHit(float damage_)
    {
        this._tile.SetState(FarmTileState.Untilled);
        this._tile.SetOccupied(false);
        Destroy(this.gameObject);
    }

    protected void updateTextureForState()
    {
        for (int index = 0; index < this.Type.TextureStates.Length; index++)
        {
            if (this.Type.TextureStates[index].State == this._currentState)
            {
                MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
                meshRenderer.material.mainTexture = this.Type.TextureStates[index].Image;
                break;
            }
        }
    }

    protected void updateTargetForState()
    {
        Targetable targetable = this.GetComponent<Targetable>();
        targetable.SetActive(this._currentState == PumpkinState.Grown);
    }
}
