namespace Osprey_Project
{
    internal class BookEvent
    {
        public string Id { get; set; }  // MongoDB Unique ID
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Event { get; set; } 
        public DateTime Date { get; set; }
    }
}