using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonHandler : MonoBehaviour
{
    public CharacterModel CharacterModelInfo;
    public void Initialize(CharacterModel characterModel, CharacterDetailView detailView)
    {
        UpdateCharacter(characterModel);

        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            detailView.gameObject.SetActive(true);
            detailView.UpdateView(CharacterModelInfo);
        });
    }

    internal void UpdateCharacter(CharacterModel characterModel)
    {
        CharacterModelInfo = characterModel;
    }
}
