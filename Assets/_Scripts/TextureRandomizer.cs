using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureRandomizer : MonoBehaviour
{
    public TextureGroup Group;

    // Start is called before the first frame update
    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = new Material(renderer.material);

        Texture image = this.Group.Textures[Random.Range(0, this.Group.Textures.Count - 1)];
        renderer.material.mainTexture = image;
    }
}
