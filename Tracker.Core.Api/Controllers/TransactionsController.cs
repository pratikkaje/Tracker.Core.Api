using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
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
            try
            {
                Transaction addedTransaction =
                    await this.transactionService.AddTransactionAsync(transaction);

                return Created(addedTransaction);
            }
            catch (TransactionValidationException transactionValidationException)
            {
                return BadRequest(transactionValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
                when (transactionDependencyValidationException.InnerException is AlreadyExistsTransactionException)
            {
                return Conflict(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
            {
                return BadRequest(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Transaction>>> GetTransactionsAsync()
        {
            try
            {
                IQueryable<Transaction> transactions =
                    await this.transactionService.RetrieveAllTransactionsAsync();

                return Ok(transactions);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }

        [HttpGet("{transactionId}")]
        public async ValueTask<ActionResult<Transaction>> GetTransactionByIdAsync(Guid transactionId)
        {
            try
            {
                Transaction transaction =
                    await this.transactionService.RetrieveTransactionByIdAsync(transactionId);

                return Ok(transaction);
            }
            catch (TransactionValidationException transactionValidationException)
                when (transactionValidationException.InnerException is NotFoundTransactionException)
            {
                return NotFound(transactionValidationException.InnerException);
            }
            catch (TransactionValidationException transactionValidationException)
            {
                return BadRequest(transactionValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
            {
                return BadRequest(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }

        }

        [HttpPut]
        public async ValueTask<ActionResult<Transaction>> PutTransactionAsync(Transaction transaction)
        {
            try
            {
                Transaction modifiedTransaction =
                    await this.transactionService.ModifyTransactionAsync(transaction);

                return Ok(modifiedTransaction);
            }
            catch (TransactionValidationException transactionValidationException)
                when (transactionValidationException.InnerException is NotFoundTransactionException)
            {
                return NotFound(transactionValidationException.InnerException);
            }
            catch (TransactionValidationException transactionValidationException)
            {
                return BadRequest(transactionValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
                when (transactionDependencyValidationException.InnerException is AlreadyExistsTransactionException)
            {
                return Conflict(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
            {
                return BadRequest(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }
    }
}
