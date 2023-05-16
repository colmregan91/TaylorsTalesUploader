//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class InteractionCreator : EditorWindow
//{
//    public interactionType InteractionType;
//    public int SelectedInteractionIndex = 0;
//    public enum interactionType
//    {
//        none,
//        rigidForce
//    }

//    private string[] typesToWords;
//    private GameObject InteractionCanvas;

//    [Header("Tags")]
//    public const string INTERACTIONCAVAS = "InteractionCanvas";

//    [MenuItem("EditorWindows/InteractionCreator")]
//    public static void ShowUploadWindow()
//    {
//        GetWindow<InteractionCreator>("InteractionCreator");
//    }

//    private void OnEnable()
//    {
//        typesToWords = Enum.GetNames(typeof(interactionType));

//        InteractionCanvas = GameObject.FindWithTag(INTERACTIONCAVAS);

//        Selection.selectionChanged += Repaint;

//}

//    private void OnDisable()
//    {
//        Selection.selectionChanged -= Repaint;
//    }
//    private void OnGUI()
//    {
//        // select Object to Configure
//        GUILayout.Label("Select Interactable Prefab from folder ...");
//        var selectedObject = Selection.activeGameObject;
//        var go = (GameObject)EditorGUILayout.ObjectField("Confirurable Object", selectedObject == null ? null : selectedObject, typeof(Interact), false);
//        GUILayout.Label("Select Interaction Behavior");
//        SelectedInteractionIndex = EditorGUILayout.Popup(SelectedInteractionIndex, typesToWords);

//        EditorGUILayout.Space();
//        Repaint();
      

//        if (GUILayout.Button("Create Interaction"))
//        {
//            if (selectedObject == null)
//            {
//                Debug.LogError("NO OBJECT SELECTED");
//                return;
//            }
//            interactionType interactionType = (interactionType)SelectedInteractionIndex;
//            CreateInteractable(selectedObject, interactionType);
//        }

//        //if (selectedObject.GetComponent<Interact>() == null)
//        //{
//        //    selectedObject = null;
//        //    Debug.LogError("Must be Object that has Interact script attached");
//        //}

//        //var go = (GameObject)EditorGUILayout.ObjectField("Confirurable Object", selectedObject == null ? null : selectedObject, typeof(Interact), true);


//        // GUILayout.Label("Select Interaction Type");
//        //var selectedIndex = EditorGUILayout.Popup(0, typesToWords);
//        // EditorGUILayout.Space();
//        // var name = "";
//        // EditorGUILayout.TextField("Name :", name);
//        // EditorGUILayout.Space();

//        // if (GUILayout.Button("Instantiate Gameobject") && !string.IsNullOrEmpty(name))
//        // {
//        //     GameObject newInteraction = new GameObject(name);
//        //     newInteraction.transform.SetParent(InteractionCanvas.transform);

//        //     switch (InteractionType)
//        //     {
//        //         case interactionType.rigidForce:
//        //             newInteraction.AddComponent<Rigidbody2D>();
//        //            var rforce = newInteraction.AddComponent<RigidForce>();

//        //             break;
//        //     }
//        // }


//        // select touch indicator 

//        // select components

//        // select interacton type


//        //switch (InteractionType)
//        //{
//        //    case interactionType.rigidForce:
//        //        gameObject.AddComponent<RigidForce>();

//        //        break;
//        //}
//    }

//    public void CreateInteractable(GameObject obj, interactionType selectedInteraction)
//    {
//        GameObject touchIndicator = GameObject.Instantiate(Resources.Load("TouchIndicator/TouchHereIndicator") as GameObject);
//        touchIndicator.name ="Touch Indicator";
//        touchIndicator.transform.SetParent(InteractionCanvas.transform, false);
//        touchIndicator.transform.localPosition = Vector3.zero;
//        GameObject childPrefab = GameObject.Instantiate(obj);
//        childPrefab.transform.SetParent(touchIndicator.transform, false);
//        childPrefab.transform.localPosition = Vector3.zero;

//        switch (selectedInteraction)
//        {
//            case interactionType.rigidForce:
//                childPrefab.AddComponent<RigidForce>();
//                break;
//        }
//    }

//}

