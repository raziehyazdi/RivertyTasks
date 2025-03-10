using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("EXCHANGE_RATE", Schema = "CURRENCY")]
public class ExchangeRate
{
    [Key]
    public int Id { get; set; }
    public string BaseCurrency { get; set; }
    public string TargetCurrency { get; set; }
    public double ExchangeRateValue { get; set; }
    public DateTime ExchangeDate { get; set; }
}