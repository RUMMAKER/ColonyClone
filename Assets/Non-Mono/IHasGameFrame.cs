using UnityEngine;

// Classes that implement this interface Need to add their gameobject to LockStepManager
// gameFrameObj list and remove it from the list when they die.
public interface IHasGameFrame {
    void GameUpdate();
}
