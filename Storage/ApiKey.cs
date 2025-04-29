namespace P2PPublicReverseProxy.Storage
{
    public class ApiKey
    {
        public long uid;
        public string key; // 18 chars
        public string secret; // 36 chars

        public ApiKey(long uid)
        {
            this.uid = uid;
            this.key = GenerateKeys(18);
            this.secret = GenerateKeys(36);
        }

        private string GenerateKeys(int len)
        {
            var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                        "abcdefghijklmnopqrstuvwxyz" +
                        "0123456789";
            var stringChars = new char[len];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = alpha[random.Next(alpha.Length)];
            }
            return new string(stringChars);
        }
    }
}