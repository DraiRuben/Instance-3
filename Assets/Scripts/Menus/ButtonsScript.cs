using System.IO;
using UnityEngine;
using System.Linq;
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
                File.Delete(file);
            }
        }
        SceneManager.LoadSceneAsync(1);
    }
    public void Settings()
    {

    }
}
