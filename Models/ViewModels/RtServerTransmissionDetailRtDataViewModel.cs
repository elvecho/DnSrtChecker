using System.ComponentModel.DataAnnotations;

namespace DnSrtChecker.Models.ViewModels
{
    public class RtServerTransmissionDetailRtDataViewModel
    { 
        public long LRtDataId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public decimal _dVatRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal DVatRate { 
            get { return _dVatRate; }
            set { 
                decimal dVatRate;
                decimal.TryParse(value.ToString(), out dVatRate);
                this._dVatRate = dVatRate;
            }
        }
        public decimal? _dVatAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DVatAmount { 
            get { return _dVatAmount; } 
            set {
                decimal dVatAmount;
                decimal.TryParse(value.ToString(), out dVatAmount);
                this._dVatAmount = dVatAmount;
            } 
        }
        public string SzVatNature { get; set; }
        public string SzVatLegalReference { get; set; }
        public short? BVatVentilation { get; set; }
        public decimal? _dSaleAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DSaleAmount {
            get { return _dSaleAmount; } 
            set { 
                decimal dSaleAmount;
                decimal.TryParse(value.ToString(), out dSaleAmount);
                this._dSaleAmount = dSaleAmount;
            }
        }
        public decimal? _dReturnAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DReturnAmount {
            get { return _dReturnAmount; }
            set {
                decimal dReturnAmount;
                decimal.TryParse(value.ToString(), out dReturnAmount);
                this._dReturnAmount = dReturnAmount;
            } 
        }
        public decimal? _dVoidAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DVoidAmount { 
            get { return _dVoidAmount; }
            set {
                decimal dVoidAmount;
                decimal.TryParse(value.ToString(), out dVoidAmount);
                this._dVoidAmount = dVoidAmount;
            } 
        }

        public virtual  RtServerTransmissionDetailViewModel RtServerTransmissionDetail { get; set; }
    }
}
