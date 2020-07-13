using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleBall
{
    /// <summary>
    /// Perform action when the user clicks on a gameobject:
    ///   
    ///   + rotate cannon towards the target over time
    ///   + fire a bullet towards the target once the target is in range
    ///   
    /// </summary>
    public class Cannon : MonoBehaviour
    {
        public float cannonRotateSpeed = 45.0f;

        public GameObject muzzlePoint;

        public GameObject bulletPrefab;
        public float bulletVelocity = 300f;
        public float bulletLifeTime = 10f;

        private Transform target = null;

        private bool targetSelected = false;

        // consistency check during startup. if the setup is invalid nothing is performed
        private bool setupValid = true;

        void Start()
        {
            #region Consistency checks

            // muzzle point of cannon
            if (!muzzlePoint)
            {
                setupValid = false;
                Debug.LogError("MuzzlePoint missing in " + this);
            }

            // bullet prefab
            if (!bulletPrefab)
            {
                setupValid = false;
                Debug.LogError("BulletPrefab missing");
            }

            // rigidbody of bullet
            Rigidbody bulletRigidBody = bulletPrefab.GetComponent<Rigidbody>();

            if (!bulletRigidBody)
            {
                setupValid = false;
                Debug.LogError("RigidBody missing in " + bulletPrefab);
            }

            #endregion Consistency checks

        }

        void Update()
        {
            // do nothing if the setup is invalid
            if (!setupValid)
                return;

            // check for left mousebutton click
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    target = hit.transform;

                    Debug.Log("Target selected: " + target.name);

                    targetSelected = true;
                }
            }

            // target got selected, rotate towards it and fire the bullet
            if (targetSelected)
            {
                RotateTowardsTargetAndFire();
            }

        }

        /// <summary>
        /// Rotate the gameobject towards the target. Fire a bullet if the target is in rotation range.
        /// </summary>
        void RotateTowardsTargetAndFire()
        {
            GameObject child = muzzlePoint;

            // relative rotation between parent and child
            Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * child.transform.rotation;

            // target direction from the child
            Vector3 targetDir = target.transform.position - child.transform.position;

            // look rotation
            Quaternion lookRotation = Quaternion.LookRotation(targetDir);

            // rotate parent towards child's target rotation, consider relative rotation between parent and child
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation * Quaternion.Inverse(relativeRotation), cannonRotateSpeed * Time.deltaTime);

            // check if the rotation is close enough and if so fire at target
            float angleDiff = Quaternion.Angle(child.transform.rotation, lookRotation);

            // fire if target is in range
            if (angleDiff < 0.1f)
            {
                FireBullet();
            }
        }

        /// <summary>
        /// Fire the bullet and invalidate the selected target
        /// </summary>
        private void FireBullet()
        {
            // instantiate the prefab
            GameObject bulletGO = Instantiate(bulletPrefab);
            Rigidbody bulletRigidBody = bulletGO.GetComponent<Rigidbody>();

            // align prefab
            bulletGO.transform.position = muzzlePoint.transform.position;
            bulletGO.transform.rotation = transform.rotation;

            // add bullet force
            bulletRigidBody.AddForce( muzzlePoint.transform.rotation * Vector3.forward * bulletVelocity);     //If it has look at take the direction from te lookat

            // destroy the bullet after a given time
            Destroy(bulletGO, bulletLifeTime);

            // invalidate selected target so that the cannon can fire again
            targetSelected = false;
        }
    }
}
