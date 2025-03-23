using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class SubsitutionCipher : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private string secretMessage = "EXAMPLE MESSAGE";

    private Dictionary<char, char> encryptMap = new Dictionary<char, char>();

    [Header("UI References")]
    [SerializeField] private TMP_Text encryptedText;
    [SerializeField] private TMP_Text partialDecryptedText;
    [SerializeField] private TMP_Text feedbackText;

    [SerializeField] private TMP_InputField[] letterMappingInputs = new TMP_InputField[26];


    private Dictionary<char, char> playerGuessMap = new Dictionary<char, char>();

    private string scrambledMessage = "";

    void Start()
    {
        // Initialize player's guess map with blanks (or space/underscore).
        // Also generate a random substitution puzzle, which sets encryptMap.
        InitializePuzzle();

        // Update all UI fields accordingly.
        DisplayEncryptedMessage();
        UpdatePartialDecryption(); // Show empty or partial at puzzle start
    }

    void InitializePuzzle()
    {
        // Generate random mapping
        (encryptMap, _) = GenerateSubstitutionMapping();

        // Encrypt the secretMessage using the random mapping
        scrambledMessage = EncryptWithSubstitution(secretMessage, encryptMap);

        // Initialize player's guess map to something like '_' (unknown)
        playerGuessMap.Clear();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            playerGuessMap[c] = '_'; // Or ' ' (space)
        }

        for (int i = 0; i < letterMappingInputs.Length; i++)
        {
            if (letterMappingInputs[i] != null)
            {
                letterMappingInputs[i].text = "";
                // Add listener to watch changes
                int index = i;
                letterMappingInputs[i].onValueChanged.AddListener((value) => OnMappingChanged(index, value));
            }
        }
    }

    void DisplayEncryptedMessage()
    {
        // Show the scrambled version in the UI
        if (encryptedText != null)
        {
            encryptedText.text = scrambledMessage;
        }
    }

    void UpdatePartialDecryption()
    {
        // Build a string by decrypting each char with player's guess
        char[] buffer = scrambledMessage.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char encryptedChar = buffer[i];
            if (encryptedChar >= 'A' && encryptedChar <= 'Z')
            {
                // The letter that is used for encryption
                // We want to see how the player thinks we *should* map it
                char guess = playerGuessMap[encryptedChar];
                if (guess == '_')
                {
                    // If user hasn't provided a guess yet, show underscore or blank
                    buffer[i] = '_';
                }
                else
                {
                    buffer[i] = guess;
                }
            }
        }

        string partial = new string(buffer);
        if (partialDecryptedText != null)
        {
            partialDecryptedText.text = partial;
        }
    }

    /// <summary>
    /// This is the callback when one of the letter mappings changes 
    /// (e.g. user typed in an InputField).
    /// index = 0 -> 'A', 1 -> 'B', etc.
    /// value is the text the user typed in the field.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    void OnMappingChanged(int index, string value)
    {
        // Make sure it's uppercase
        value = value.ToUpper();

        // Keep it to just 1 letter (A-Z). 
        // If the user typed multiple or invalid characters, we handle that gracefully.
        char guessedChar = '_';
        if (!string.IsNullOrEmpty(value))
        {
            char c = value[0];
            if (c >= 'A' && c <= 'Z')
            {
                guessedChar = c;
            }
        }

        // The encrypted letter is 'A' + index
        char encryptedLetter = (char)('A' + index);
        playerGuessMap[encryptedLetter] = guessedChar;

        // Update the partial decrypted text
        UpdatePartialDecryption();
    }

    public void CheckSolution()
    {
        // Decrypt the scrambledMessage with the player's guessed map
        string userDecrypted = DecryptWithPlayerGuess(scrambledMessage, playerGuessMap);

        // Compare with the known secret message. 
        // For fairness, we can remove spaces or compare ignoring case if desired.
        if (userDecrypted == secretMessage.ToUpper())
        {
            feedbackText.text = "Puzzle Solved!";
        }
        else
        {
            feedbackText.text = "Not quite correct. Keep trying!";
        }
    }

    private string DecryptWithPlayerGuess(string encrypted, Dictionary<char, char> guessMap)
    {
        char[] buffer = encrypted.ToUpper().ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];
            if (c >= 'A' && c <= 'Z')
            {
                char guess = guessMap[c];
                buffer[i] = (guess == '_') ? '_' : guess;
            }
        }
        return new string(buffer);
    }

    private (Dictionary<char, char>, Dictionary<char, char>) GenerateSubstitutionMapping()
    {
        // Create an alphabet list
        List<char> letters = new List<char>();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            letters.Add(c);
        }

        // Shuffle letters to create random mapping
        System.Random rand = new System.Random();
        for (int i = 0; i < letters.Count; i++)
        {
            int r = rand.Next(i, letters.Count);
            char temp = letters[i];
            letters[i] = letters[r];
            letters[r] = temp;
        }

        // Build encryption/decryption maps
        Dictionary<char, char> enc = new Dictionary<char, char>();
        Dictionary<char, char> dec = new Dictionary<char, char>();
        char letter = 'A';
        foreach (char c in letters)
        {
            enc[letter] = c;
            dec[c] = letter;
            letter++;
        }
        return (enc, dec);
    }

    private string EncryptWithSubstitution(string input, Dictionary<char, char> map)
    {
        input = input.ToUpper();
        char[] buffer = input.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char letter = buffer[i];
            if (letter >= 'A' && letter <= 'Z')
            {
                buffer[i] = map[letter];
            }
        }
        return new string(buffer);
    }
}
