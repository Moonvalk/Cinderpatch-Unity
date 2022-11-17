using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;
using Moonvalk.Utilities;

public class TrophyController : MonoBehaviour
{
    public GameObject[] Trophies;
    public SpriteRenderer[] TrophyLights;

    public float TrophyLightSpinSpeed = -20f;
    private float _trophyLightRotation = 0f;
    protected float[] _trophyOpacity;

    // Start is called before the first frame update
    private void Start()
    {
        this._trophyOpacity = new float[this.Trophies.Length];
        for (int index = 0; index < this._trophyOpacity.Length; index++)
        {
            this.Trophies[index].gameObject.SetActive(false);
            this._trophyOpacity[index] = 0f;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        this._trophyLightRotation += Time.deltaTime * this.TrophyLightSpinSpeed;
        if (this._trophyLightRotation > 10000f)
        {
            this._trophyLightRotation -= 10000f;
        }
    }

    private void FixedUpdate()
    {
        this.updateTrophyLights();
    }

    public void updateTrophyLights()
    {
        foreach (SpriteRenderer renderer in this.TrophyLights)
        {
            renderer.transform.rotation = Quaternion.Euler(0f, 0f, this._trophyLightRotation);
        }
    }

    public void ShowTrophy(int index_)
    {
        this.Trophies[index_].gameObject.SetActive(true);
        Tween newTween = new Tween(() => ref this._trophyOpacity[index_]);
        newTween.Duration(0.5f).Ease(Easing.Cubic.InOut);
    }
}
