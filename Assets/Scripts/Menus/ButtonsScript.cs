using System.Drawing.Text;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript : Minigame
{
    private string root;
    private GameObject _FM;
    private GameObject _RM;
    private GameObject _CM;
    private GameObject _MM;

    private void Start()
    {
        root = Application.persistentDataPath;
        _FM = FishManager.Instance.gameObject;
        _RM = RifleMinigame.Instance.gameObject;
        _CM = Cups.Instance.gameObject;
        _MM = MoleWacker.Instance.gameObject;
    }
    public void Quit()
    {
        if (_FM.activeSelf)
        {
            _FM.GetComponent<Minigame>().TriggerMinigameEnd();
        }
        else if (_CM.activeSelf)
        {
            _CM.GetComponent<Minigame>().TriggerMinigameEnd();
        }
        else if(_RM.activeSelf)
        {
            _RM.GetComponent<Minigame>().TriggerMinigameEnd();
        }
        else if (_MM.activeSelf)
        {
            _MM.GetComponent<Minigame>().TriggerMinigameEnd();
        }
        else
        {
            Application.Quit();
        }  
    }

    public void Load()
    {
        var files = from file in Directory.EnumerateFiles(root) select file;
        foreach(var file in files)
        {
            if (file.EndsWith(".json"))
            {
                SceneManager.LoadSceneAsync(1);
                return;
            }
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
        if (Directory.Exists("Game")) Directory.Delete("Game",true);
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
