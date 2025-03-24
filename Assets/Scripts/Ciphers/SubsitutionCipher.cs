using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SubstitutionPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private string secretMessage = "HELLO UNITY";

    // Letters that should be automatically solved in the grid from the start
    [SerializeField] private char[] preRevealedPlainLetters = { 'H', 'E', 'L' };

    // Where we display the encrypted puzzle text
    [Header("UI References")]
    [SerializeField] private TMP_Text encryptedText;
    // Where we display the partial-decrypted text in real time
    [SerializeField] private TMP_Text partialDecryptedText;
    // Feedback label (e.g., ※Correct!§ or ※Try again!§)
    [SerializeField] private TMP_Text feedbackText;

    // 26 input fields for letters A每Z, each for the user＊s guess:
    // index 0 -> scrambled 'A', index 1 -> scrambled 'B', etc.
    [SerializeField] private TMP_InputField[] letterMappingInputs = new TMP_InputField[26];

    // The random encryption map: plain -> scrambled
    private Dictionary<char, char> encryptMap = new Dictionary<char, char>();
    // The user＊s guess for how scrambled letters map back to plain
    private Dictionary<char, char> playerGuessMap = new Dictionary<char, char>();

    // The scrambled version of secretMessage
    private string scrambledMessage = "";

    void Start()
    {
        InitializePuzzle();
    }

    /// <summary>
    /// Sets up the puzzle:
    /// 1) Generate random substitution cipher.
    /// 2) Encrypt the secret message.
    /// 3) Clear the user's guess map.
    /// 4) Pre-reveal certain letters in the grid.
    /// 5) Show the scrambled text and partial solution (mostly unknown).
    /// </summary>
    private void InitializePuzzle()
    {
        // Generate a random cipher
        encryptMap = GenerateSubstitutionMapping();
        // Encrypt the secret message
        scrambledMessage = EncryptWithSubstitution(secretMessage, encryptMap);

        // Initialize user＊s guess map so every scrambled letter is set to '_'
        playerGuessMap.Clear();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            playerGuessMap[c] = '_';
        }

        // Hook up each input field
        for (int i = 0; i < letterMappingInputs.Length; i++)
        {
            if (letterMappingInputs[i] != null)
            {
                // Clear old text
                letterMappingInputs[i].text = "";

                // Remove old listeners to avoid duplication
                letterMappingInputs[i].onValueChanged.RemoveAllListeners();

                // Add new listener
                int index = i;
                letterMappingInputs[i].onValueChanged.AddListener((value) => OnMappingChanged(index, value));
            }
        }

        // Now reveal whichever letters are supposed to be pre-filled
        RevealPreFilledLetters();

        // Display the scrambled message
        if (encryptedText != null)
            encryptedText.text = scrambledMessage;

        // Show partial decrypted text (most letters unknown)
        UpdatePartialDecryption();
    }

    /// <summary>
    /// Generate a random substitution mapping (A->X, B->Q, etc.)
    /// and return it as (plain->scrambled).
    /// </summary>
    private Dictionary<char, char> GenerateSubstitutionMapping()
    {
        // Create a list of letters A每Z
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

        // Build the mapping
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
    /// Encrypt a string using our substitution mapping. Non-letters remain as is.
    /// We treat input as uppercase internally.
    /// </summary>
    private string EncryptWithSubstitution(string input, Dictionary<char, char> map)
    {
        input = input.ToUpper();
        char[] arr = input.ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            char letter = arr[i];
            if (letter >= 'A' && letter <= 'Z')
            {
                arr[i] = map[letter];
            }
        }
        return new string(arr);
    }

    /// <summary>
    /// Called whenever one of the 26 InputFields is changed.
    /// 'index' is 0..25 for scrambled letter A..Z.
    /// 'value' is the text typed by the user.
    /// </summary>
    private void OnMappingChanged(int index, string value)
    {
        // scrambled letter is 'A' + index
        char scrambledLetter = (char)('A' + index);

        // We'll allow any single letter A每Z (case-insensitive).
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

        playerGuessMap[scrambledLetter] = guessedChar;
        UpdatePartialDecryption();
    }

    /// <summary>
    /// Builds a partially decrypted string using the player's guesses.
    /// If playerGuessMap[eChar] = X, we replace eChar with X in the scrambled text.
    /// If it's '_', we leave it as '_' (or we could show the scrambled letter).
    /// </summary>
    private void UpdatePartialDecryption()
    {
        char[] buffer = scrambledMessage.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char eChar = buffer[i];
            if (eChar >= 'A' && eChar <= 'Z')
            {
                char guess = playerGuessMap[eChar];
                if (guess == '_')
                {
                    // If unknown, we can show '_' or the scrambled letter.
                    // We'll show '_' so the user sees unsolved letters.
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
    /// If you have a button for the user to guess the entire phrase (like ※HELLO UNITY§),
    /// you can call this method and compare the partial guess to the real solution.
    /// This is optional 〞 you might just let them solve the mapping visually.
    /// </summary>
    public void OnCheckSolution()
    {
        // Build the player's full decryption of the scrambled message
        string userDecrypted = DecryptWithPlayerGuess(scrambledMessage);
        // Compare ignoring case
        if (userDecrypted.Equals(secretMessage, System.StringComparison.OrdinalIgnoreCase))
        {
            if (feedbackText) feedbackText.text = "Puzzle Solved!";
        }
        else
        {
            if (feedbackText) feedbackText.text = "Not quite correct. Keep trying!";
        }
    }

    /// <summary>
    /// Decrypt the scrambled message using whatever the player has put in the grid.
    /// Any unknown letters in playerGuessMap remain '_' in the final string.
    /// </summary>
    private string DecryptWithPlayerGuess(string encrypted)
    {
        char[] buffer = encrypted.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char eChar = buffer[i];
            if (eChar >= 'A' && eChar <= 'Z')
            {
                char guess = playerGuessMap[eChar];
                if (guess == '_')
                {
                    buffer[i] = '_';
                }
                else
                {
                    buffer[i] = guess;
                }
            }
        }
        return new string(buffer);
    }

    /// <summary>
    /// If you want to see if the entire cipher is known (i.e., for all scrambled letters A每Z,
    /// the player has typed in a valid guess), you can check that here.
    /// </summary>
    public void OnCheckCipherComplete()
    {
        if (IsCipherFullyKnown())
        {
            if (feedbackText) feedbackText.text = "All letters discovered! Cipher complete.";
        }
        else
        {
            if (feedbackText) feedbackText.text = "Some letters remain unknown.";
        }
    }

    private bool IsCipherFullyKnown()
    {
        // If for every scrambled letter A每Z, playerGuessMap[letter] != '_',
        // we consider it fully known
        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (playerGuessMap[c] == '_')
                return false;
        }
        return true;
    }

    /// <summary>
    /// Look up how each letter in preRevealedPlainLetters was scrambled,
    /// then fill in the grid with that correct mapping.
    /// </summary>
    private void RevealPreFilledLetters()
    {
        foreach (char letter in preRevealedPlainLetters)
        {
            char upper = char.ToUpper(letter);
            // Only proceed if it's A每Z and in our map
            if (upper >= 'A' && upper <= 'Z' && encryptMap.ContainsKey(upper))
            {
                // The scrambled letter for 'upper'
                char scrambledLetter = encryptMap[upper];  // e.g. if upper='H' => scrambledLetter='X'

                // Mark that in playerGuessMap so partial text will show it
                playerGuessMap[scrambledLetter] = upper;

                // Also fill in the corresponding InputField 
                // index for scrambledLetter is (scrambledLetter - 'A')
                int index = scrambledLetter - 'A';
                if (index >= 0 && index < letterMappingInputs.Length)
                {
                    letterMappingInputs[index].text = upper.ToString();
                }
            }
        }
    }
}
