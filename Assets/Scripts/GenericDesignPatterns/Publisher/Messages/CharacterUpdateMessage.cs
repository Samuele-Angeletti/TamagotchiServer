public class CharacterUpdateMessage : IPublisherMessage
{
    public CharacterUpdateMessage(CharacterModel characterModel)
    {
        CharacterModel = characterModel;
    }

    public CharacterModel CharacterModel { get; }
}