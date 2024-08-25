using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToSceneBI : MonoBehaviour {
    public void Switch(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }
}