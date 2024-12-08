using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SetUpDropdown
{
    public static void SetUp(TMP_Dropdown SceneSelector)
    {
        // Set up dropdown
        SceneSelector.options.Clear();
        string currentSceneName = SceneManager.GetActiveScene().name;
        int myIndex = 0;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == currentSceneName) myIndex = i;
            SceneSelector.options.Add(new TMP_Dropdown.OptionData(sceneName));
        }
        SceneSelector.onValueChanged.AddListener(index =>
        {
            SceneManager.LoadScene(SceneSelector.options[index].text);
        });
        SceneSelector.SetValueWithoutNotify(myIndex);
        SceneSelector.captionText.text = SceneSelector.options[myIndex].text; // Update displayed text
    }
}
