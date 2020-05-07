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

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            Begin();
            StartCoroutine(BehaviourCycle());
        }

        protected virtual void Initialize() { }

        protected virtual void Begin() { }

        protected virtual void Main()
        {
            
        }

        protected IEnumerator BehaviourCycle()
        {
            ValidateLayer();
            while (behaviourRunning)
            {
                try
                {
                    Main();
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