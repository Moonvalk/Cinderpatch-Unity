using UnityEngine;
using Moonvalk.Animation;

public class TestTweenBehavior : MonoBehaviour
{
    protected Vector3 _position;

    // Start is called before the first frame update
    private void Start()
    {
        this._position = new Vector3();

        // Tween start.
        TweenVec3 inTween = new TweenVec3(() => ref this._position);
            inTween.To(new Vector3(3f, 2f, -5f)).Duration(2f).Ease(Easing.Elastic.Out).OnComplete(() => {
                
                // Tween back.
                TweenVec3 outTween = new TweenVec3(() => ref this._position);
                    outTween.To(new Vector3(-3f, 4f, -2f)).Duration(2f).Ease(Easing.Elastic.Out).OnComplete(() => {
                        inTween.Start();
                    }).Start();

            }).Start();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = this._position;
    }
}
