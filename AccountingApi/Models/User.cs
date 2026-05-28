namespace AccountingApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username {  get; set; } = string.Empty;

        // ❌ 絕對不要直接存 Password！
        // ✅ 我們存 PasswordHash，這是加密後的「亂碼」
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
