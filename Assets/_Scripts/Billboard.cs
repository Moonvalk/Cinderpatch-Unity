using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Billboard : MonoBehaviour
    {
        public bool TiltTowardsCamera = false;
        protected Camera _mainCamera;

        private void Start()
        {
            this._mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            Vector3 originalAngle = transform.rotation.eulerAngles;
            transform.LookAt(this._mainCamera.transform);

            if (TiltTowardsCamera) {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, originalAngle.z);
                return;
            }
            transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.x, 0f, originalAngle.z);
        }
    }
}
