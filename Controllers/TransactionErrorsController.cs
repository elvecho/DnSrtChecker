using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DnSrtChecker.Models;
using DnSrtChecker.Models.ViewModels;
using DnSrtChecker.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DnSrtChecker.Controllers
{
    [Authorize]
    public class TransactionErrorsController : BasicController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRtErrorRepository _transactionRtErrorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionErrorsController> _logger;

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public TransactionErrorsController(IUnitOfWork unitOfWork,
                                           ITransactionRtErrorRepository transactionRtErrorRepository,
                                           ILogger<TransactionErrorsController> logger,
                                           UserManager<User> userManager,
                                           RoleManager<Role> roleManager,
                                           IMapper mapper) :base(userManager,roleManager)
        {
            _unitOfWork = unitOfWork;
            _transactionRtErrorRepository = transactionRtErrorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(IDictionary<string,string> routeValues)
        {
            
            if (string.IsNullOrWhiteSpace(routeValues["IdDocument"]))
            {
                NotFound();
            }
            var getTransactions = await _transactionRtErrorRepository.GetTransactionRtError(Int32.Parse(routeValues["IdDocument"]), routeValues["RtServerId"], Int32.Parse(routeValues["RetailStoreId"]), Int32.Parse( routeValues["StoreGroupId"]),routeValues["DeviceId"]);
            if(getTransactions.Count==0)
            {
                return NotFound();
            }
            return View(_mapper.Map<List<TransactionRtError>, List<TransactionRtErrorViewModel>>(getTransactions));
        }
    }
}