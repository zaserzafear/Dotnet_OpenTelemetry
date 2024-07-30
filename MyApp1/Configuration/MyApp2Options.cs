namespace MyApp1.Configuration
{
    public class MyApp2Options
    {
        private string _baseUrl = string.Empty;
        private int _timeout = 30;

        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("BaseUrl cannot be null or empty.");
                _baseUrl = value;
            }
        }

        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Timeout must be a positive integer.");
                _timeout = value;
            }
        }
    }
}
