using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalDebug : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            ClearPlayerPrefs();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ClearPlayerPrefs();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.DeleteKey("NotebookText");
            PlayerPrefs.Save();

            Debug.Log("Notebook borrado");
        }
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("playerprefs limpios");
    }
}