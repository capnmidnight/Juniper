using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LowPolyAnimalPack
{
  [CustomEditor(typeof(AnimalManager))]
  public class AnimalManagerEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      AnimalManager animalManager = (AnimalManager)target;

      if (!Application.isPlaying)
      {
        base.OnInspectorGUI();
        return;
      }

      GUILayout.Space(10);

      animalManager.PeaceTime =  EditorGUILayout.Toggle("Peace Time", animalManager.PeaceTime);

      GUILayout.Space(5);

      if (GUILayout.Button("Nuke Animals"))
      {
        animalManager.Nuke();
      }
    }
  }
}