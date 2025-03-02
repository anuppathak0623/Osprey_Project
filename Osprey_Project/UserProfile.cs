namespace Osprey_Project
{
    internal class UserProfile
    {
        public string Id { get; set; }  // MongoDB Unique ID
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Consider hashing before storing
        public string DOB { get; set; }
        public string Address { get; set; }

    }
}