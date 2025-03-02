namespace Osprey_Project
{
    internal class Contact
    {
        public string Id { get; set; }  // MongoDB Unique ID
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}