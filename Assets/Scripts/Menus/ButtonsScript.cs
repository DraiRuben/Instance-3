using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript : MonoBehaviour
{
    private string root;

    private void Start()
    {
        root = Application.persistentDataPath;
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void Load()
    {
        var files = from file in Directory.EnumerateFiles(root) select file;
        if (files.Count() > 0)
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
    public void Begin()
    {
        var files = from file in Directory.EnumerateFiles(root) select file;
        if (files.Count() > 0)
        {
            foreach (var file in files)
            {
                if(!file.EndsWith(".log"))
                    File.Delete(file);
            }
        }
        if (Directory.Exists("Game")) Directory.Delete("Game");
        SceneManager.LoadSceneAsync(1);
    }
    public void Settings()
    {
        SettingsMenu.instance.gameObject.SetActive(true);
    }

    public void Sound()
    {
        AudioMenu.instance.gameObject.SetActive(true);
    }

    public void Resume()
    {
        if (transform.parent.name == "PauseMenu")
        {
            Time.timeScale = 1;
        }
        transform.parent.gameObject.SetActive(false);
    }
}
