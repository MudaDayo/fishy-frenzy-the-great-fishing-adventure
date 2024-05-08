using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    

    public void NextScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

}
