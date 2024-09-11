using System;
using UnityEngine;

public class NonMonoGame
{
    private NonUnityGameObject sphere;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]

    public static void StartMyGame() 
    {
        new NonMonoGame();
    }

    public NonMonoGame() 
    {
        Debug.Log("My game started");
        Application.onBeforeRender += MyRenderMethod;
        sphere = new NonUnityGameObject("Player", new Vector2(-5f, 3f));    
    }

    private void MyRenderMethod()
    {
        Debug.Log("I'm rendering");

        sphere.CustomUpdate(Time.deltaTime);
    }
}
