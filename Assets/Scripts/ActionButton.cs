using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    FSMStateType buttonAction;
    public List<BasicAI> hostList = new List<BasicAI>();
    private void Awake() {
        
    }

    

    public void SetAction(FSMStateType actionType) {
        hostList = FindObjectOfType<PlayerSelection>().getSelected();
        buttonAction = actionType;
        transform.GetChild(0).GetComponent<Text>().text = buttonAction.ToString();
    }

    public void ClickButton() {
        
            PlayerSelection selection = FindObjectOfType<PlayerSelection>();
            selection.startActionLoop(buttonAction);
       
    }
}
