using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour {

    public static GUIManager singleton = null;
    public Collider2D[] selected;

    private void Awake()
    {
        if (singleton != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        singleton = this;
        selected = new Collider2D[100];
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
