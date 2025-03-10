using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RivertyTasks.Models
{
    public class CurrencyInfo
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }

    [Table("EXCHANGE_RATE", Schema = "CURRENCY")]
    public class ExchangeRate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BaseCurrency { get; set; }

        [Required]
        public string TargetCurrency { get; set; }

        [Required]
        public double ExchangeRateValue { get; set; }

        [Required]
        public DateTime ExchangeDate { get; set; }
    }
}
