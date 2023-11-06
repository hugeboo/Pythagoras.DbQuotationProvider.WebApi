using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pythagoras.DbQuotationProvider.Persistence.Entities
{
    [Table("h_quotation_ticks")]
    public sealed class HQuotationTick
    {
        public const int LAST_TICK = 3;
        public const int OPEN_TICK = 4;
        public const int HIGH_TICK = 5;
        public const int LOW_TICK = 6;
        public const int CLOSE_TICK = 7;
        public const int VOLUME_TICK = 8;

        [Column("ticker")]
        public string Ticker { get; set; } = string.Empty;

        // TODO: create index [Time, TypeId]
        [Column("time")]
        public DateTime Time { get; set; }

        [Column("type_id")]
        public int TypeId { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        // TODO: rename column to Size
        [Column("qtty")]
        public long Size { get; set; }

        [Column("ts_id")]
        public int TsId { get; set; }
    }
}
