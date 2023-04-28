using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour {
    protected static T instance = null;
    protected static object mInstanceLock = new object();
    protected static bool mDestroyedOnApplicationQuit = false;

    private Transform mTransform = null;
    private GameObject mGameObject = null;

    private bool mIsInitialized = false;
    private bool mIsForcefullyDestroyed = false;

    /// <summary>
    /// Gets the singleton instance which will be persistent until Application quits.
    /// </summary>
    /// <value>The instance.</value>
    public static T Singleton
    {
        get
        {
            System.Type _singletonType = typeof(T);

            // We are requesting an instance after application is quit
            if (mDestroyedOnApplicationQuit) {
                Debug.LogWarning("[SingletonPattern] " + _singletonType + " instance is already destroyed.");
                return null;
            }

            lock (mInstanceLock) {
                if (instance == null) {
                    // Get all the instances that exist in the screen
                    T[] _monoComponents = FindObjectsOfType(_singletonType) as T[];

                    if (_monoComponents.Length > 0) {
                        instance = _monoComponents[0];

                        for (int iter = 1; iter < _monoComponents.Length; iter++)
                            Destroy(_monoComponents[iter].gameObject);
                    }

                    // We need to create new instance
                    if (instance == null) {
                        instance = new GameObject().AddComponent<T>();
                        // Update name 
                        instance.name = _singletonType.Name;
                    }
                }
            }

            // Check if component is initialized or not
            SingletonMono<T> _singletonInstance = (SingletonMono<T>)(object)instance;

            if (!_singletonInstance.mIsInitialized) {
                _singletonInstance.Init();
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    public Transform cachedTransform
    {
        get
        {
            if (mTransform == null)
                mTransform = transform;

            return mTransform;
        }
    }

    public GameObject cachedGameObject
    {
        get
        {
            if (mGameObject == null)
                mGameObject = gameObject;

            return mGameObject;
        }
    }

    protected virtual void Start() { }

    protected virtual void Reset() {
        // Reset properties
        mGameObject = null;
        mTransform = null;
        mIsInitialized = false;
        mIsForcefullyDestroyed = false;
    }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }

    protected virtual void OnDestroy() {
        // Singleton instance means same instance will run throughout the gameplay session
        // If its destroyed that means application is quit
        if (instance == this && !mIsForcefullyDestroyed)
            mDestroyedOnApplicationQuit = true;
    }

    protected virtual void Init() {
        // Set as initialized
        mIsInitialized = true;

        // Just in case, handling so that only one instance is alive
        if (instance == null) {
            instance = this as T;
        }
        // Destroying the reduntant copy of this class type
        else if (instance != this) {
            Destroy(cachedGameObject);
            return;
        }

        // Set it as persistent object
        DontDestroyOnLoad(cachedGameObject);
    }

    public void ForceDestroy() {
        // Mark that object was forcefully destroyed
        mIsForcefullyDestroyed = true;

        // Destory
        Destroy(cachedGameObject);
    }
}