namespace BookList.Models.ViewModells
{
    public class SessionVM
    {
        public string token { get; set; }
        public bool loggedIn { get; set; } = false;
    }
}
