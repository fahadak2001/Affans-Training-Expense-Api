using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using ExpenseBackend.Models;
using ExpenseBackend.Services;

namespace ExpenseBackend.Controllers;

[ApiController]
[Route("api/expenses")]
[Produces("application/json")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IDistributedCache _distributedCache;

    public ExpenseController(IExpenseService expenseService, IDistributedCache distributedCache)
    {
        _expenseService = expenseService;
        _distributedCache = distributedCache;
    }

    [HttpGet("List")]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        string cacheKey = $"expenses:{username}";
        byte[]? cachedExpensesBytes = await _distributedCache.GetAsync(cacheKey);

        if (cachedExpensesBytes != null)
        {
            var cachedExpenses = JsonSerializer.Deserialize<List<Expense>>(Encoding.UTF8.GetString(cachedExpensesBytes));
            return Ok(cachedExpenses);
        }

        IEnumerable<Expense> expenses = _expenseService.GetAllExpenses(username);

        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        var expensesJson = JsonSerializer.Serialize(expenses.ToList());
        var expensesBytes = Encoding.UTF8.GetBytes(expensesJson);
        await _distributedCache.SetAsync(cacheKey, expensesBytes, options);

        return Ok(expenses.ToList());
    }


    [HttpPost("Create")]
    public ActionResult<Expense> CreateExpense([FromBody] Expense expense)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        expense.UserName = username;

        _expenseService.CreateExpense(expense);

        string cacheKey = $"expenses:{username}";
        _distributedCache.RemoveAsync(cacheKey);
        
        return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
    }

    [HttpGet("{id}")]
    public ActionResult<Expense> GetExpenseById(int id)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        var expense = _expenseService.GetExpenseById(id);
        if (expense == null || expense.UserName != username)
        {
            return NotFound();
        }
        return Ok(expense);
    }

    [HttpPut("Update")]
    public IActionResult UpdateExpense([FromBody] Expense expense)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        var existingExpense = _expenseService.GetExpenseById(expense.Id);
        if (existingExpense == null || existingExpense.UserName != username)
        {
            return NotFound();
        }

        existingExpense.Value = expense.Value;
        existingExpense.Description = expense.Description;

        _expenseService.UpdateExpense(existingExpense);

        string cacheKey = $"expenses:{username}";
        _distributedCache.RemoveAsync(cacheKey);
        
        return Ok(existingExpense);
    }

    [HttpDelete("Delete/{id}")]
    public IActionResult DeleteExpense(int id)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        var expenseToDelete = _expenseService.GetExpenseById(id);
        if (expenseToDelete == null || expenseToDelete.UserName != username)
        {
            return NotFound();
        }

        _expenseService.DeleteExpense(id);

        string cacheKey = $"expenses:{username}";
        _distributedCache.RemoveAsync(cacheKey);
        
        return NoContent();
    }
}