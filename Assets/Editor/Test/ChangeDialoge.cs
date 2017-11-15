using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChangeDialoge : ScriptableWizard {

    public string name = "hao name";


    [MenuItem("GameObject/Change Name")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ChangeDialoge>("统一修改", "Change");
    }

    private void OnWizardCreate()
    {
        
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject o in objs) {
            Undo.RecordObject(o, "Change Name");
            o.name = this.name;
        }

        
    }



    private void OnSelectionChange()
    {
        ShowNotification(new GUIContent("选择了物体: " + Selection.gameObjects.Length + " 个.", "Some tips."));
    }

    private void ShowProgress() {
        EditorUtility.DisplayCancelableProgressBar("progress", "info", 0.1f);
        EditorUtility.ClearProgressBar();
    }
}
