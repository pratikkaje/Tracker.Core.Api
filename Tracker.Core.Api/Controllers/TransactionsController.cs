using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Services.Foundations.Transactions;

namespace Tracker.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : RESTFulController
    {
        private readonly ITransactionService transactionService;

        public TransactionsController(ITransactionService transactionService) =>
            this.transactionService = transactionService;

        [HttpPost]
        public async ValueTask<ActionResult<Transaction>> PostTransactionAsync(Transaction transaction)
        {
            Transaction addedTransaction =
                await this.transactionService.AddTransactionAsync(transaction);

            return Created(addedTransaction);
        }
    }
}
