using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager singleton;
    public string playerName;
    public int playerId;
    public List<Unit> selectedUnits = new List<Unit>();
    public int money = 40;
    public int power = 40;
    public int manPower = 40;
    public int influence = 40;

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }
}