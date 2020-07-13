using UnityEngine;

namespace BubbleBall
{
    /// <summary>
    /// Check the collision of the bubble bullet with a gameobject and wrap that gameobject into a bubble.
    /// 
    /// Important: Currently for demonstration purposes the bubble collider reacts to all gameobjects. You need to adapt the code to filter out gameobjects which shouldn't be affected, e. g. consider tags or a layers.
    /// 
    /// </summary>
    public class BubbleBulletHit : MonoBehaviour
    {
        private static int LAYER_INDEX_IGNORE_RAYCAST = 2;

        /// <summary>
        /// Seconds after which this gameobject will be destroyed.
        /// </summary>
        public float lifeTime = 5f;

        /// <summary>
        /// The bubble which is being wrapped around the hit gameobject
        /// </summary>
        public GameObject bubbleBall;

        /// <summary>
        /// The explosion that's being instantiated when the bubbles hit a gameobject
        /// </summary>
        public GameObject explosion;

        /// <summary>
        /// The seconds after a collision after which the bubble particles will be destroyed
        /// </summary>
        public float particleCollisionDestroyDelay = 0.4f;

        /// <summary>
        /// Spherecast for hit detection
        /// </summary>
        public float hitRadius = 2;

        // apply this factor for calculated bubbles; use 1 in case you don't want it resized
        private float additionalBubbleSizeFactor = 1.1f;

        private Transform owner;
        private Transform hitTransform = null;
        private bool bubblePhase = false;

        public void SetOwner(Transform owner) { this.owner = owner; }

        void Start()  {

            // if the ball hasn't touch anything destroy it
            Destroy(gameObject, lifeTime); 
        }

        private void Update()
        {
            // works only on the "Enemy" tag implicitly, see check in OnTriggerEnter
            if (hitTransform == null)
                return;

            if (bubblePhase)
                return;

            // disable various behaviours (collider, rigidboy, script). otherwise the gameobject might move while the bubble turns and then it looks like it sticks out
            #region Disable behaviours

                // disable rigidbody
                Rigidbody[] rigidBodies = hitTransform.GetComponents<Rigidbody>();
                foreach (Rigidbody rigidBody in rigidBodies)
                {
                    rigidBody.isKinematic = true;
                }

                // disable colliders
                Collider[] colliders = hitTransform.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                    collider.isTrigger = true;
                }

                #region Malbers specific
                /* 
                // disable the malbers scripts
                MonoBehaviour[] mbs = hitTransform.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour item in mbs)
                {
                    if( item is MalbersAnimations.Controller.MAnimal)
                    {
                        item.enabled = false;
                    }
                    else if (item is MalbersAnimations.FollowTarget)
                    {
                        item.enabled = false;
                    }

                }
                */
                #endregion Malbers specific

            #endregion Disable behaviours

            Bounds bounds = hitTransform.GetComponentInChildren<Renderer>().bounds;

            BubbleTarget bubbleCenterGO = hitTransform.GetComponentInChildren<BubbleTarget>();

            float bubbleDiameter;
            if( bubbleCenterGO != null && bubbleCenterGO.diameterType == BubbleTarget.DiameterType.Fixed)
            {
                bubbleDiameter = bubbleCenterGO.diameterValue;
            }
            else
            {
                // calculate a surrounding scale by assuming we have a box and calculating the diagonal using sqrt(x*x+y*y+z*z)
                bubbleDiameter = Mathf.Sqrt(bounds.size.x * bounds.size.x + bounds.size.y * bounds.size.y + bounds.size.z * bounds.size.z);

                // make the bubbles a tad larger than the calculated value by this factor
                bubbleDiameter *= additionalBubbleSizeFactor;
            }

            GameObject bubblePrefab = Instantiate(bubbleBall);

            bubblePrefab.transform.position = hitTransform.position;
            bubblePrefab.transform.localScale = new Vector3(bubbleDiameter, bubbleDiameter, bubbleDiameter);

            hitTransform.SetParent( bubblePrefab.transform, true);
            hitTransform.transform.localPosition = Vector3.zero;
            hitTransform.localRotation = Quaternion.identity;

            // offset the transform in the bubble by that value
            // important in this case is to use the local scale
            if (bubbleCenterGO != null && bubbleCenterGO.pivot == BubbleTarget.Pivot.Enabled)
            {
                Vector3 bubbleCenter = bubbleCenterGO.transform.localPosition;

                Vector3 offset = new Vector3(
                    bubbleCenter.x * hitTransform.transform.localScale.x,
                    bubbleCenter.y * hitTransform.transform.localScale.y,
                    bubbleCenter.z * hitTransform.transform.localScale.z
                    );

                hitTransform.transform.localPosition = -offset;
            }

            // create explosion after a collision, e. g. sparks or a light
            if (explosion != null)
            {
                GameObject explosionGO = Instantiate(explosion);
                explosionGO.transform.position = transform.position;

                Destroy(explosionGO, 1f);
            }

            bubblePhase = true;
        }

        void OnTriggerEnter(Collider other)
        {
            // skip ignore raycast layer
            if (other.gameObject.layer == LAYER_INDEX_IGNORE_RAYCAST)
                return;

            // skip collision with self
            if (other.transform.root == owner)
                return;

            // skip irrelevant tags
            Debug.Log("Collision with a target. Please add a filter (e. g. tag or layer) in " + this);
            /*
            if (other.tag != "Enemy")
                return;
            */

            Collider[] colliders = Physics.OverlapSphere(transform.position, hitRadius);

            foreach (var nearbyObjects in colliders)
            {
                // skip instances of the same class
                if (nearbyObjects.GetComponent<BubbleBulletHit>())
                    continue;

                // set the hit transform, it'll wrap the target into a bubble in the Update method
                hitTransform = other.transform;

                break;
            }

            Destroy(gameObject, particleCollisionDestroyDelay);

        }
    }
}