using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    public static SceneManager singleton = null;
    public List<IHasGameUpdate> gameUpdateObjs;
    public List<IAction> actions;

    void Awake () {
        if (singleton != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        singleton = this;
        gameUpdateObjs = new List<IHasGameUpdate>();
        actions = new List<IAction>();
    }
	
	public void AddAction(IAction a)
    {
        actions.Add(a);
    }
}
