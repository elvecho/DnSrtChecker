using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class TRXTotalsByDays
    {
        public DateTime? DRtDeviceClosureDateTime { get; set; }
        public string SzRtServerId { get; set; }
        private decimal? _total { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? Total
        {
            get { return _total; }
            set
            {
                decimal total;
                decimal.TryParse(value.ToString(), out total);
                this._total = total;
            }
        }
    }
}
