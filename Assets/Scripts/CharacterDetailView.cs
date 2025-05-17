using TMPro;
using UnityEngine;

public class CharacterDetailView : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] TextMeshProUGUI id;
    [SerializeField] TextMeshProUGUI animationName;
    [SerializeField] TextMeshProUGUI position;
    [SerializeField] TextMeshProUGUI rotation;

    [Header("Form Data")]
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] TMP_InputField xPosInput;
    [SerializeField] TMP_InputField yPosInput;
    [SerializeField] TMP_InputField zPosInput;
    [SerializeField] TMP_InputField xRotInput;
    [SerializeField] TMP_InputField yRotInput;
    [SerializeField] TMP_InputField zRotInput;
    public void UpdateView(CharacterModel characterModel)
    {
        id.text = characterModel.ID;
        animationName.text = characterModel.NewAnimation;
        position.text = characterModel.Position.ToString();
        rotation.text = characterModel.Rotation.ToString();

        dropdown.value = dropdown.options.FindIndex(x => x.text.Equals(characterModel.NewAnimation));

        xPosInput.text = characterModel.Position.X.ToString();
        yPosInput.text = characterModel.Position.Y.ToString();
        zPosInput.text = characterModel.Position.Z.ToString();

        xRotInput.text = characterModel.Rotation.X.ToString();
        yRotInput.text = characterModel.Rotation.Y.ToString();
        zRotInput.text = characterModel.Rotation.Z.ToString();
    }

    public void Submit()
    {
        if (!IsFormValid())
            return;

        CharacterModel characterModel = new CharacterModel()
        {
            ID = id.text,
            NewAnimation = dropdown.options[dropdown.value].text,
            Position = new Position()
            {
                X = float.Parse(xPosInput.text),
                Y = float.Parse(yPosInput.text),
                Z = float.Parse(zPosInput.text)
            },
            Rotation = new Rotation()
            {
                X = float.Parse(xRotInput.text),
                Y = float.Parse(yRotInput.text),
                Z = float.Parse(zRotInput.text)
            }
        };

        Publisher.Publish(new CharacterUpdateMessage(characterModel));
    }

    private bool IsFormValid()
    {
        return dropdown.value >= 0 
            && !string.IsNullOrEmpty(dropdown.options[dropdown.value].text)
            && !string.IsNullOrEmpty(xPosInput.text)
            && !string.IsNullOrEmpty(yPosInput.text)
            && !string.IsNullOrEmpty(zPosInput.text)
            && !string.IsNullOrEmpty(xRotInput.text)
            && !string.IsNullOrEmpty(yRotInput.text)
            && !string.IsNullOrEmpty(zRotInput.text);
    }
}