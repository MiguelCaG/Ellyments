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

        public bool CheckpointEquals(string scene, Vector2 position) { return (this.scene == scene && this.position == position); }
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

    public void SaveState()
    {
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetInt("HeartsCount", heartsCount);
        PlayerPrefs.SetFloat("Aura", aura);
        PlayerPrefs.SetFloat("MaxAura", maxAura);
        PlayerPrefs.SetInt("CurrentElement", (int)currentElement);

        // Save Checkpoint
        PlayerPrefs.SetInt("Checkpoint_Respawn", lastCheckpoint.respawn ? 1 : 0);
        PlayerPrefs.SetString("Checkpoint_Scene", lastCheckpoint.scene);
        PlayerPrefs.SetFloat("Checkpoint_X", lastCheckpoint.position.x);
        PlayerPrefs.SetFloat("Checkpoint_Y", lastCheckpoint.position.y);

        // Save Travel Zone
        PlayerPrefs.SetInt("TravelZone_Travel", newTravelZone.travel ? 1 : 0);
        PlayerPrefs.SetString("TravelZone_Scene", newTravelZone.newScene);
        PlayerPrefs.SetFloat("TravelZone_X", newTravelZone.newPosition.x);
        PlayerPrefs.SetFloat("TravelZone_Y", newTravelZone.newPosition.y);
        PlayerPrefs.SetFloat("TravelZone_Direction", newTravelZone.newDirection);

        PlayerPrefs.Save();
    }

    public void LoadState()
    {
        health = PlayerPrefs.GetInt("Health", heartsCount);
        heartsCount = PlayerPrefs.GetInt("HeartsCount", heartsCount);
        aura = PlayerPrefs.GetFloat("Aura", 0f);
        maxAura = PlayerPrefs.GetFloat("MaxAura", 20f);
        currentElement = (PlayerBehaviour.Element)PlayerPrefs.GetInt("CurrentElement", (int)PlayerBehaviour.Element.None);

        // Load Checkpoint
        bool respawn = PlayerPrefs.GetInt("Checkpoint_Respawn", 0) == 1;
        string scene = PlayerPrefs.GetString("Checkpoint_Scene", "TutorialZone");
        float posX = PlayerPrefs.GetFloat("Checkpoint_X", 0f);
        float posY = PlayerPrefs.GetFloat("Checkpoint_Y", -2.1f);
        lastCheckpoint = new Checkpoint(respawn, scene, new Vector2(posX, posY));

        // Load Travel Zone
        bool travel = PlayerPrefs.GetInt("TravelZone_Travel", 0) == 1;
        string newScene = PlayerPrefs.GetString("TravelZone_Scene", "TutorialZone");
        float newX = PlayerPrefs.GetFloat("TravelZone_X", 0f);
        float newY = PlayerPrefs.GetFloat("TravelZone_Y", -2.1f);
        float newDirection = PlayerPrefs.GetFloat("TravelZone_Direction", 1f);
        newTravelZone = new TravelZone(travel, newScene, new Vector2(newX, newY), newDirection);
    }

    public void Reset()
    {
        currentElement = PlayerBehaviour.Element.None;
        health = heartsCount;
        aura = 0f;
        SaveState();
    }
}
