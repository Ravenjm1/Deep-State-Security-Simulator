using UnityEngine;
using Steamworks;
using UnityEngine.PlayerLoop;
using System;

public class SteamManager : MonoBehaviour
{
    private static SteamManager _instance;
    public static SteamManager Instance => _instance;

    public static bool Initialized { get; private set; }

    public static event Action OnInitialized = delegate {  };

    private void Start()
    {
        if (_instance != this)
            return;
            
        DontDestroyOnLoad(gameObject);

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize Test failed. Incorrect version of Steamworks.NET.");
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks.NET] DllCheck Test failed. One or more of the Steamworks binaries seems to be the wrong version.");
        }

        try
        {
            if (!SteamAPI.Init())
            {
                Debug.LogError("SteamAPI_Init() failed.");
                return;
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib.");
            Debug.LogError(e);
            return;
        }

        OnInitialized();

        Initialized = true;
    }
    private void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (_instance != this)
        {
            return;
        }

        SteamAPI.Shutdown();
    }

    private void Update()
    {
        if (Initialized)
        {
            SteamAPI.RunCallbacks();
        }
    }
}
