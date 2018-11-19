using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    public GameObject marinePrefab;

    public static SceneManager singleton = null;
    public System.Random rng = null;
    private int unitIdCounter = 0;

    // SortedDictionary allows quick lookup of object while keeping order (determinism is important for lockstep).
    public SortedDictionary<int, IHasGameUpdate> gameUpdateObjs;

    public List<IAction> actions;
    public bool gameStarted = false;

    void Awake () {
        if (singleton != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        singleton = this;
        gameUpdateObjs = new SortedDictionary<int, IHasGameUpdate>();
        actions = new List<IAction>();
    }

    void Update()
    {
        if (!gameStarted) return;
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddAction(new CreateUnit(UnitType.Marine, 0));
        }
    }
    void OnGUI()
    {
        if (!gameStarted) return;
        GUI.Label(new Rect(2, 30, 300, 100), "Press G to spawn marine(test)");
    }

    public void AddAction(IAction a)
    {
        actions.Add(a);
    }

    public void SpawnMarine(int playerId)
    {
        GameObject obj = GameObject.Instantiate(marinePrefab);
        obj.transform.position = new Vector3((float)rng.NextDouble()*2-1f, (float)rng.NextDouble()*2-1f, 0);
        Marine m = obj.GetComponent<Marine>();
        m.PlayerId = playerId;
        m.UnitId = unitIdCounter;
        unitIdCounter++;
        gameUpdateObjs.Add(m.UnitId, m);
    }
}
