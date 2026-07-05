using FinanceTracker.Application.DTOs.Transaction;
using FinanceTracker.Application.Exceptions;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TransactionsApiController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public TransactionsApiController(ITransactionService transactionService, IAccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("by-account/{accountId}")]
    public async Task<IActionResult> GetByAccount(Guid accountId)
    {
        var account = await _accountService.GetAccountByIdAsync(accountId);
        if (account.UserId != GetUserId())
            return Forbid();

        var transactions = await _transactionService.GetTransactionsByAccountAsync(accountId);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();
            return Ok(transaction);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var account = await _accountService.GetAccountByIdAsync(dto.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();

            var result = await _transactionService.CreateTransactionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TransactionUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();

            await _transactionService.UpdateTransactionAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account.UserId != GetUserId())
                return Forbid();

            await _transactionService.DeleteTransactionAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/all")]
    public async Task<IActionResult> GetAllForAdmin(
    [FromQuery] Guid? userId = null,
    [FromQuery] Guid? accountId = null,
    [FromQuery] DateTime? from = null,
    [FromQuery] DateTime? to = null)
    {
        var transactions = await _transactionService.GetAllTransactionsAsync(userId, accountId, from, to);
        return Ok(transactions);
    }
}