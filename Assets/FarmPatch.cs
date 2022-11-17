using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Moonvalk.Animation;
using Moonvalk.Utilities;
using Moonvalk;

public class FarmPatch : MonoBehaviour
{
    public List<FarmTile> Tiles;
    public MeshRenderer SelectedTileDisplay;
    public FarmTileTexture[] Textures;

    protected FarmPatchManager _manager;
    protected Dictionary<FarmTileState, Texture> _textureMap;
    protected bool _isPlayerInPatch = false;
    protected bool _isTileAvailable = false;
    protected Tween _selectedTileTween;
    protected Spring _selectedTileSpring;
    protected float _selectedTileOpacity = 0f;
    protected float _selectedTileScale = 0f;
    protected TweenVec2 _selectedTileMoveTween;
    protected Vector2 _selectedTilePosition;
    protected int _nearestTile;
    protected MicroTimer _tillingTimer;

    private void Awake()
    {
        this._textureMap = new Dictionary<FarmTileState, Texture>();
        foreach (FarmTileTexture texture in this.Textures)
        {
            this._textureMap.Add(texture.State, texture.Image);
        }
    }

    private void Start()
    {
        this.SelectedTileDisplay.material = new Material(this.SelectedTileDisplay.material);

        this._selectedTileTween = new Tween(() => ref this._selectedTileOpacity);
        this._selectedTileTween.Duration(0.3f).Ease(Easing.Cubic.InOut);
        this._selectedTileSpring = new Spring(() => ref this._selectedTileScale);
        this._selectedTileSpring.Tension(50f).Dampening(10f);

        this._selectedTileMoveTween = new TweenVec2(() => ref this._selectedTilePosition);
        this._selectedTileMoveTween.Duration(0.3f).Ease(Easing.Cubic.Out);

        this._tillingTimer = new MicroTimer(() => {
            this.tillCurrentTile();
            PlayerController.Player1.SetTilling(false);
        });
    }

    private void FixedUpdate()
    {
        this.updateSelectedTileDisplay();
        this.updateSelectedTilePosition();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (this._isTileAvailable)
        {
            if (this.Tiles[this._nearestTile].State == FarmTileState.Tilled)
            {
                if (Input.GetMouseButtonDown(0) && PlayerController.SeedCount > 0)
                {
                    PlayerController.Player1.RemoveSeed();
                    this._manager.RequestNewSapling(this.Tiles[this._nearestTile]);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (!_tillingTimer.IsRunning)
                {
                    this._tillingTimer.Start(PlayerController.Player1.TillDuration);
                    PlayerController.Player1.SetTilling(true, this.Tiles[this._nearestTile].transform.position);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (this._tillingTimer.IsRunning)
            {
                this._tillingTimer.Stop();
                PlayerController.Player1.SetTilling(false);
            }
        }
    }

    private void tillCurrentTile()
    {
        this.Tiles[this._nearestTile].SetState(FarmTileState.Tilled);
    }

    private void OnTriggerEnter(Collider other_)
    {
        this.setPlayerInPatch(true);
    }

    private void OnTriggerExit(Collider other_)
    {
        this.setPlayerInPatch(false);
    }

    protected void setPlayerInPatch(bool flag_)
    {
        if (this._isPlayerInPatch == flag_)
        {
            return;
        }
        this._isPlayerInPatch = flag_;
    }

    protected void updateSelectedTileDisplay()
    {
        this.SelectedTileDisplay.material.color = new Color(1f, 1f, 1f, this._selectedTileOpacity);
        this.SelectedTileDisplay.transform.localScale = new Vector3(this._selectedTileScale, this._selectedTileScale, this._selectedTileScale);
    }

    protected void updateSelectedTilePosition()
    {
        PlayerController player = PlayerController.Player1;
        if (this._isPlayerInPatch && !player.IsAiming)
        {
            int nearestIndex = 0;
            float nearestDistance = float.MaxValue;
            for (int index = 0; index < this.Tiles.Count; index++)
            {
                float distance = Vector3.Distance(this.Tiles[index].transform.position, player.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = index;
                }
            }

            bool isTileOccupied = this.Tiles[nearestIndex].Occupied;
            if (isTileOccupied == this._isTileAvailable)
            {
                this._isTileAvailable = !isTileOccupied;
                this.displaySelectedTile(!isTileOccupied);
                if (this._isTileAvailable)
                {
                    Transform tileTransform = this.Tiles[nearestIndex].transform;
                    this._selectedTilePosition = new Vector2(tileTransform.position.x, tileTransform.position.z);
                }
            }
            if (isTileOccupied)
            {
                if (this._tillingTimer.IsRunning)
                {
                    this._tillingTimer.Stop();
                    PlayerController.Player1.SetTilling(false);
                }
                return;
            }

            if (this._nearestTile != nearestIndex)
            {
                this._nearestTile = nearestIndex;
                Transform tileTransform = this.Tiles[this._nearestTile].transform;
                this._selectedTileMoveTween.To(new Vector2(tileTransform.position.x, tileTransform.position.z)).Start();
            }
        }
        else if (this._isTileAvailable)
        {
            this._isTileAvailable = false;
            this.displaySelectedTile(false);
        }

        this.SelectedTileDisplay.transform.position = new Vector3(this._selectedTilePosition.x, this.SelectedTileDisplay.transform.position.y, this._selectedTilePosition.y);
    }

    protected void displaySelectedTile(bool flag_)
    {
        this._selectedTileTween.To(flag_ ? 1f : 0f).Start();
        this._selectedTileSpring.To(flag_ ? 1f : 0f);
    }

    public void SetManager(FarmPatchManager manager_)
    {
        this._manager = manager_;
        foreach (FarmTile tile in Tiles)
        {
            tile.SetManager(this);
        }
    }

    public Texture GetTexture(FarmTileState state_)
    {
        return this._textureMap[state_];
    }
}
