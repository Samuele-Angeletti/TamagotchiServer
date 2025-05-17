internal class CharacterModelReceivedMessage : IPublisherMessage
{
    public CharacterModelReceivedMessage(CharacterModel characterModel)
    {
        CharacterModel = characterModel;
    }

    public CharacterModel CharacterModel { get; }
}