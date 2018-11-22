using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IHasGameUpdate
{
    public virtual int PlayerId { get; set; }
    public virtual int UnitId { get; set; }
    public virtual int XOffset { get; set; }
    public virtual Point Position { get; set; }
    public virtual UnitState State { get; set; }
    public virtual int Speed { get; set; }

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (PlayerId == 0)
        {
            sr.color = Color.red;
        }
        if (PlayerId == 1)
        {
            sr.color = Color.yellow;
        }
        if (PlayerId == 2)
        {
            sr.color = Color.green;
        }
        if (PlayerId == 3)
        {
            sr.color = Color.blue;
        }
    }

    public virtual void GameUpdate()
    {
        Move();
    }

    private void Move()
    {
        int direction = 1;
        if (PlayerId >= 2) direction = -1;

        switch (State)
        {
            case UnitState.Initial:
                return;
            case UnitState.Forward:
                Position.Add(new Point(direction * Speed, 0));
                KeepInBound();
                gameObject.transform.position = Position.ToVector3();
                return;
            case UnitState.Hold:
                return;
            case UnitState.Back:
                Position.Add(new Point(-direction * Speed, 0));
                KeepInBound();
                gameObject.transform.position = Position.ToVector3();
                return;
            case UnitState.Charge:
                return;
        }
    }
    private void KeepInBound()
    {
        int leftBorder = (-SceneManager.singleton.mapWidth / 2) + XOffset;
        int rightBorder = (SceneManager.singleton.mapWidth / 2) + XOffset;
        if (Position.x < leftBorder)
        {
            Position.x = leftBorder;
        }
        if (Position.x > rightBorder)
        {
            Position.x = rightBorder;
        }
    }
}
