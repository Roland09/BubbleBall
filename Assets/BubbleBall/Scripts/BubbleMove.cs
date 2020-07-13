using UnityEngine;

namespace BubbleBall
{
    /// <summary>
    /// Move an object constantly over time
    /// 
    ///   * Add script to an object and set x/y/z coordinates to move value(eg x = 100)
    ///   
    /// </summary>
    public class BubbleMove : MonoBehaviour
    {
        [SerializeField]
        private Vector3 moveXYZ = Vector3.zero;

        void Update()
        {
            transform.Translate(moveXYZ * Time.deltaTime);
        }
    }
}