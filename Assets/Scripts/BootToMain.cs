using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootToMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Boot", 1f);
    }

    public void Boot()
    {
        SceneManager.LoadScene(1);
    }
}
