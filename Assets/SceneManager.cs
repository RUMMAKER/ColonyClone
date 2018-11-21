using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    public static SceneManager singleton = null;
    public const int integerScale = 100; // int 1 == float 0.01f
    public int mapWidth;
    private Point team1Spawn;
    private Point team2Spawn;
    public GameObject marinePrefab;
    private int unitIdCounter = 0;
    public System.Random rng = null;
    public bool gameStarted = false;
    // SortedDictionary allows quick lookup of object while keeping order (determinism is important for lockstep).
    public SortedDictionary<int, IHasGameUpdate> gameUpdateObjs = new SortedDictionary<int, IHasGameUpdate>();
    public List<ILockStepAction> actions = new List<ILockStepAction>();

    void Awake () {
        if (singleton != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        singleton = this;
        team1Spawn = new Point(-mapWidth/2, 0);
        team2Spawn = new Point(mapWidth/2, 0);
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

    public void AddAction(ILockStepAction a)
    {
        actions.Add(a);
    }

    // Methods for spawning units.
    public void SpawnMarine(int playerId)
    {
        Point spawnPoint = team1Spawn;
        if (playerId >= 2) spawnPoint = team2Spawn;
        Point InitialPosition = new Point(spawnPoint.x + rng.Next(-20, 20),
                                          spawnPoint.y + rng.Next(-150, 150));

        GameObject obj = GameObject.Instantiate(marinePrefab);
        Marine m = obj.GetComponent<Marine>();
        m.PlayerId = playerId;
        m.UnitId = GetNextUnitId();
        m.SpawnPosition = InitialPosition;
        m.Position = InitialPosition;
        m.State = UnitState.Initial;
        obj.transform.position = m.Position.ToVector3();
        gameUpdateObjs.Add(m.UnitId, m); // Keep reference in collection.
    }

    // Helper functions.
    private int GetNextUnitId()
    {
        unitIdCounter++;
        return unitIdCounter;
    }
}
