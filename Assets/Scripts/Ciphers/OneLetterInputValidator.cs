using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class OneLetterInputValidator : MonoBehaviour
{
    private TMP_InputField _inputField;

    void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.characterLimit = 1; // limit to 1 character
        _inputField.onValueChanged.AddListener(ValidateLetter);
    }

    private void ValidateLetter(string input)
    {
        // If empty, no need to check
        if (string.IsNullOrEmpty(input)) return;

        // Take the first character only
        char c = input[0];

        // Force uppercase or do other checks
        if (c >= 'a' && c <= 'z')
        {
            c = char.ToUpper(c);
        }

        // If it's not A-Z, then clear the field
        if (c < 'A' || c > 'Z')
        {
            _inputField.text = "";
        }
        else
        {
            // If valid, keep only that one character
            _inputField.text = c.ToString();
        }
    }
}
