using System;
using SQLite; // из sqlite-net-pcl

namespace PotionPanic.Models
{
    [Table("GameResults")]
    public class GameResult
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string? PlayerName { get; set; }

        public int Score { get; set; }

        public DateTime DateUtc { get; set; } = DateTime.UtcNow;

        public int? DurationSec { get; set; }

        public string? Notes { get; set; }
    }
}
