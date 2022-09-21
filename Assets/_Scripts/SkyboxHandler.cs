using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxHandler : MonoBehaviour
{
    public Material SkyboxMaterial;
    public float RotationSpeed = 1.2f;
    protected float _startingRotation;
    
    private void Start()
    {
        this._startingRotation = Random.Range(0f, 360f / this.RotationSpeed);
    }

    // Update is called once per frame
    private void Update()
    {
        this.SkyboxMaterial.SetFloat("_Rotation", (Time.time + this._startingRotation) * RotationSpeed);
    }
}
