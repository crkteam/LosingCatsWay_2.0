using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class TutorialDirector : MvcBehaviour
{
    public List<TutorialActor> actors = new List<TutorialActor>();
    public UnityEvent OnDirectorStart;
    public UnityEvent OnDirectorEnd;
    public bool hasNextDirector;

    private int stepIndex;

    public void Init()
    {
        actors.Sort((x, y) => x.order.CompareTo(y.order));
        for (int i = 0; i < actors.Count; i++)
            actors[i].gameObject.SetActive(false);
        stepIndex = -1;
        
        OnDirectorStart?.Invoke();
    }

    private void Action(int index)
    {
        if (index < 0)
            return;
        if (index >= actors.Count)
        {
            App.system.tutorial.Close();
            
            if (hasNextDirector)
            {
                App.system.tutorial.SetCameraDrag(true);
                App.system.tutorial.SetCameraPinch(true);
                App.system.tutorial.SetBlackBg(false);
                return;
            }
            
            OnDirectorEnd?.Invoke();
            App.system.tutorial.isTutorial = false;
            return;
        }
        
        stepIndex = index;
        actors[stepIndex].gameObject.SetActive(true);
        actors[stepIndex].Enter();
    }

    public void NextAction()
    {
        Action(stepIndex + 1);
    }

    [Button]
    private void GetAllChildActors()
    {
        actors.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            TutorialActor actor = transform.GetChild(i).GetComponent<TutorialActor>();
            if (actor != null)
                actors.Add(actor);
        }
    }
}
