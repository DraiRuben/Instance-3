using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsScript : Minigame
{
    private string root;
    private bool _HasSave;
    [SerializeField] private ConfirmationPrompt _NewGamePrompt;
    [SerializeField] private Button _LoadButton;
    private GameObject _FM;
    private GameObject _RM;
    private GameObject _CM;
    private GameObject _MM;
    private bool test = true;
    private void Start()
    {

        root = Application.persistentDataPath;
        _FM = FishManager.Instance != null ? FishManager.Instance.gameObject : null;
        _RM = RifleMinigame.Instance != null ? RifleMinigame.Instance.gameObject : null;
        _CM = Cups.Instance != null ? Cups.Instance.gameObject : null;
        _MM = MoleWacker.Instance != null ? MoleWacker.Instance.gameObject : null;

        root = Application.persistentDataPath;
        System.Collections.Generic.IEnumerable<string> files = from file in Directory.EnumerateFiles(root) select file;
        foreach (string file in files)
        {
            if (file.EndsWith(".json"))
            {
                _HasSave = true;
                return;
            }
        }
        _HasSave = false;
        if (!_HasSave && _LoadButton) _LoadButton.interactable = false;
    }
    public void Quit()
    {
        AudioManager._Instance.PlaySFX("uiClick");
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (_FM.activeSelf)
            {
                _FM.GetComponent<Minigame>().TriggerMinigameEnd(test);
            }
            else if (_CM.activeSelf)
            {
                _CM.GetComponent<Minigame>().TriggerMinigameEnd(test);
            }
            else if (_RM.activeSelf)
            {
                _RM.GetComponent<Minigame>().TriggerMinigameEnd(test);
            }
            else if (_MM.activeSelf)
            {
                _MM.GetComponent<Minigame>().TriggerMinigameEnd(test);
            }
            if (PauseMenu.instance.gameObject.activeSelf)
            {
                PauseMenu.instance.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
        Application.Quit();
    }

    public void Load()
    {
        AudioManager._Instance.PlaySFX("uiClick");
        if (_HasSave)
        {
            SceneManager.LoadSceneAsync(1);
            return;
        }
    }
    public void Begin()
    {
        AudioManager._Instance.PlaySFX("uiClick");
        //shows confirmation prompt in case the user already has a save file
        if (_HasSave)
        {
            _NewGamePrompt.OpenConfirmationPrompt(
            () =>
            {
                System.Collections.Generic.IEnumerable<string> files = from file in Directory.EnumerateFiles(root) select file;
                if (files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        if (!file.EndsWith(".log"))
                            File.Delete(file);
                    }
                }
                if (Directory.Exists("Game")) Directory.Delete("Game", true);
                this.Invoke(() => SceneManager.LoadSceneAsync(1), 0.4f / 0.6f);
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
        AudioManager._Instance.PlaySFX("uiClick");
        SettingsMenu.instance.gameObject.SetActive(true);
    }

    public void Sound()
    {
        AudioManager._Instance.PlaySFX("uiClick");
        AudioMenu.instance.gameObject.SetActive(true);
    }
    public void Controls()
    {
        ControlsMenu.Instance.gameObject.SetActive(true);
    }
    public void Resume()
    {
        AudioManager._Instance.PlaySFX("uiClick");
        if (transform.parent.name == "PauseMenu")
        {
            Time.timeScale = 1;
        }
        transform.parent.gameObject.SetActive(false);
    }
}
