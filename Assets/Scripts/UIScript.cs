using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript:MonoBehaviour{
    public void StartGame(){SceneManager.LoadScene("Level1");}
    public void MainMenu(){SceneManager.LoadScene("StartMenu");}
    public void ExitGame(){Application.Quit();}
}
