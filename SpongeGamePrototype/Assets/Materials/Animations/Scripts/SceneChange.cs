using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{
    //public int test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NextSceneFuntion()
    {
        SceneManager.LoadScene(1);

    }

    public void QuitFuntion()
    {
        Debug.Log("Quit");
        Application.Quit();
        
    }
}
