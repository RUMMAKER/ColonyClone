using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public static SpawnManager singleton = null;
    private Point team1Spawn;
    private Point team2Spawn;
    private int unitIdCounter = 100; // earlier ids are used for buildings.

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    private void Start()
    {
        team1Spawn = new Point(-SceneManager.singleton.mapWidth / 2, 0);
        team2Spawn = new Point(SceneManager.singleton.mapWidth / 2, 0);
    }

    // Methods for spawning units.
    public void SpawnUnit(UnitType type, int playerId)
    {
        Point spawnPoint = team1Spawn;
        if (playerId >= 2) spawnPoint = team2Spawn;

        int xOffset = SceneManager.singleton.rng.Next(-20, 20);
        int yOffset = SceneManager.singleton.rng.Next(-150, 150);
        Point InitialPosition = new Point(spawnPoint.x + xOffset,
                                          spawnPoint.y + yOffset);

        switch (type)
        {
            case UnitType.Marine:
                GameObject obj = GameObject.Instantiate(ResourceManager.singleton.marinePrefab);
                Marine m = obj.GetComponent<Marine>();
                m.playerId = playerId;
                m.selectableId = GetNextUnitId();
                m.xOffset = xOffset;
                m.position.x = InitialPosition.x;
                m.position.y = InitialPosition.y;
                m.state = UnitState.Initial;
                obj.transform.position = m.position.ToVector3();
                SceneManager.singleton.units.Add(m.selectableId, m); // Keep reference in collection.
                return;
        }
    }

    // Helper functions.
    private int GetNextUnitId()
    {
        unitIdCounter++;
        return unitIdCounter;
    }
}
