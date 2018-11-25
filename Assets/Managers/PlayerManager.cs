using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public class Resources
    {
        public int money = 40;
        public int power = 40;
        public int manPower = 40;
        public int influence = 40;
    }
    public static PlayerManager singleton;
    public string playerName;
    public int playerId;
    public Resources[] playerResources = new Resources[4]; // tracks resources for each player.

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