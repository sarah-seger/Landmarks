/*


    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class CDtrial : ExperimentTask
{
    //[Header("Task-specific Properties")]

    private bool changePresent; // is this a change trial?
    private bool responded; // has the participant provided a valid response yet?
    private KeyCode correctAnswer; // What button is the correct one?
    private KeyCode response; // which button button did they press?
    private GameObject changedObject; // container for which object was changed
    private Color originalColor; // container for reverting the changed object back
    private Color changeColor;
    private bool feedbackProvided;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // WRITE TASK STARTUP CODE HERE


        // run some basic task setup
        hud.showEverything();
        hud.setMessage("Press [1] if any object changed. Otherwise press [2]");
        manager.player.transform.position = Vector3.zero;


        // Odd trials are change
        if (parentTask.repeatCount % 2 != 0)
        {
            changePresent = true;
            correctAnswer = KeyCode.Alpha1;
            Debug.Log("There is a change!");
        }

        else
        {
            correctAnswer = KeyCode.Alpha2;
            changePresent = false;
            Debug.Log("There is no change!");
        }


        // Pick one target object at random
        var iChange = Random.Range(0, manager.targetObjects.transform.childCount);
        for (int i = 0; i < manager.targetObjects.transform.childCount; i++)
        {
            
            // change the color of the object (if necessary)
            if (i == iChange)
            {
                changedObject = manager.targetObjects.transform.GetChild(i).gameObject;
                originalColor = changedObject.GetComponent<Renderer>().material.color;
                if (changePresent)
                {
                    // record which object is getting changed and it's original color
                    changeColor = Random.ColorHSV();

                    // actually change the color
                    changedObject.GetComponentInChildren<Renderer>().material.color = changeColor;
                }
                else
                {
                    changeColor = originalColor;
                }
            }
        }

    }

    public override bool updateTask()
    {
        // WRITE TASK UPDATE CODE HERE

        hud.ForceShowMessage(); // always show everything


        // Handle taking the response
        if (!responded)
        {
            // hit return to continue on to the next trial
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                responded = true;
                response = KeyCode.Alpha1;
                Debug.Log("You said there was a change");
                StartCoroutine(CheckAnswer());

            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                responded = true;
                response = KeyCode.Alpha2;
                Debug.Log("You said there was NO change");
                StartCoroutine(CheckAnswer());
            }
        }


        // Handle when to end the trial
        if (feedbackProvided)
        {
            return true;
        }
        else return false;
        
    }


    public override void endTask()
    {
        TASK_END();

        // LEAVE BLANK
    }


    public override void TASK_END()
    {
        base.endTask();

        // WRITE TASK EXIT CODE HERE


        // Log Data
        // More concise LM_TrialLog logging
        if (trialLog.active)
        {
            trialLog.AddData(transform.name + "changePresent", changePresent.ToString());
            trialLog.AddData(transform.name + "changedObject", changedObject.name);
            trialLog.AddData(transform.name + "originalColor", originalColor.ToString());
            trialLog.AddData(transform.name + "changedColor", changeColor.ToString());
            trialLog.AddData(transform.name + "participantResponse", response.ToString());
            trialLog.AddData(transform.name + "correctAnswer", correctAnswer.ToString());
            trialLog.AddData(transform.name + "correct", (response == correctAnswer).ToString());
        }

        // clean up
        hud.setMessage("");
        hud.showOnlyHUD();
        if (changePresent)
        changedObject.GetComponent<Renderer>().material.color = originalColor; // change it back
        feedbackProvided = false;
        responded = false;


    }

    IEnumerator CheckAnswer()
    {
        string hudMessage;
        hud.showOnlyHUD();
        // check the answer
        if (response == correctAnswer)
        {
            hudMessage = "[DING DING DING DING] You are correct!!!!!";
        }
        else
        {
            hudMessage = "[HARSH BUZZER NOISE] You are wrong!!!!!";
        }

        hud.setMessage(hudMessage);
        hud.ForceShowMessage();

        yield return new WaitForSeconds(3);

        feedbackProvided = true;
    }

}