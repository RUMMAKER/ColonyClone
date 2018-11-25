using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Unit : SelectableBehaviour, IHasGameUpdate
{
    public HitBox[] hitboxes;
    public int xOffset; // offset at spawn.
    public Point position;
    public UnitState state;
    public int speed;
    private SpriteRenderer sr;

    protected virtual void Awake()
    {
        position = new Point(0,0);
        hitboxes = new HitBox[0];
        selectableName = "Unit";
        selectableDesc = "Unit Desc";
        sr = GetComponent<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        SetColorBasedOnPlayerId();
    }
    private void SetColorBasedOnPlayerId()
    {
        if (playerId == 0)
        {
            sr.color = Color.red;
        }
        if (playerId == 1)
        {
            sr.color = Color.yellow;
        }
        if (playerId == 2)
        {
            sr.color = Color.green;
        }
        if (playerId == 3)
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
        if (playerId >= 2) direction = -1;

        switch (state)
        {
            case UnitState.Initial:
                return;
            case UnitState.Forward:
                position.Add(new Point(direction * speed, 0));
                KeepInBound();
                gameObject.transform.position = position.ToVector3();
                return;
            case UnitState.Hold:
                return;
            case UnitState.Back:
                position.Add(new Point(-direction * speed, 0));
                KeepInBound();
                gameObject.transform.position = position.ToVector3();
                return;
            case UnitState.Charge:
                return;
        }
    }
    private void KeepInBound()
    {
        int leftBorder = (-SceneManager.singleton.mapWidth / 2) + xOffset;
        int rightBorder = (SceneManager.singleton.mapWidth / 2) + xOffset;
        if (position.x < leftBorder)
        {
            position.x = leftBorder;
        }
        if (position.x > rightBorder)
        {
            position.x = rightBorder;
        }
    }
    public override void OnSelect()
    {
        base.OnSelect();
        sr.color = Color.white;
    }
    public override void OnDeSelect()
    {
        base.OnSelect();
        SetColorBasedOnPlayerId();
    }
    public override UIAction[] GetActions()
    {
        int[] unitIds = GUIManager.singleton.selected.Select(x => x.selectableId).ToArray();
        UIAction[] actions = new UIAction[4];
        actions[0] = new UIAction("Attack", 
            "Move forward and attack enemies.", 
            ResourceManager.singleton.forwardImg, 
            new ActionSetUnitState(UnitState.Forward, unitIds));
        actions[1] = new UIAction("Hold",
            "Hold position and defend.",
            ResourceManager.singleton.holdImg,
            new ActionSetUnitState(UnitState.Hold, unitIds));
        actions[2] = new UIAction("Retreat",
            "Fall back.",
            ResourceManager.singleton.backImg,
            new ActionSetUnitState(UnitState.Back, unitIds));
        actions[3] = new UIAction("Charge",
            "Command units to ignore enemies and attack control center.",
            ResourceManager.singleton.chargeImg,
            new ActionSetUnitState(UnitState.Charge, unitIds));
        return actions;
    }

    // debug draw hitbox.
    void OnGUI()
    {
        foreach (HitBox b in hitboxes)
        {
            b.DebugDrawHitbox(position);
        }
    }
}
