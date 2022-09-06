using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class TrxGroupedByDay
    {
        public DateTime? DRtDeviceClosureDateTime { get; set; }
        public string SzRtDeviceId { get; set; }
        private decimal _dVatRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal DVatRate
        {
            get { return _dVatRate; }
            set
            {
                decimal dVatRate;
                decimal.TryParse(value.ToString(), out dVatRate);
                this._dVatRate = dVatRate;
            }
        }
        public string SzVatNature { get; set; }
        private decimal? _sumValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? SumValue
        {
            get { return _sumValue; }
            set
            {
                decimal dVoidAmount;
                decimal.TryParse(value.ToString(), out dVoidAmount);
                this._sumValue = dVoidAmount;
            }
        }
        public decimal? dVoidAmount { get; set; }
        public decimal? dVatAmount { get; set; }
        public decimal? dReturnAmount { get; set; }
        public decimal? dSaleAmount { get; set; }
    }
}
