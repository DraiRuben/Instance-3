using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript : MonoBehaviour
{
    private string root;
    private bool _HasSave;
    [SerializeField] private ConfirmationPrompt _NewGamePrompt;
    private void Start()
    {
        root = Application.persistentDataPath;
        var files = from file in Directory.EnumerateFiles(root) select file;
        foreach (var file in files)
        {
            if (file.EndsWith(".json"))
            {
                _HasSave = true;
                return;
            }
        }
        _HasSave = false;
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void Load()
    {
        if (_HasSave)
        {
            SceneManager.LoadSceneAsync(1);
            return;
        }
    }
    public void Begin()
    {
        //shows confirmation prompt in case the user already has a save file
        if (_HasSave)
        {
            _NewGamePrompt.OpenConfirmationPrompt(
            () =>
            {
                var files = from file in Directory.EnumerateFiles(root) select file;
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        if (!file.EndsWith(".log"))
                            File.Delete(file);
                    }
                }
                if (Directory.Exists("Game")) Directory.Delete("Game", true);
                SceneManager.LoadSceneAsync(1);
                _NewGamePrompt.ChangePromptState();
            }, 
            () => 
            {
                _NewGamePrompt.ChangePromptState();
            }, 
            "Si vous continuez, toute progression sera effacée !");
        }
        else
        {
            SceneManager.LoadSceneAsync(1);
        }
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
