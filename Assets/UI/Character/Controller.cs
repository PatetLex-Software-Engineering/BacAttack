using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [Header("Components")]
    public MeshRenderer virus;
    public Material edit;

    [Header("UI")]
    public TextMeshProUGUI totalATP;

    [Header("Tools")]
    public List<Tool> tools;

    private Material original;

    private Save save;


    void Start()
    {
        this.original = virus.sharedMaterial;    
        this.save = Save.Load();
    }

    void Update()
    {
        float atp = save.player.regenerables[1].value;
        virus.sharedMaterial = Tool.current != null ? edit : original;
        
        totalATP.text = Mathf.RoundToInt(atp).ToString();

        foreach (Tool tool in tools) {
            Image image = tool.gameObject.GetComponent<Image>();
            Color color = image.color;
            if (tool.atpCost <= atp) {
                color.a = 1F;
                tool.SetLocked(false);
            } else {
                color.a = 0.4F;
                tool.SetLocked(true);
            }
            image.color = color;
        }

        virus.gameObject.transform.localScale = Vector3.one * (save.player.statistics[0].value / 5F) * 7;
    }

    public void Exit() {
        this.save.Serialize();
        SceneManager.LoadSceneAsync("Game");
    }

    public void Scale() {
        save.player.regenerables[1].value -= Tool.current.atpCost;
        save.player.statistics[0].value += 1F;
    }
}
