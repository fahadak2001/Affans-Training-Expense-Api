using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseBackend.Models;
using ExpenseBackend.Services;

namespace ExpenseBackend.Controllers;

[ApiController]
[Route("api/expenses")]
[Produces("application/json")]
[Authorize]  //This attribute ensures that the controller requires authentication.
             //and if the JWT token that is passed is valid or expired or not.
             //matching the JWT token secret key, issuer and expiration time. from appsettings.json.
             //I didnt make a method for Authorization, because the JWT token is
             //already validated in the middleware. just by using this attribute.
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IAuthService _authService;

    public ExpenseController(IExpenseService expenseService, IAuthService authService)
    {
        _expenseService = expenseService;
        _authService = authService;
    }

    [HttpGet("List")]
    public ActionResult<IEnumerable<Expense>> GetExpenses()
    {
        var username = User.Identity?.Name; // Here we get the username from the JWT token.
                                            // In the JWT token I have added claims,
                                            // NameIdentifier as username.
                                            // Name as email and Role as role.
                                            // I am making the JWT token in user service in LoginAPI.
                                            // So i have to get this data of user to seperate expense data
                                            // for each user. and only send the data for this user to the frontend.

        if (_authService.Authenticate(username) )// Here I am only checking if the username is null or not.
                                                 // because if it is, then i cannot get the expenses. 
                                                 // Auth is still being done in the middleware by verifying JWT token.
                                                 // That is only created after Authentication in LoinAPI.
        {
            IEnumerable<Expense> expenses = _expenseService.GetAllExpenses(username);

            return Ok(expenses.ToList());
            
        }

        return Unauthorized();
    }

    [HttpPost("Create")]
    public ActionResult<Expense> CreateExpense([FromBody] Expense expense)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var username = User.Identity?.Name;
        if (_authService.Authenticate(username))
        {
            expense.UserName = username;

            _expenseService.CreateExpense(expense);

            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }
        return Unauthorized();
    }

    [HttpGet("{id}")]
    public ActionResult<Expense> GetExpenseById(int id)
    {
        var username = User.Identity?.Name;
        if (_authService.Authenticate(username))
        {
            var expense = _expenseService.GetExpenseById(id);
            if (expense == null || expense.UserName != username)
            {
                return NotFound();
            }
            return Ok(expense);
        }
        return Unauthorized();

    }

    [HttpPut("Update")]
    public IActionResult UpdateExpense([FromBody] Expense expense)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var username = User.Identity?.Name;
        if (_authService.Authenticate(username))
        {
            var existingExpense = _expenseService.GetExpenseById(expense.Id);
            if (existingExpense == null || existingExpense.UserName != username)
            {
                return NotFound();
            }

            existingExpense.Value = expense.Value;
            existingExpense.Description = expense.Description;

            _expenseService.UpdateExpense(existingExpense);

            return Ok(existingExpense);
            
        }
        return Unauthorized();

    }

    [HttpDelete("Delete/{id}")]
    public IActionResult DeleteExpense(int id)
    {
        var username = User.Identity?.Name;
        if (_authService.Authenticate(username))
        {
            _expenseService.DeleteExpense(id);

            return NoContent();
        }
        return Unauthorized();

    }
}