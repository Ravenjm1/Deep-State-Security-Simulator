using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyStat : MonoBehaviour
{
    private static LobbyStat _instance;
    public static LobbyStat Instance => _instance;

    public int Quota = 50;
    public bool? RichQuota;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (_instance == null)
            _instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void QuotaUp()
    {
        Quota += 50;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene")
        {
            QuotaUp();

            if (RichQuota != null)
            {
                ShowResult((bool)RichQuota);
            }
                
        }
    }
    void ShowResult(bool result)
    {
        Debug.Log("ShowResult");
        GameObject text;
        if (result == true)
        {
            text = GameObject.Find("Main Camera/GUI/Text Win");
        }
        else 
        {
            text = GameObject.Find("Main Camera/GUI/Text Lose");
        }

        text.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
