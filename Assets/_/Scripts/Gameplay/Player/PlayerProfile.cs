using System;
using System.Security.Cryptography;

namespace _.Scripts.Gameplay.Player
{
    public class PlayerProfile
    {
        private string _id;

        public string ID => _id;
        
        public PlayerProfile()
        {
            _id = GenerateSecureID();
        }

        private string GenerateSecureID()
        {
            byte[] byteArray = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(byteArray);
            
            return Convert.ToBase64String(byteArray)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
    }
}