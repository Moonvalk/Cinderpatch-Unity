using UnityEngine;

namespace Moonvalk.Utilities
{
    /// <summary>
    /// A container for holding basic Transform data.
    /// </summary>
    public class TransformLite
    {
        /// <summary>
        /// The position of this Transform.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The rotation of this Transform.
        /// </summary>
        public Quaternion Rotation;

        /// <summary>
        /// The scale of this Transform.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TransformLite()
        {
            this.Position = new Vector3();
            this.Rotation = new Quaternion();
            this.Scale = new Vector3(1f, 1f, 1f);
        }

        /// <summary>
        /// Constructor which copies data from an existing Transform.
        /// </summary>
        /// <param name="transformToCopy_">The Transform object to copy.</param>
        public TransformLite(Transform transformToCopy_)
        {
            this.Copy(transformToCopy_);
        }

        /// <summary>
        /// Copies all Transform data to this container.
        /// </summary>
        /// <param name="transformToCopy_">The Transform object to copy.</param>
        public void Copy(Transform transformToCopy_)
        {
            this.Position = transformToCopy_.position;
            this.Rotation = transformToCopy_.rotation;
            this.Scale = transformToCopy_.localScale;
        }
    }
}