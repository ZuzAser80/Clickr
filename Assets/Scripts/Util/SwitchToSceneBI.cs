using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToSceneBI : MonoBehaviour {
    public void Switch(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }

    public void FlipGOState(GameObject go) {
        go.SetActive(!go.activeSelf);
    }  

    public void Click() {
        FindObjectOfType<SteamFace>().PlayClick();
    }

    public void LeaveGame() => Application.Quit();
}