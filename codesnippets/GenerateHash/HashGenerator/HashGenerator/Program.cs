using System;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static void Main()
    {
        // Hash obtained from JavaScript
        string hashedPassword = "2jmj7l5rSw0yVb/vlWAYkK/YBwk=";

        // Create a new instance of the SHA256 algorithm
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Convert the hash to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(hashedPassword);

            // Compute the hash
            byte[] hashedBytes = sha256Hash.ComputeHash(bytes);

            // Convert the hashed bytes to a string
            string finalHash = BitConverter.ToString(hashedBytes).Replace("-", string.Empty).ToLower();

            Console.WriteLine("Final Hash: " + finalHash);
        }

        Console.ReadKey();
    }
}