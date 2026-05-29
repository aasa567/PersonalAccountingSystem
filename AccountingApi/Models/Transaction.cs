using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingApi.Models
{
    public class Transaction
    {
        [Key] // 代表這是主鑑 (Primary Key)
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title {  get; set; } = string.Empty; //支出項目名稱，例如: 午餐

        [Column(TypeName = "decimal(18, 2)")] // 設定金錢格式
        public decimal Amount { get; set; } // 金額
        public string Category { get; set; } = "食"; 
        public DateTime Date { get; set; } = DateTime.Now; // 日期

        public string? Note { get; set; } // 備註 (可為空)

        [Required]
        public string Type { get; set; } = "Expense"; // 類型：Expense (支出) 或 Income (收入)
    }
}
