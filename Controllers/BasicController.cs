using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnSrtChecker.FiltersmodelBindRequest;
using DnSrtChecker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnSrtChecker.Controllers
{
    public class BasicController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public BasicController(UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void PopulateStatusDropDownList(object selectedStatus = null)
        {
            
            List<SelectListItem> statusItems = new List<SelectListItem>();
            statusItems.Add(new SelectListItem { Text = "Stato Server", Value = "" });
            statusItems.Add(new SelectListItem { Text = "In Servizio", Value = "true" });
            statusItems.Add(new SelectListItem { Text = "Non in Servizio", Value = "false" });
            ViewBag.status = new SelectList(statusItems, "Value", "Text", selectedStatus);
        }
        public void PopulateErrorDropDownList(object selectedError = null)
        {
            List<SelectListItem> errorItems = new List<SelectListItem>();
            errorItems.Add(new SelectListItem { Text = "Controllo Errori", Value = "" });
            errorItems.Add(new SelectListItem { Text = "In Errore", Value = "true" }); 
            errorItems.Add(new SelectListItem { Text = "Nessun Errore", Value = "false" });

            ViewBag.error = new SelectList(errorItems, "Value", "Text", selectedError);
        }
        public void PopulateNonCompliantDropDownList(object selectedNonCompliant = null)
        {
            List<SelectListItem> nonCompliantItems = new List<SelectListItem>();
            nonCompliantItems.Add(new SelectListItem { Text = "Transazioni", Value = "" });
            nonCompliantItems.Add(new SelectListItem { Text = "Non conformi", Value = "true" });
            nonCompliantItems.Add(new SelectListItem { Text = "Conformi", Value = "false" });

            ViewBag.nonCompliant = new SelectList(nonCompliantItems, "Value", "Text", selectedNonCompliant);
        }

        public void PopulateNonCompliantOrHasMismatchDropDownList(object selectedNonCompliantOrHasMismatch = null)
        {
            List<SelectListItem> nonCompliantOrHasMismatchItems = new List<SelectListItem>();
            nonCompliantOrHasMismatchItems.Add(new SelectListItem { Text = "conformita e differenza", Value = "" });
            nonCompliantOrHasMismatchItems.Add(new SelectListItem { Text = "NonConformi/con differenza", Value = "true" });
            nonCompliantOrHasMismatchItems.Add(new SelectListItem { Text = "Conformi/nessun differenza", Value = "false" });

            ViewBag.nonCompliantOrHasMismatch = new SelectList(nonCompliantOrHasMismatchItems, "Value", "Text", selectedNonCompliantOrHasMismatch);
        }

        public void PopulatewithMismatchDropDownList(object selectedWithMismatch = null)
        {
            List<SelectListItem> withMismatchItems = new List<SelectListItem>();
            withMismatchItems.Add(new SelectListItem { Text = "Differenza", Value = "" });
            withMismatchItems.Add(new SelectListItem { Text = "Con differenza", Value = "true" });
            withMismatchItems.Add(new SelectListItem { Text = "Nessun differenza", Value = "false" });

            ViewBag.withMismatch = new SelectList(withMismatchItems, "Value", "Text", selectedWithMismatch);
        }
        public void PopulateisCheckedDropDownList(object selectedIsChecked = null)
        {
            List<SelectListItem> IsCheckedItems = new List<SelectListItem>();
            IsCheckedItems.Add(new SelectListItem { Text = "Stato Verifica", Value = "" });
            IsCheckedItems.Add(new SelectListItem { Text = "Verificata", Value = "true" });
            IsCheckedItems.Add(new SelectListItem { Text = "Non Verificata", Value = "false" });

            ViewBag.isChecked = new SelectList(IsCheckedItems, "Value", "Text", selectedIsChecked);
        }
        public void PopulateisCheckedOrArchivedDropDownList(object selectedIsCheckedOrArchived=null)
        {
            List<SelectListItem> IsCheckedOrArchivedItems = new List<SelectListItem>();
            IsCheckedOrArchivedItems.Add(new SelectListItem { Text = "Stato Verifica", Value = "" });
            IsCheckedOrArchivedItems.Add(new SelectListItem { Text = "Verificata", Value = "isChecked" });
            IsCheckedOrArchivedItems.Add(new SelectListItem { Text = "Non Verificata", Value = "isNotChecked" });
            IsCheckedOrArchivedItems.Add(new SelectListItem { Text = "Archiviata", Value = "isArchived" });
            ViewBag.isCheckedOrArchived = new SelectList(IsCheckedOrArchivedItems, "Value", "Text", selectedIsCheckedOrArchived);

        }
        public void PopulateConformityDropDownList(object selectedConformity = null)
        {
            List<SelectListItem> ConformityItems = new List<SelectListItem>();
            ConformityItems.Add(new SelectListItem { Text = "Conformità e differenza", Value = "" });
            ConformityItems.Add(new SelectListItem { Text = "Conforme/No Differenza", Value = "CompliantHasNotMismatch" });
            ConformityItems.Add(new SelectListItem { Text = "Conforme/Con Differenza", Value = "CompliantHasMismatch" });
            ConformityItems.Add(new SelectListItem { Text = "No Conforme/No Differenza", Value = "NonCompliantHasNotMismtach" });
            ConformityItems.Add(new SelectListItem { Text = "No Conforme/Con Differenza", Value = "NonCompliantHasMismatch" });

            ViewBag.Conformity = new SelectList(ConformityItems, "Value", "Text", selectedConformity);

        }
        public FiltersmodelBindingRequest ResetFilters( )
        {
          
            var filters = new FiltersmodelBindingRequest
            {
                StoreGroup = "",
                Store = "",
                ServerRt = "",
                Status = "",
                NonCompliant = "",
                Error = "",
                TransactionDateFrom = null,
                TransactionDateTo= null,
                IsChecked = null,
                NonCompliantOrHasMismatch = "",
                RtDocumentNmbr="",
                RtClosureNmbr="",
                PosTaNmbr = "",
                TransmissionDateFrom="",
                TransmissionDateTo=""
            };
            return filters;
        }

        public Task<User> GetCurrentuser()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}