using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour{
    /// <summary> Load the Level1 scene </summary>
    public void StartGame() => SceneManager.LoadScene("Level1");
    /// <summary> Load the StartMenu scene </summary>
    public void MainMenu() => SceneManager.LoadScene("StartMenu");
    /// <summary> Exits the game </summary>
    public void ExitGame() => Application.Quit();
}
