using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    public static SceneManager singleton = null;
    public const int integerScale = 100; // int 1 == float 0.01f
    public int mapWidth;

    public int seed;
    public System.Random rng;
    public bool gameStarted = false;
    
    // Use SortedDictionary because iterating has order.
    public SortedDictionary<int, Unit> units = new SortedDictionary<int, Unit>();
    public SortedDictionary<int, Building> buildings = new SortedDictionary<int, Building>();

    void Awake () {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }
}
