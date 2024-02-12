using AutoMapper;
using Bank_Web_Api.Model;
using Bank_Web_Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Bank_Web_Api.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class AccountsController:ControllerBase
    {
        //now let's inject the accountService
        private IAccountService _accountService;
        //Let's bring AutoMapper
        IMapper _mapper;
        public AccountsController(IAccountService accountService,IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        //registernewAccount
        [HttpPost]
        [Route("register_new_account")]
        public IActionResult RegisterNewAccount([FromBody] RegisterNewAccountModel newAccount)
        {
            if (!ModelState.IsValid) return BadRequest(newAccount);

            //map to account
            var account = _mapper.Map<Account>(newAccount);
            return Ok(_accountService.Create(account,newAccount.Pin,newAccount.ConfirmPin));
        }
        
        [HttpGet]
        [Route("get_all_accounts")]
        public IActionResult GetAllAccount()
        {
            //we want to map to GetAccountModel as define in ur
            var accounts = _accountService.GetAllAccount();
            var cleanedAccounts = _mapper.Map<IList<GetAccountModel>>(accounts);
            return Ok(cleanedAccounts);
        }
        
        [HttpPost]
        [Route("authentication")]
        public IActionResult Authenticate([FromBody] AuthenticationModel model)
        {
            if (!ModelState.IsValid) return BadRequest(model);
            return Ok(_accountService.Authentication(model.AccountNumber, model.Pin));
        }
        
        [HttpGet]
        [Route("get_by_account_number")]
        public IActionResult GetByAccountNumber(string AccountNumber)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account Number must be 10 digits");
            var account = _accountService.GetByAccountNumber(AccountNumber);
            var cleanedAccount = _mapper.Map<GetAccountModel>(account);
            return Ok(cleanedAccount);
        }
       
        [HttpGet]
        [Route("get_account_by_id")]
        public IActionResult GetAccountById(int Id)
        {
            var account = _accountService.GetById(Id);
            var cleanedAccount = _mapper.Map<GetAccountModel>(account);
            return Ok(cleanedAccount);
        }
        
        [HttpPut]
        [Route("update_account")]
        public IActionResult UpdateAccount([FromBody] UpdateAccountModel model)
        {
            if (!ModelState.IsValid) return BadRequest(model);

            var account = _mapper.Map<Account>(model);
            _accountService.Update(account, model.Pin);
            return Ok();
        }

    }
}
