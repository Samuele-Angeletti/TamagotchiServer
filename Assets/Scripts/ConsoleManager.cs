using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsoleManager : MonoBehaviour, ISubscriber
{
    [SerializeField] Transform consoleGrid;
    [SerializeField] CharacterButtonHandler characterButtonHandlerPrefab;
    [SerializeField] CharacterDetailView characterDetailView;

    List<CharacterButtonHandler> _characterButtonHandlers;
    private void Awake()
    {
        _characterButtonHandlers = new List<CharacterButtonHandler>();
    }
    private void Start()
    {
        Publisher.Subscribe(this, typeof(CharacterModelReceivedMessage));
    }
    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(CharacterModelReceivedMessage));
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is CharacterModelReceivedMessage characterModelReceivedMessage)
        {
            var characterButton = _characterButtonHandlers
                .FirstOrDefault(btn => 
                btn.CharacterModelInfo.ID == characterModelReceivedMessage.CharacterModel.ID);

            if (characterButton != null)
            {
                // aggiorna il pulsante
                characterButton.UpdateCharacter(characterModelReceivedMessage.CharacterModel);
            }
            else
            {
                // spawn nuovo pulsante
                var newCharacterButton = Instantiate(characterButtonHandlerPrefab, consoleGrid);
                newCharacterButton.Initialize(characterModelReceivedMessage.CharacterModel, characterDetailView);
            }
        }
    }

    private void OnDisable()
    {
        OnDisableSubscriber();
    }
}

