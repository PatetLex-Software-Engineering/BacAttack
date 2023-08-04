using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    [Header("Components")]
    public GameObject interactionOverlay;
    public TextMeshProUGUI keyLabel;
    public TextMeshProUGUI actionLabel;

    public LayerMask layer;
    public CinemachineVirtualCameraBase cinemachineCamera;


    [Header("Values")]
    public float reach;
    public float maxAngle;

    private Hoistable hoist;

    void Start() {
        Statistics statistics = gameObject.GetComponent<Statistics>();
        ProximitySpawn spawn = gameObject.GetComponent<ProximitySpawn>();

        Save save = Save.Load();
        if (save != null) {
            this.gameObject.transform.position = save.player.position;
            statistics.Instance(save.player.regenerables[0], save.player.regenerables[1], save.player.statistics[0]);
        
            foreach (Save.Entity entity in save.entities) {
                foreach (ProximitySpawn.Spawn s in spawn.spawns) {
                    if (s.entity.gameObject.name == entity.name) {
                        spawn.Instance(s.entity, entity.position, entity.statistics[0].value, entity.statistics[1].value, entity.statistics[2].value);
                        break;
                    }
                }
            }
        }
    }

    void Update() {
        Collider[] collidingObjects = Physics.OverlapSphere(this.transform.position, reach, layer);
        float closestAngle = float.MaxValue;
        GameObject closestObject = null;
        for (int i = 0; i < collidingObjects.Length; i++) {
            Collider collider = collidingObjects[i];
            Interactable interactable = collider.gameObject.GetComponent<Interactable>();
            if (interactable != null && interactable.ShouldInteract(this)) {
                float angle = Vector3.Angle(cinemachineCamera.State.CorrectedOrientation * Vector3.forward, collider.gameObject.transform.position - cinemachineCamera.transform.position);
                if (Mathf.Abs(angle) < maxAngle) {
                    float distance = Vector3.Distance(this.transform.position, collider.gameObject.transform.position);
                    if (angle < closestAngle) {
                        closestAngle = angle;
                        closestObject = collider.gameObject;
                    }
                }
            }
        }
        if (closestObject != null) {
            Interactable interactable = closestObject.GetComponent<Interactable>();
            interactionOverlay.transform.position = Camera.main.WorldToScreenPoint(new Vector3(closestObject.transform.position.x, closestObject.transform.position.y + closestObject.transform.localScale.y + 0.2F, closestObject.transform.position.z));
            keyLabel.text = interactable.Key(this).ToString().ToUpper();
            actionLabel.text = interactable.Action(this);
            interactionOverlay.SetActive(true);
            if (Input.GetKey(interactable.Key(this))) {
                interactable.Interact(this);
            }
        } else {
            interactionOverlay.SetActive(false);
        }
    }

    void FixedUpdate() {
        if (Input.GetKey(KeyCode.Tab)) {
            GUI gui = gameObject.GetComponent<GUI>();
            if (gui.current == gui.menu) {
                gui.Close();
            } else if (gui.current == null) {
                gui.SetMenu(gui.menu);
            }
        }

        if (hoist != null) {
            Statistics statistics = GetComponent<Statistics>();
            Spawnable spawnable = hoist.gameObject.GetComponent<Spawnable>();
            statistics.Speed().AddModifier(new Statistics.Modifier(hoist.speedRatio * spawnable.Speed(), Statistics.Modifier.Operator.SET));

            AI ai = hoist.gameObject.GetComponent<AI>();
            Rigidbody rigidbody = hoist.gameObject.GetComponent<Rigidbody>();
            Collider collider = hoist.gameObject.GetComponent<Collider>();
            ai.enabled = false;
            collider.enabled = false;
            rigidbody.velocity = GetComponent<Rigidbody>().velocity;
            hoist.gameObject.transform.position = transform.position - new Vector3(0, 0.1F, 0);

            if (Input.GetKey(KeyCode.LeftShift)) {
                UnHoist();
            }
            if (statistics.ATP().value == 0) {
                UnHoist();
            }
        }
    }

    public void Damage(float damage) {
        Statistics statistics = gameObject.GetComponent<Statistics>();
        float preHealth = statistics.Health().value - damage;
        statistics.Health().value -= damage;
        if (preHealth <= 0) {
            GUI gui = gameObject.GetComponent<GUI>();
            gui.SetMenu(gui.death);
        }
    }

    public void Evolve() {
        Serialize();
        SceneManager.LoadSceneAsync("Character");
    }

    public void Serialize() {
        Statistics statistics = gameObject.GetComponent<Statistics>();
        ProximitySpawn spawn = gameObject.GetComponent<ProximitySpawn>();
        ChunkLoader chunkLoader = gameObject.GetComponent<ChunkLoader>();

        Save save = new Save();

        save.seed = chunkLoader.terrain.seed;

        Save.Player player = new Save.Player();
        player.position = this.gameObject.transform.position;
        List<Statistics.Statistic> stats = new List<Statistics.Statistic>();
        stats.Add(statistics.Speed());
        List<Statistics.Regenerable> regenerables = new List<Statistics.Regenerable>();
        regenerables.Add(statistics.Health());
        regenerables.Add(statistics.ATP());
        player.statistics = stats;
        player.regenerables = regenerables;
        save.player = player;

        List<Save.Entity> entities = new List<Save.Entity>();
        foreach (Spawnable spawnable in spawn.Track()) {
            if (spawnable != null) {
                Save.Entity entity = new Save.Entity();
                entity.name = spawnable.gameObject.name;
                entity.position = spawnable.gameObject.transform.position;
                List<Statistics.Statistic> s = new List<Statistics.Statistic>();
                s.Add(new Statistics.Statistic(spawnable.Scale()));
                s.Add(new Statistics.Statistic(spawnable.Speed()));
                s.Add(new Statistics.Statistic(spawnable.ATP()));
                entity.statistics = s;
                entities.Add(entity);
            }
        }
        save.entities = entities;

        List<Save.Wave> tiles = new List<Save.Wave>();
        Dictionary<Vector2Int, Tile> sample = Terrain.WaveFunction.Sample(chunkLoader.terrain);
        foreach (Vector2Int coord in sample.Keys) {
            Tile tile = sample[coord];
            Save.Wave wave = new Save.Wave();
            wave.coord = coord;
            wave.name = tile.name;
            tiles.Add(wave);
        }
        save.tiles = tiles;

        save.Serialize();
    }

    public void Respawn() {
        Statistics statistics = gameObject.GetComponent<Statistics>();
        GUI gui = gameObject.GetComponent<GUI>();

        UnHoist();
        statistics.Health().value = statistics.Health().maxValue;
        statistics.ATP().value = statistics.ATP().maxValue;
        gameObject.transform.position = new Vector3(12, 12, 12);

        gui.Close();
        gui.SetMenu(gui.loading);
    }

    public void Leave() {
        SceneManager.LoadSceneAsync("Menu");
    }

    public Hoistable Hoist() {
        return hoist;
    }

    public void SetHoist(Hoistable hoist) {
        UnHoist();
        this.hoist = hoist;
        GetComponent<Movement>().SetMovementController(Movement.MovementController.VECTOR);
        cinemachineCamera.gameObject.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 60;
    }

    public void UnHoist() {
        if (this.hoist != null) {
            cinemachineCamera.gameObject.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 50;
            GetComponent<Movement>().SetMovementController(Movement.MovementController.CLASSIC);
            AI ai = hoist.gameObject.GetComponent<AI>();
            Collider collider = hoist.gameObject.GetComponent<Collider>();
            ai.enabled = true;
            collider.enabled = true;
            hoist = null;
        }   
    }
}
