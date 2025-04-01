using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SubstitutionPuzzleManager : MonoBehaviour
{
    [SerializeField] private TMP_Text missingLettersText;

    [SerializeField] public Transform symbolGridParent;
    [SerializeField] private Shop shop;
    [SerializeField]
    private string[] extraPhrases = {
    };

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

    // 26 input fields for letters A�CZ. Index 0 = scrambled 'A', 1 = scrambled 'B', ...
    [SerializeField] private TMP_InputField[] letterMappingInputs = new TMP_InputField[26];

    // A random cipher mapping plain->scrambled
    private Dictionary<char, char> encryptMap = new Dictionary<char, char>();
    // The inverse mapping (scrambled->plain), so we can easily check if a guess is correct
    private Dictionary<char, char> decryptMap = new Dictionary<char, char>();

    // The player's guess map for scrambled->plain.
    // Key = scrambled letter, Value = guessed plain letter (or '_' if unknown).
    private Dictionary<char, char> playerGuessMap = new Dictionary<char, char>();

    // Which phrase we��re currently on
    private int currentPhraseIndex = 0;

    // The scrambled text of the current phrase
    private string currentScrambledPhrase = "";

    void Start()
    {
        InitializeMultiPuzzle();
        ShuffleChildrenUnderGrid();
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

        // Clear guess map: scrambled letters (A�CZ) => '_'
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
        currentScrambledPhrase = Monospace(EncryptWithSubstitution(phraseToEncrypt, encryptMap));

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
        string cleaned = userDecrypted.Replace("<mspace=30>", "").Replace("</mspace>", "");

        string actualPhrase = secretPhrases[currentPhraseIndex];
        bool matches = cleaned.Equals(actualPhrase, System.StringComparison.OrdinalIgnoreCase);
        // Compare ignoring case
        if (matches)
        {
            // Reveal all letter mappings from this phrase so the user��s cipher knowledge grows
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
        UpdateMissingLettersDisplay();

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
            string partial = Monospace(DecryptWithPlayerGuess(currentScrambledPhrase));
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
            if (shop != null)
            {
                shop.money += 100;
                shop.UpdateText(); // Refresh UI in the Shop
            }
            if (feedbackText != null)
                feedbackText.text = "All letters discovered! Cipher complete.";
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Some letters remain unknown.";
        }
        ApplyLetterColors();
    }

    private bool IsCipherFullyKnown()
    {
        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (playerGuessMap[c] == '-')
                return false;
        }
        return true;
    }

    /// <summary>
    /// Wraps the input string in <mspace=30> tags to display it in monospace font.
    public static string Monospace(string input)
    {
        return "<mspace=30>" + input + "</mspace>";
    }
    public void OnRequestNewCipher()
    {
        if (!IsCipherFullyKnown())
        {
            if (feedbackText != null)
                feedbackText.text = "You must complete the cipher before requesting a new one!";
            return;
        }
        // 1) Generate new cipher
        encryptMap = GenerateSubstitutionMapping();





        decryptMap.Clear();
        foreach (var kvp in encryptMap)
        {
            decryptMap[kvp.Value] = kvp.Key;
        }
        ShuffleChildrenUnderGrid();

        // 2) Reset the player’s guess map
        playerGuessMap.Clear();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            playerGuessMap[c] = '-';
        }

        // 3) Clear all input fields & reattach listeners
        for (int i = 0; i < letterMappingInputs.Length; i++)
        {
            if (letterMappingInputs[i] != null)
            {
                // Clear text and color
                letterMappingInputs[i].onValueChanged.RemoveAllListeners();
                letterMappingInputs[i].text = "";
                if (letterMappingInputs[i].GetComponent<Image>() != null)
                {
                    letterMappingInputs[i].GetComponent<Image>().color = Color.white;
                }

                // Add back the listener so we can capture new guesses
                int index = i;
                letterMappingInputs[i].onValueChanged.AddListener(
                    (value) => OnMappingChanged(index, value));
            }
        }
        RevealPreFilledLetters();

        // 4) Re-encrypt *the current puzzle* with the new cipher
        if (currentPhraseIndex < secretPhrases.Length)
        {
            string phraseToEncrypt = secretPhrases[currentPhraseIndex];
            currentScrambledPhrase = EncryptWithSubstitution(phraseToEncrypt, encryptMap);

            // Update the Encrypted Text label
            if (encryptedText != null)
                encryptedText.text = currentScrambledPhrase;

            // Refresh partial text (should show all underscores except pre-revealed letters)
            UpdatePartialDecryption();

            if (feedbackText != null)
                feedbackText.text = "New cipher applied! Solve this puzzle again with the new cipher.";
        }
        else
        {
            // If the player was already past the last phrase, do nothing or show a message
            if (feedbackText != null)
                feedbackText.text = "No remaining puzzles to re-encrypt.";
        }
    }
    private void ShuffleChildrenUnderGrid()
    {
        // Get a list of all current children
        List<Transform> children = new List<Transform>();
        foreach (Transform child in symbolGridParent)
        {
            children.Add(child);
        }

        // Shuffle this list
        System.Random rand = new System.Random();
        for (int i = 0; i < children.Count; i++)
        {
            int r = rand.Next(i, children.Count);
            Transform temp = children[i];
            children[i] = children[r];
            children[r] = temp;
        }

        // Now reassign sibling indices in the new order
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }

    private void ApplyLetterColors()
    {
        for (int i = 0; i < letterMappingInputs.Length; i++)
        {
            // 'A' + i = the scrambled letter we’re dealing with
            char scrambledLetter = (char)('A' + i);
            char guessedChar = playerGuessMap[scrambledLetter];

            // If there's a guess and we have the correct mapping in decryptMap
            if (guessedChar != '_' && decryptMap.ContainsKey(scrambledLetter))
            {
                char actualPlain = decryptMap[scrambledLetter];
                if (guessedChar == actualPlain)
                {
                    // Correct
                    letterMappingInputs[i].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    // Incorrect
                    letterMappingInputs[i].GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                // Blank or unknown
                letterMappingInputs[i].GetComponent<Image>().color = Color.white;
            }
        }
    }
    public void AddSingleExtraPhrase()
    {
        // 1) If no extra phrases remain, do nothing or show a message
        if (extraPhrases == null || extraPhrases.Length == 0)
        {
            if (feedbackText != null)
                feedbackText.text = "No more extra phrases available.";
            return;
        }

        // 2) Convert extraPhrases to a List so we can remove from it
        List<string> extraList = new List<string>(extraPhrases);

        // 3) Pick one phrase from the extra list
        //    a) Random pick
        System.Random rand = new System.Random();
        int r = rand.Next(0, extraList.Count);
        string chosenPhrase = extraList[r];



        // 4) Remove that phrase from the extraList so it isn’t added again
        extraList.RemoveAt(r);

        // 5) Update extraPhrases array to reflect that removal
        extraPhrases = extraList.ToArray();

        // 6) Append the chosen phrase to secretPhrases
        List<string> secretList = new List<string>(secretPhrases);
        secretList.Add(chosenPhrase);
        secretPhrases = secretList.ToArray();


        // 8) Show feedback
        if (feedbackText != null)
        {
            feedbackText.text = $"Extra phrase added: {chosenPhrase}";
        }

        Debug.Log($"Appended extra phrase: {chosenPhrase}");
    }



    public void RevealRandomUnknownLetter()
    {

        List<char> unrevealedPlainLetters = new List<char>();
        for (char plain = 'A'; plain <= 'Z'; plain++)
        {
            // If the cipher maps this plain letter to some scrambled letter
            if (encryptMap.ContainsKey(plain))
            {
                char scrambled = encryptMap[plain];

                // Check if the player's guess for that scrambled letter is still unknown
                if (playerGuessMap[scrambled] == '_' || playerGuessMap[scrambled] == '-')
                {
                    unrevealedPlainLetters.Add(plain);
                }
            }
        }

        // 2) If everything is already revealed, there's nothing to reveal!
        if (unrevealedPlainLetters.Count == 0)
        {
            if (feedbackText != null)
                feedbackText.text = "No unknown letters remain to reveal!";
            return;
        }

        // 3) Pick ONE random letter from the list
        System.Random rand = new System.Random();
        int randomIndex = rand.Next(unrevealedPlainLetters.Count);
        char letterToReveal = unrevealedPlainLetters[randomIndex]; 

        // 4) Reveal that letter: find its scrambled version and fill in the guess map
        char scrambledLetter = encryptMap[letterToReveal];

        // Update the guess map with the real plain letter
        playerGuessMap[scrambledLetter] = letterToReveal;

        // Update the corresponding InputField in the UI
        int fieldIndex = scrambledLetter - 'A';
        if (fieldIndex >= 0 && fieldIndex < letterMappingInputs.Length)
        {
            letterMappingInputs[fieldIndex].text = letterToReveal.ToString();

            // Color it green, since it's a "correct reveal"
            var img = letterMappingInputs[fieldIndex].GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                img.color = Color.green;
            }
        }

        // 5) Refresh partial text to reflect the newly revealed letter
        UpdatePartialDecryption();

        if (feedbackText != null)
            feedbackText.text = $"Revealed a random letter: {letterToReveal}";
    }

    private void UpdateMissingLettersDisplay()
    {
        if (missingLettersText == null) return;  // If no text assigned, do nothing.

        List<char> missing = new List<char>();

        // For each PLAIN letter from A..Z, see if it's been guessed
        for (char plain = 'A'; plain <= 'Z'; plain++)
        {
            // If our cipher maps this plain letter to a scrambled letter...
            if (encryptMap.ContainsKey(plain))
            {
                char scrambled = encryptMap[plain];
                // Check if the player's guess for that scrambled letter is still unknown
                char guess = playerGuessMap[scrambled];
                if (guess == '-' || guess == '_')
                {
                    // This means the user hasn't discovered (guessed) that letter yet
                    missing.Add(plain);
                }
            }
        }

        // Format the text
        if (missing.Count == 0)
        {
            missingLettersText.text = "No missing letters (all solved)!";
        }
        else
        {
            // Join them like "Missing letters: A, M, X, Y"
            missingLettersText.text = "Missing letters: " + string.Join(", ", missing);
        }
    }

}
