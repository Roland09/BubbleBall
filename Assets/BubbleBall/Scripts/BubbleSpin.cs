using UnityEngine;

namespace BubbleBall
{
    /// <summary>
    /// Spin an object constantly over time
    /// 
    ///   * Add script to an object and set x/y/z coordinates to spin value(eg x = 100)
    ///   
    /// </summary>
    public class BubbleSpin : MonoBehaviour
    {
        [SerializeField]
        private Vector3 spinXYZ = Vector3.zero;

        void Update()
        {
            transform.Rotate(spinXYZ * Time.deltaTime);
        }
    }
}