using System.Collections;
using System.IO;
using UnityEngine;

namespace DSL
{
    //The DSL equivalent to MonoBehaviour
    //All objects on the "DSL" layer, will be controlled by
    //the Dialogue System
    public abstract class DSLBehaviour : MonoBehaviour
    {
        protected bool behaviourRunning = true;

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void Start()
        {
            Begin();
            StartCoroutine(BehaviourUpdateCycle());
            StartCoroutine(BehaviourFixedUpdateCycle());
        }

        /// <summary>
        /// The start of an objects life.
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// What happens after the object's been created
        /// </summary>
        protected virtual void Begin() { }

        /// <summary>
        /// The main function of an object
        /// </summary>
        protected virtual void Main() { }

        /// <summary>
        /// The main function of an object based on physics
        /// </summary>
        protected virtual void PhysicsMain() { }

        /// <summary>
        /// Stop the behaviour of an object
        /// </summary>
        public virtual void StopBehaviour()
        {
            behaviourRunning = false;
        }

        /// <summary>
        /// Continue the behaviour of an object
        /// </summary>
        public virtual void ResumeBehaviour()
        {
            behaviourRunning = true;
        }

        /// <summary>
        /// The cycle for main
        /// </summary>
        /// <returns></returns>
        private IEnumerator BehaviourUpdateCycle()
        {
            ValidateLayer();
            while (true)
            {
                try
                {
                    if(behaviourRunning) Main();
                }
                catch (IOException ioException)
                {
                    behaviourRunning = false;
                    Debug.Log("Behaviour is turned off...");
                    throw ioException;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// The cycle of physics main
        /// </summary>
        /// <returns></returns>
        private IEnumerator BehaviourFixedUpdateCycle()
        {
            ValidateLayer();
            while (true)
            {
                try
                {
                    if (behaviourRunning) PhysicsMain();
                }
                catch (IOException ioException)
                {
                    behaviourRunning = false;
                    Debug.Log("Behaviour is turned off...");
                    throw ioException;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Validate the object's layer
        /// </summary>
        private void ValidateLayer()
        {
            Component[] objDSLBehaviour = GetComponents(typeof(DSLBehaviour));

            foreach (Component component in objDSLBehaviour) {
                if (gameObject.layer != LayerMask.NameToLayer(DialogueSystem.DSL_LAYER) && component.GetType().IsSubclassOf(typeof(DSLBehaviour)))
                {
                    behaviourRunning = false;
                    throw new InvalidLayerException("This object is running DSLBehaviour, but it's set to the wrong layer.");
                }
            }

        }
    }
}