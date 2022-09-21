using UnityEngine;
using Moonvalk.Animation;
using Moonvalk.Utilities;

public class TestSpring : MonoBehaviour
{
    protected Vector3 _position;
    protected Vector3 _scale;
    protected Vector3 _rotation;
    public SpringProperties Properties;

    public void Start()
    {
        this._position = new Vector3(2f, 3f, 0f);
        this._scale = new Vector3(1f, 1f, 1f);
        this._rotation = new Vector3(0f, 0f, 0f);

        SpringVec3 newSpring = new SpringVec3(() => ref this._position, () => ref this._scale, () => ref this._rotation);
        this.Properties.AddSpring(newSpring);

        MicroTimer timer = new MicroTimer(0.1f);
        timer.OnComplete(() => {
            
            newSpring.To(new Vector3(0f, 3f, -6f), new Vector3(2f, 2f, 2f), new Vector3(0f, 20f, 360f)).OnComplete(() => {
                newSpring.To(new Vector3(2f, 3f, 0f), new Vector3(1f, 1f, 1f), new Vector3(60f, 0f, 0f)).OnComplete(() => {
                    timer.Start();
                });
            });
        }).Start();
    }

    public void Update()
    {
        transform.position = this._position;
        transform.localScale = this._scale;
        transform.rotation = Quaternion.Euler(this._rotation.x, this._rotation.y, this._rotation.z);
    }
}