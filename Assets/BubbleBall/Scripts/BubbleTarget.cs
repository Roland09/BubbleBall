using UnityEngine;

namespace BubbleBall
{
    /// <summary>
    /// There are situations where the pivot of the gameobject is on the outer bounds. This makes the wrapped gameobject spin offset, i. e. not in center.
    /// When this script is attached to the hit gameobject, then the data in there will be used, e. g. optionally that object's pivot instead of the gameobject's. Or you can specify a fixed diameter instead of having it calculated.
    /// </summary>
    public class BubbleTarget : MonoBehaviour
    {
        /// <summary>
        /// Pivot to be used for centering the gameobject inside the bubble
        /// </summary>
        public enum Pivot
        {
            Enabled,
            Disabled
        }

        /// <summary>
        /// Fixed diameter or calculated from gameobject bounds
        /// </summary>
        public enum DiameterType
        {
            Calculated,
            Fixed
        }

        /// <summary>
        /// If enabled, then this gameobject will be used as the pivot for the rotation.
        /// If disabled, then the gameobject's pivot will be used for the rotation, which isn't necessarily in the center
        /// </summary>
        public Pivot pivot = Pivot.Enabled;

        /// <summary>
        /// Whether the diameter of the encapsulating bubble should be calculated from the gameobject bounds or should be fixed
        /// </summary>
        public DiameterType diameterType = DiameterType.Calculated;

        /// <summary>
        /// The diameter to be used for fixed diameter type
        /// </summary>
        public float diameterValue = 0f;
    }
}
