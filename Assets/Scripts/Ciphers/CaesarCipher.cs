using TMPro;
using UnityEngine;

public class CaesarCipher: MonoBehaviour
{
    // You can make this public or private with a serialized field so you can set it in the Inspector.
    [SerializeField]
    private int shiftKey = 3;

    // This could be your original secret message
    [SerializeField]
    private string secretMessage = "HELLO UNITY";

    // Reference to UI elements
    public TMP_Text encryptedText;     // The text that displays the scrambled text
    public TMP_InputField userInput;   // Where the user types in a guess
    public TMP_Text feedbackText;      // Displays success/failure or hints

    private string encryptedMessage;

    void Start()
    {
        // Generate the scrambled text when the puzzle starts
        encryptedMessage = EncryptCaesarCipher(secretMessage, shiftKey);
        encryptedText.text = encryptedMessage;
    }

    // Caesar Cipher encryption
    private string EncryptCaesarCipher(string input, int shift)
    {
        // For example, uppercase only:
        input = input.ToUpper();
        char[] buffer = input.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char letter = buffer[i];
            // A = 65, Z = 90 in ASCII
            if (letter >= 'A' && letter <= 'Z')
            {
                letter = (char)(letter + shift);
                if (letter > 'Z')
                {
                    letter = (char)(letter - 26); // wrap around
                }
                buffer[i] = letter;
            }
        }
        return new string(buffer);
    }

    // Caesar Cipher decryption ¨C you can call this if you want to compare user input
    private string DecryptCaesarCipher(string input, int shift)
    {
        input = input.ToUpper();
        char[] buffer = input.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char letter = buffer[i];
            if (letter >= 'A' && letter <= 'Z')
            {
                letter = (char)(letter - shift);
                if (letter < 'A')
                {
                    letter = (char)(letter + 26); // wrap around
                }
                buffer[i] = letter;
            }
        }
        return new string(buffer);
    }

    // Method that gets called when the user hits "Submit" button
    public void CheckUserInput()
    {
        string userGuess = userInput.text.ToUpper();
        // Compare guess to the secretMessage or see if decrypting the encrypted text with user¡¯s shift matches the secret
        if (userGuess == secretMessage)
        {
            feedbackText.text = "Success! You solved the cipher.";
        }
        else
        {
            feedbackText.text = "Incorrect. Try again!";
        }
    }
}
