using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public static GUIManager singleton;
    public bool mouseOverUI = false;
    public List<SelectableBehaviour> selected = new List<SelectableBehaviour>();

    public GameObject[] buttons;
    public GameObject nameText;
    public GameObject descText;
    public GameObject progressBar;
    private Text nameTextText;
    private Text descTextText;
    private Slider progressBarSlider;
    private Button[] buttonsButtons;

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
        nameTextText = nameText.GetComponent<Text>();
        descTextText = descText.GetComponent<Text>();
        progressBarSlider = progressBar.GetComponent<Slider>();
        buttonsButtons = new Button[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsButtons[i] = buttons[i].GetComponent<Button>();
        }
    }
    private void Update()
    {
        if (selected.Count > 0)
        {
            nameTextText.text = selected[0].selectableName;
            descTextText.text = selected[0].selectableDesc;
            progressBarSlider.value = selected[0].GetProgress();
        }
    }
    public void Select(SelectableBehaviour s)
    {
        s.OnSelect();
        selected.Add(s);
    }
    public void DeselectAll()
    {
        foreach (SelectableBehaviour s in selected)
        {
            s.OnDeSelect();
        }
        selected.Clear();
    }
    public void DoButtonAction(int i)
    {
        if (selected.Count == 0) return;

        SelectableBehaviour s = selected[0];
        if (s.playerId != PlayerManager.singleton.playerId) return; // can't command things you dont own.

        UIAction[] actions = s.GetActions();
        if (actions == null || actions.Length < i+1 || actions[i] == null) return;
        LockStepManager.singleton.AddPendingAction(actions[i].action);
    }
    public void UpdateButtons()
    {
        if (selected.Count > 0)
        {
            // If don't own selected, don't display buttons.
            if (selected[0].playerId != PlayerManager.singleton.playerId)
            {
                foreach (GameObject b in buttons)
                {
                    b.GetComponent<Image>().sprite = null;
                }
                return;
            }
            // Update button image to reflect the actions in selected.
            UIAction[] actions = selected[0].GetActions();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i < actions.Length)
                {
                    buttons[i].GetComponent<Image>().sprite = actions[i].labelImg;
                }
                else
                {
                    buttons[i].GetComponent<Image>().sprite = null;
                }
            }
        }
        else
        {
            // If nothing selected, don't display buttons.
            foreach (GameObject b in buttons)
            {
                b.GetComponent<Image>().sprite = null;
            }
        }
    }
}
