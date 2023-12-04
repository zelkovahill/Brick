using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{

    public void LoadScene(string SceneName)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.QQ);
        SceneManager.LoadScene(SceneName);
    }
    
    public void QuitGame()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.a);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  
        
#endif
        Application.Quit();
    }
    
}
