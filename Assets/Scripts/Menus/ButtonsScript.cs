using System.Drawing.Text;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript : Minigame
{
    private string root;
    private bool _HasSave;
    [SerializeField] private ConfirmationPrompt _NewGamePrompt;
    private GameObject _FM;
    private GameObject _RM;
    private GameObject _CM;
    private GameObject _MM;
    private bool test = true;
    private void Start()
    {
       
        root = Application.persistentDataPath;
        _FM = FishManager.Instance?.gameObject;
        _RM = RifleMinigame.Instance?.gameObject;
        _CM = Cups.Instance?.gameObject;
        _MM = MoleWacker.Instance?.gameObject;

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
        if (_FM.activeSelf)
        {
            _FM.GetComponent<Minigame>().TriggerMinigameEnd(test);
        }
        else if (_CM.activeSelf)
        {
            _CM.GetComponent<Minigame>().TriggerMinigameEnd(test);
        }
        else if(_RM.activeSelf)
        {
            _RM.GetComponent<Minigame>().TriggerMinigameEnd(test);
        }
        else if (_MM.activeSelf)
        {
            _MM.GetComponent<Minigame>().TriggerMinigameEnd(test);
        }
        else
        {
            Application.Quit();
        }
        if (PauseMenu.instance.gameObject.activeSelf)
        {
            PauseMenu.instance.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
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
            "Si vous continuez, toute progression sera effac�e !");
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
    public void Controls()
    {
        ControlsMenu.Instance.gameObject.SetActive(true);
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
