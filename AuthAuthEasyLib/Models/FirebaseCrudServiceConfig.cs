using Firebase.Database;

namespace AuthAuthEasyLib.Services
{
    public class FirebaseCrudServiceConfig 
    {
        public FirebaseCrudServiceConfig(string url, string resourceName, FirebaseOptions options = null)
        {
            Url = url;
            Options = options;
            ResourceName = resourceName;
        }

        public string Url { get; set; }
        public FirebaseOptions Options { get; set; }
        public string ResourceName { get; set; }
    }
}
