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

        protected virtual void Initialize() { }

        protected virtual void Begin() { }

        protected virtual void Main()
        {
            
        }

        protected virtual void PhysicsMain() { }

        protected IEnumerator BehaviourUpdateCycle()
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

        protected IEnumerator BehaviourFixedUpdateCycle()
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

        public virtual void StopBehaviour()
        {
            behaviourRunning = false;
        }

        public virtual void ResumeBehaviour()
        {
            behaviourRunning = true;
        }
    }
}