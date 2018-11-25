using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE {

    public int radius;
    public Point position;
    private HitBox[] hitboxes;

    public AOE(int radius, Point position)
    {
        this.position = position;
        this.radius = radius;
        hitboxes = new HitBox[3];
        hitboxes[0] = new HitBox(radius/2, radius, new Point(0, 0));
        hitboxes[1] = new HitBox(radius, radius/2, new Point(0, 0));
        hitboxes[2] = new HitBox(radius/3, radius/3, new Point(0, 0));
    }

    public bool IsTouching(Unit u)
    {
        foreach (HitBox myBox in hitboxes)
        {
            int myX = myBox.relativePosition.x + position.x;
            int myY = myBox.relativePosition.y + position.y;
            foreach (HitBox theirBox in u.hitboxes)
            {
                int theirX = theirBox.relativePosition.x + u.position.x;
                int theirY = theirBox.relativePosition.y + u.position.y;
                bool touching = (System.Math.Abs(myX - theirX) * 2 < (myBox.width + theirBox.width))
                             && (System.Math.Abs(myY - theirY) * 2 < (myBox.height + theirBox.height));
                if (touching)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void DebugDrawHitBox()
    {
        foreach (HitBox b in hitboxes)
        {
            b.DebugDrawHitbox(position);
        }
    }
}
