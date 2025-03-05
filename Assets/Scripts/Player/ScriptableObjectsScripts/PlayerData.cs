using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerInfo/Player Data")]
public class PlayerData : ScriptableObject
{
    // Elements
    public PlayerBehaviour.Element currentElement = PlayerBehaviour.Element.None;

    // Health
    public int health = 5;
    public int heartsCount = 5;

    // Aura
    public float aura = 0f;
    public float maxAura = 20f;

    // Checkpoint
    [System.Serializable]
    public class Checkpoint
    {
        public bool respawn;
        public string scene;
        public Vector2 position;

        public Checkpoint(bool respawn, string scene, Vector2 position)
        {
            this.respawn = respawn;
            this.scene = scene;
            this.position = position;
        }
    }
    public Checkpoint lastCheckpoint = new Checkpoint(false, "TutorialZone", new Vector2(0f, -2.1f));

    // Travel Zone
    [System.Serializable]
    public class TravelZone
    {
        public bool travel;
        public string newScene;
        public Vector2 newPosition;
        public float newDirection;

        public TravelZone(bool travel, string newScene, Vector2 newPosition, float newDirection)
        {
            this.travel = travel;
            this.newScene = newScene;
            this.newPosition = newPosition;
            this.newDirection = newDirection;
        }
    }
    public TravelZone newTravelZone = new TravelZone(false, "TutorialZone", new Vector2(0f, -2.1f), 1f);

    public void Reset()
    {
        currentElement = PlayerBehaviour.Element.None;
        health = heartsCount;
        aura = 0f;
    }
}
