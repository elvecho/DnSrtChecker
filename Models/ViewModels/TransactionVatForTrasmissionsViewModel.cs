using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransactionVatForTrasmissionsViewModel
    {       
        public decimal? _dPosVatRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DPosVatRate
        {
            get { return _dPosVatRate; }
            set
            {
                decimal dPosVatRate;
                decimal.TryParse(value.ToString(), out dPosVatRate);
                this._dPosVatRate = dPosVatRate;
            }
        }
        public decimal? _dPosGrossAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DPosGrossAmount
        {
            get { return _dPosGrossAmount; }
            set
            {
                decimal dPosGrossAmount;
                decimal.TryParse(value.ToString(), out dPosGrossAmount);
                this._dPosGrossAmount = dPosGrossAmount;
            }
        }
       
       
        public decimal? _dRtVatRate { get; set; }
        //[DisplayFormat(DataFormatString = "{0:N0}%", ApplyFormatInEditMode = true)]
        public decimal? DRtVatRate
        {
            get { return _dRtVatRate; }
            set
            {
                decimal dRtVatRate;
                decimal.TryParse(value.ToString(), out dRtVatRate);
                this._dRtVatRate = dRtVatRate;
            }
        }
        public decimal? _dRtGrossAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DRtGrossAmount
        {
            get { return _dRtGrossAmount; }
            set
            {
                decimal dRtGrossAmount;
                decimal.TryParse(value.ToString(), out dRtGrossAmount);
                this._dRtGrossAmount = dRtGrossAmount;
            }
        }
        public decimal _dVatRate { get; set; }

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
        public decimal? _dVatAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DVatAmount
        {
            get { return _dVatAmount; }
            set
            {
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
        public decimal? DSaleAmount
        {
            get { return _dSaleAmount; }
            set
            {
                decimal dSaleAmount;
                decimal.TryParse(value.ToString(), out dSaleAmount);
                this._dSaleAmount = dSaleAmount;
            }
        }
        public decimal? _dReturnAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DReturnAmount
        {
            get { return _dReturnAmount; }
            set
            {
                decimal dReturnAmount;
                decimal.TryParse(value.ToString(), out dReturnAmount);
                this._dReturnAmount = dReturnAmount;
            }
        }
        public decimal? _dVoidAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DVoidAmount
        {
            get { return _dVoidAmount; }
            set
            {
                decimal dVoidAmount;
                decimal.TryParse(value.ToString(), out dVoidAmount);
                this._dVoidAmount = dVoidAmount;
            }
        }
       

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DSumAmount
        {
            get { return DSaleAmount+DVatAmount-DReturnAmount-DVoidAmount; }
            
        }
        public RtServerVat SzVatCode { get; set; }
       

    }
}
