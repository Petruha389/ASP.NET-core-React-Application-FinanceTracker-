using FinanceTracker.Application.DTOs.Account;
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
public class AccountsApiController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsApiController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var accounts = await _accountService.GetAccountsByUserAsync(userId);
        return Ok(accounts);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/all")]
    public async Task<IActionResult> GetAllForAdmin()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId())
                return Forbid();
            return Ok(account);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccountCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        dto.UserId = GetUserId();
        try
        {
            var result = await _accountService.CreateAccountAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AccountUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId())
                return Forbid();
            await _accountService.UpdateAccountAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId())
                return Forbid();
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // ========== НОВЫЕ МЕТОДЫ ==========

    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] AccountOperationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId())
                return Forbid();

            await _accountService.DepositAsync(id, dto.Amount, dto.Description ?? "Пополнение");
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] AccountOperationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account.UserId != GetUserId())
                return Forbid();

            await _accountService.WithdrawAsync(id, dto.Amount, dto.Description ?? "Снятие");
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}