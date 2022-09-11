using UnityEngine;
using System.Collections.Generic;

namespace Moonvalk.Animation
{
    [System.Serializable]
    public class SpringProperties : MonoBehaviour
    {
        /// <summary>
        /// Tension to be applied to all springs.
        /// </summary>
        public float Tension = 0.5f;

        /// <summary>
        /// Dampening to be applied to all springs.
        /// </summary>
        public float Dampening = 0.05f;
        
        /// <summary>
        /// A list of all springs that this properties object will update settings for.
        /// </summary>
        private List<ISpring> springs;

        /// <summary>
        /// Unity Event - Occurs once when this object is first loaded.
        /// </summary>
        private void OnAwake()
        {
            this.springs = new List<ISpring>();
        }

        /// <summary>
        /// Unity Event - Occurs whenever a value changes in inspector.
        /// </summary>
        private void OnValidate()
        {
            if (this.springs != null)
            {
                foreach(ISpring spring in springs)
                {
                    spring.Dampening(this.Dampening);
                    spring.Tension(this.Tension);
                }
            }
        }

        /// <summary>
        /// Adds a new Spring object to be updated with new settings.
        /// </summary>
        /// <param name="spring_">The Spring object.</param>
        public void AddSpring(ISpring spring_)
        {
            if (this.springs == null)
            {
                this.springs = new List<ISpring>();
            }
            this.springs.Add(spring_);
        }
    }
}