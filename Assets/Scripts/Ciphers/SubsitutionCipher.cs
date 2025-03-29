using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SubstitutionPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [Tooltip("List of multiple secret phrases to solve in sequence.")]
    [SerializeField]
    private string[] secretPhrases = {
        "VENI VIDI VICI",
        "EUREKA",
        "CARPE DIEM"
    };

    // Letters that should be automatically solved in the grid from the start
    [SerializeField] private char[] preRevealedPlainLetters = { 'H', 'E', 'L' };

    [Header("UI References")]
    [SerializeField] private TMP_Text encryptedText;        // Displays the scrambled text of the CURRENT phrase
    [SerializeField] private TMP_Text partialDecryptedText; // Displays partial decryption as the player guesses
    [SerializeField] private TMP_Text feedbackText;         // "Correct!" / "Keep trying!" messages

    // 26 input fields for letters A¨CZ. Index 0 = scrambled 'A', 1 = scrambled 'B', ...
    [SerializeField] private TMP_InputField[] letterMappingInputs = new TMP_InputField[26];

    // A random cipher mapping plain->scrambled
    private Dictionary<char, char> encryptMap = new Dictionary<char, char>();
    // The inverse mapping (scrambled->plain), so we can easily check if a guess is correct
    private Dictionary<char, char> decryptMap = new Dictionary<char, char>();

    // The player's guess map for scrambled->plain.
    // Key = scrambled letter, Value = guessed plain letter (or '_' if unknown).
    private Dictionary<char, char> playerGuessMap = new Dictionary<char, char>();

    // Which phrase we¡¯re currently on
    private int currentPhraseIndex = 0;

    // The scrambled text of the current phrase
    private string currentScrambledPhrase = "";

    void Start()
    {
        InitializeMultiPuzzle();
    }

    private void InitializeMultiPuzzle()
    {
        // Generate a single random cipher (plain->scrambled)
        encryptMap = GenerateSubstitutionMapping();

        // Build the inverse mapping (scrambled->plain) for checking correctness
        decryptMap.Clear();
        foreach (var kvp in encryptMap)
        {
            // kvp.Key = plain letter, kvp.Value = scrambled letter
            decryptMap[kvp.Value] = kvp.Key;
        }

        // Clear guess map: scrambled letters (A¨CZ) => '_'
        playerGuessMap.Clear();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            playerGuessMap[c] = '-';
        }

        // Set up input fields
        for (int i = 0; i < letterMappingInputs.Length; i++)
        {
            if (letterMappingInputs[i] != null)
            {
                // Clear old text
                letterMappingInputs[i].text = "";

                // Reset color to default (white, gray, etc.)
                // Adjust to your own desired default color
                if (letterMappingInputs[i].GetComponent<Image>() != null)
                {
                    letterMappingInputs[i].GetComponent<Image>().color = Color.white;
                }

                // Remove any old listeners
                letterMappingInputs[i].onValueChanged.RemoveAllListeners();

                // Add a new listener
                int index = i;
                letterMappingInputs[i].onValueChanged.AddListener((value) => OnMappingChanged(index, value));
            }
        }

        // Reveal some letters from the start if desired
        RevealPreFilledLetters();

        // Reset puzzle to the first phrase
        currentPhraseIndex = 0;
        LoadCurrentPhrase(); // load phrase #1
    }

    /// <summary>
    /// Encrypts & displays the current phrase with partial reveals from playerGuessMap.
    /// </summary>
    private void LoadCurrentPhrase()
    {
        // If we've run out of phrases, puzzle is done
        if (currentPhraseIndex >= secretPhrases.Length)
        {
            if (feedbackText != null)
                feedbackText.text = "All phrases solved! Puzzle complete.";
            return;
        }

        // Grab the actual phrase
        string phraseToEncrypt = secretPhrases[currentPhraseIndex];

        // Encrypt with our cipher
        currentScrambledPhrase = EncryptWithSubstitution(phraseToEncrypt, encryptMap);

        // Show it
        if (encryptedText != null)
            encryptedText.text = currentScrambledPhrase;

        // Update partial decryption text
        UpdatePartialDecryption();

        if (feedbackText != null)
            feedbackText.text = $"Phrase {currentPhraseIndex + 1} of {secretPhrases.Length}";
    }

    /// <summary>
    /// If the user hits a "Check Solution" button, we see if
    /// their partial decryption matches the current phrase exactly.
    /// If correct, we move to the next phrase; if not, show feedback.
    /// </summary>
    public void OnCheckSolution()
    {
        if (currentPhraseIndex >= secretPhrases.Length) return;

        // Build the player's full decryption of the current scrambled phrase
        string userDecrypted = DecryptWithPlayerGuess(currentScrambledPhrase);
        // The correct plain text
        string actualPhrase = secretPhrases[currentPhraseIndex];

        // Compare ignoring case
        bool matches = userDecrypted.Equals(actualPhrase, System.StringComparison.OrdinalIgnoreCase);
        if (matches)
        {
            // Reveal all letter mappings from this phrase so the user¡¯s cipher knowledge grows
            RevealAllLettersFromPhrase(actualPhrase);

            if (feedbackText != null)
                feedbackText.text = "Correct! Moving to next phrase...";

            // Move to the next phrase
            currentPhraseIndex++;
            LoadCurrentPhrase();
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Not quite correct. Keep trying!";
        }
    }


    private void OnMappingChanged(int index, string value)
    {
        char scrambledLetter = (char)('A' + index);

        // We treat input as uppercase, only first character
        value = value.Trim().ToUpper();
        char guessedChar = '_';
        if (!string.IsNullOrEmpty(value))
        {
            char c = value[0];
            if (c >= 'A' && c <= 'Z')
            {
                guessedChar = c;
            }
        }

        // Update guess map
        playerGuessMap[scrambledLetter] = guessedChar;

        // Check correctness: If guessedChar == decryptMap[scrambledLetter], color green
        // If guessedChar == '_' or incorrect, color white (or your default).
        if (letterMappingInputs[index].GetComponent<Image>() != null)
        {
            if (guessedChar != '_' && decryptMap.ContainsKey(scrambledLetter))
            {
                char actualPlain = decryptMap[scrambledLetter];
                if (guessedChar == actualPlain)
                {
                    // Correct guess for that scrambled letter
                    letterMappingInputs[index].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    // Wrong guess
                    letterMappingInputs[index].GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                // Blank or unknown
                letterMappingInputs[index].GetComponent<Image>().color = Color.white;
            }
        }

        // Refresh partial decryption
        UpdatePartialDecryption();
    }

    /// <summary>
    /// Generates a random cipher: plain (A-Z) => scrambled letter
    /// </summary>
    private Dictionary<char, char> GenerateSubstitutionMapping()
    {
        List<char> letters = new List<char>();
        for (char c = 'A'; c <= 'Z'; c++)
            letters.Add(c);

        // Shuffle
        System.Random rand = new System.Random();
        for (int i = 0; i < letters.Count; i++)
        {
            int r = rand.Next(i, letters.Count);
            char temp = letters[i];
            letters[i] = letters[r];
            letters[r] = temp;
        }

        // Build mapping
        Dictionary<char, char> map = new Dictionary<char, char>();
        char current = 'A';
        foreach (char c in letters)
        {
            map[current] = c;
            current = (char)(current + 1);
        }
        return map;
    }

    /// <summary>
    /// Encrypt a string with our cipher. Non-letters stay the same.
    /// We use uppercase behind the scenes.
    /// </summary>
    private string EncryptWithSubstitution(string input, Dictionary<char, char> map)
    {
        input = input.ToUpper();
        char[] arr = input.ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            char ch = arr[i];
            if (ch >= 'A' && ch <= 'Z')
            {
                arr[i] = map[ch];
            }
        }
        return new string(arr);
    }

    /// <summary>
    /// Decrypt the given scrambled string using the player's guess map.
    /// If a letter in the guess map is still '_', we show it as '_'.
    /// </summary>
    private string DecryptWithPlayerGuess(string scrambled)
    {
        char[] arr = scrambled.ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            char sc = arr[i]; // scrambled char
            if (sc >= 'A' && sc <= 'Z')
            {
                char guess = playerGuessMap[sc];
                if (guess == '_')
                    arr[i] = '_';
                else
                    arr[i] = guess;
            }
        }
        return new string(arr);
    }

    /// <summary>
    /// Called after any input change to refresh the partial decryption
    /// of the current scrambled phrase. 
    /// </summary>
    private void UpdatePartialDecryption()
    {
        if (currentPhraseIndex < secretPhrases.Length)
        {
            // Rebuild partial text for the current phrase
            string partial = DecryptWithPlayerGuess(currentScrambledPhrase);
            if (partialDecryptedText != null)
            {
                partialDecryptedText.text = partial;
            }
        }
    }

    /// <summary>
    /// Once a phrase is correctly guessed, we reveal the letter mappings for that entire phrase
    /// so that future puzzles have more letters pre-solved.
    /// </summary>
    private void RevealAllLettersFromPhrase(string plainPhrase)
    {
        // We look at how plainPhrase was scrambled by the cipher
        string scrambled = EncryptWithSubstitution(plainPhrase, encryptMap);

        for (int i = 0; i < plainPhrase.Length; i++)
        {
            char p = char.ToUpper(plainPhrase[i]);
            char s = scrambled[i];
            if (p >= 'A' && p <= 'Z')
            {
                // If we haven't guessed that scrambled letter yet, now we know
                if (playerGuessMap[s] == '_')
                {
                    playerGuessMap[s] = p;

                    // Update the relevant InputField in the grid
                    int index = s - 'A';
                    if (index >= 0 && index < letterMappingInputs.Length)
                    {
                        letterMappingInputs[index].text = p.ToString();
                        // Also color it green since it's correct
                        if (letterMappingInputs[index].GetComponent<Image>() != null)
                        {
                            letterMappingInputs[index].GetComponent<Image>().color = Color.green;
                        }
                    }
                }
            }
        }

        // Refresh partial text
        UpdatePartialDecryption();
    }


    private void RevealPreFilledLetters()
    {
        foreach (char letter in preRevealedPlainLetters)
        {
            char upper = char.ToUpper(letter);
            if (upper >= 'A' && upper <= 'Z')
            {
                if (encryptMap.ContainsKey(upper))
                {
                    // e.g. if encryptMap[upper] = scrambledLetter
                    char scrambledLetter = encryptMap[upper];

                    // Fill it in
                    playerGuessMap[scrambledLetter] = upper;

                    // Update the grid UI
                    int index = scrambledLetter - 'A';
                    if (index >= 0 && index < letterMappingInputs.Length)
                    {
                        letterMappingInputs[index].text = upper.ToString();

                        // Color the tile green, since it's correct
                        if (letterMappingInputs[index].GetComponent<Image>() != null)
                        {
                            letterMappingInputs[index].GetComponent<Image>().color = Color.green;
                        }
                    }
                }
            }
        }
    }

    public void OnCheckCipherComplete()
    {
        if (IsCipherFullyKnown())
        {
            if (feedbackText != null)
                feedbackText.text = "All letters discovered! Cipher complete.";
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Some letters remain unknown.";
        }
    }

    private bool IsCipherFullyKnown()
    {
        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (playerGuessMap[c] == '_')
                return false;
        }
        return true;
    }
}
