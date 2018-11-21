using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Marine : Unit {

    public override int PlayerId { get; set; }
    public override int UnitId { get; set; }
    public override Point SpawnPosition { get; set; }
    public override Point Position { get; set; }
    public override UnitState State { get; set; }
    private readonly int speed = 5;

    public override void GameUpdate()
    {
        Move();
    }

    private void Move()
    {
        int direction = 1;
        int minX = SpawnPosition.x;
        int maxX = SpawnPosition.x + SceneManager.singleton.mapWidth;
        if (PlayerId >= 2)
        {
            direction = -1;
            maxX = SpawnPosition.x;
            minX = SpawnPosition.x - SceneManager.singleton.mapWidth;
        }

        switch (State)
        {
            case UnitState.Initial:
                // stuff
                return;
            case UnitState.Forward:
                Position.Add(new Point(direction * speed, 0));
                KeepInBound(minX, maxX);
                gameObject.transform.position = Position.ToVector3();
                return;
            case UnitState.Hold:
                // stuff
                return;
            case UnitState.Back:
                Position.Add(new Point(direction * -speed, 0));
                KeepInBound(minX, maxX);
                gameObject.transform.position = Position.ToVector3();
                return;
            case UnitState.Charge:
                // stuff
                return;
        }
    }
    private void KeepInBound(int minX, int maxX)
    {
        Position.x = System.Math.Max(Position.x, minX);
        Position.x = System.Math.Min(Position.x, maxX);
    }
}
