using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public Player_Pawn player;

    private GameObject objectToSpawn;

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        } else
        {
            Destroy(gameObject);
        }
 
    }

    #endregion

    // Start is called before the first frame update

    void Start()
    {
        player = FindObjectOfType<Player_Pawn>();

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools) //For each pool that we create
        {
            Queue<GameObject> objectPool = new Queue<GameObject>(); //We create a queue full of objects

            //We make sure that we want to add all of the objects into the queue
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                DontDestroyOnLoad(obj);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool); //Add our queue into our dictionary
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player_Pawn>();
            
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag: " + tag + " doesn't exist");
            return null;
        }



        objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        Vector3 scalex = objectToSpawn.transform.localScale;
        scalex.x = player.gameObject.transform.localScale.x;
        objectToSpawn.transform.localScale = scalex;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;

    }
}
