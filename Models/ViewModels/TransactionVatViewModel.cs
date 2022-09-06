using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DnSrtChecker.Models.ViewModels
{
    public class TransactionVatViewModel
    {
        public string SzRtDocumentId { get; set; }
        public string SzVatCodeId { get; set; }
        public int CounterPos { get; set; }
        public int CounterRT { get; set; }

        public decimal? _dPosVatRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DPosVatRate {
            get { return _dPosVatRate; }
            set {
                decimal dPosVatRate;
                decimal.TryParse(value.ToString(), out dPosVatRate);
                this._dPosVatRate = dPosVatRate;
            }
        }
        public decimal? _dPosGrossAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DPosGrossAmount {
            get { return _dPosGrossAmount; }
            set
            {
                decimal dPosGrossAmount;
                decimal.TryParse(value.ToString(), out dPosGrossAmount);
                this._dPosGrossAmount = dPosGrossAmount;
            }
        }
        public decimal? _dPosNetAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DPosNetAmount {
            get { return _dPosNetAmount; }
            set
            {
                decimal dPosNetAmount;
                decimal.TryParse(value.ToString(), out dPosNetAmount);
                this._dPosNetAmount = dPosNetAmount;
            } 
        }
        public decimal? _dPosVatAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DPosVatAmount
        {
            get { return _dPosVatAmount; }
            set
            {
                decimal dPosVatAmount;
                decimal.TryParse(value.ToString(), out dPosVatAmount);
                this._dPosVatAmount = dPosVatAmount;
            }
        }
        public decimal? _dRtVatRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}%", ApplyFormatInEditMode = true)]
        public decimal? DRtVatRate {
            get { return _dRtVatRate; }
            set {
                decimal dRtVatRate;
                decimal.TryParse(value.ToString(), out dRtVatRate);
                this._dRtVatRate = dRtVatRate;
            }
        }
        public decimal? _dRtGrossAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DRtGrossAmount {
            get { return _dRtGrossAmount; }
            set
            {
                decimal dRtGrossAmount;
                decimal.TryParse(value.ToString(), out dRtGrossAmount);
                this._dRtGrossAmount = dRtGrossAmount;
            }
        }
        public decimal? _dRtNetAmount { get; set; }
        [DataType(dataType: DataType.Currency), DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DRtNetAmount
        {
            get { return _dRtNetAmount ; }
            set
            {
                decimal dRtNetAmount;
                decimal.TryParse(value.ToString(), out dRtNetAmount);
                this._dRtNetAmount = dRtNetAmount;
            }
        }
        public decimal? _dRtVatAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DRtVatAmount {
            get { return _dRtVatAmount ; }
            set
            {
                decimal dRtVatAmount;
                decimal.TryParse(value.ToString(), out dRtVatAmount);
                this._dRtVatAmount = dRtVatAmount;
            }
        }
        public bool? BVatMismatchFlag { get; set; }
        public bool? BVatCheckedFlag { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        //calculate values
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DMismatchGrossAmount { 
            get {
                try
                {
                   return DPosGrossAmount - DRtGrossAmount;
                }
                catch (Exception e)
                {
                    return 0;

                }
            } 
        }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DMismatchNetAmount { 
            get {
                try
                {
                    return DPosNetAmount.GetValueOrDefault() - DRtNetAmount.GetValueOrDefault();
                }
                catch (Exception e)
                {
                    return  0;

                }
            } 
        }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? DMismatchVatAmount {
            get {
                try
                {
                    return DPosVatAmount.GetValueOrDefault() - DRtVatAmount.GetValueOrDefault();

                }
                catch (Exception e)
                {
                    return 0;

                }
            } 
        }
        public  TransactionAffiliationViewModel LTransactionAffiliation { get ; set; }
        public virtual RtServerVat SzVatCode { get; set; }

    }
}
