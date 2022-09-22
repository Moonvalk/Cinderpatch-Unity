using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    public Texture2D CursorImage;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.SetCursor(this.CursorImage, Vector2.zero, CursorMode.ForceSoftware);
    }
}
