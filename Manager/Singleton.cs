using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_Instance;
    public static T Instance
    {
        get
        {
            if (s_Instance != null)
            {
                Debug.Log("DEBUG: Singleton [case1]: returning loaded singleton");
                return s_Instance;      // Early return the created instance 
            }

            T[] objectResult = FindObjectsOfType<T>();
            Debug.Log("DEBUG: Singleton FindObjectsOfType.count=" + objectResult.Length);
            // Find the active singleton already created

            s_Instance = FindObjectOfType<T>();       // note: this is use during the Awake() logic
            if (s_Instance != null)
            {
                Debug.Log("DEBUG:Singleton [case2]: Found the active object in memory");
                return s_Instance;
            }

            Create();     // create new game object 

            return s_Instance;
        }
    }

    public void UpdateSingletonName()
    {
        gameObject.name = GetSingletonName();
    }

    // 
    static void Create()
    {
        Debug.Log("DEBUG: Singleton [case3]: Create new Singleton Object");
        GameObject singletonObject = new GameObject("Singleton");
        s_Instance = singletonObject.AddComponent<T>();
    }

    string InfoGameObject()
    {
        return GetGameObjectInfo(gameObject);
    }

    public static string GetGameObjectInfo(GameObject obj)
    {
        return "(" + obj.GetInstanceID() + ":" + obj.name + ")";
    }

    protected abstract string GetSingletonName();
    protected virtual bool ShouldDestroyOnLoad()
    {
        return false;   // false=Keep object even loading a new scene
    }

    protected virtual void DidAwake()
    {

    }

    void Awake()
    {
        Debug.Log("DEBUG: MonoSingleton: Awake() begin. " + InfoGameObject());
        if (Instance != this)
        {
            Debug.Log("DEBUG: Singleton: will destroy the extra gameObject ");
            Destroy(gameObject);
            return;
        }

        // Start Initialization
        UpdateSingletonName();

        Debug.Log("DEBUG: GameCounter: Awake Logic");
        if (ShouldDestroyOnLoad() == false)
        {
            DontDestroyOnLoad(gameObject);      // note: s_Instance become null when switch scene if not place this line of code
        }

        DidAwake();
    }
}