

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.UseCases.Interfaces;
using MIDASM.Presentation.Controllers;

namespace MIDASS.Presentation.Controllers;

[Route("api/[controller]")]
public class UsersController : ApiBaseController
{
    private readonly IUserServices _userServices;
    private readonly IAuditLogger _auditLogger;
    public UsersController(IUserServices userServices,
            IAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
        _userServices = userServices;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    [Route("book-borrowing")]
    public async Task<IActionResult> CreateBookBorrowingRequestAsync([FromBody] BookBorrowingRequestCreate bookBorrowingRequest)
    {
        var result = await _userServices.CreateBookBorrowingRequestAsync(bookBorrowingRequest);

        return ProcessResult(result);
    }

    [HttpGet]
    [Route("{id:guid}/book-borrowing-requests")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetBookBorrowingRequestByIdAsync(Guid id, [FromQuery] UserBookBorrowingRequestQueryParameters queryParameters)
    {
        var result = await _userServices.GetBookBorrowingRequestByIdAsync(id, queryParameters);

        return ProcessResult(result);
    }
    [HttpGet]
    [Route("{id:guid}/book-borrowed")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetBookBorrowedRequestDetailByIdAsync(Guid id, [FromQuery] QueryParameters queryParameters)
    {
        var result = await _userServices.GetBookBorrowedRequestDetailByIdAsync(id, queryParameters);

        return ProcessResult(result);
    }
    [HttpPut]
    [Route("{id:guid}/book-borrowed/due-date-extend")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ExtendDueDateAsync(Guid id, [FromBody] DueDatedExtendRequest dueDatedExtendRequest)
    {
        var result = await _userServices.ExtendDueDateBookBorrowed(dueDatedExtendRequest);

        return ProcessResult(result);
    }
    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _userServices.GetByIdAsync(id);

        return ProcessResult(result);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAsync([FromQuery] UserQueryParameters queryParameters)
    {
        var result = await _userServices.GetAsync(queryParameters);
        return ProcessResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreateRequest createRequest)
    {
        var result = await _userServices.CreateAsync(createRequest);
        return ProcessResult(result);
    }


    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateRequest updateRequest)
    {
        var result = await _userServices.UpdateAsync(updateRequest);
        return ProcessResult(result);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _userServices.DeleteAsync(id);
        return ProcessResult(result);
    }

    [HttpGet]
    [Route("{id:guid}/audit-logs")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAuditLogsAsync(Guid id, [FromQuery] UserAuditLogQueryParameters queryParameters)
    {
        var result = await _auditLogger.GetUserActivitiesAsync(id, queryParameters);
        return ProcessResult(result);
    }

    [HttpPut]
    [Route("{id:guid}/profile")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UpdateProfileAsync(Guid id, [FromBody] UserProfileUpdateRequest userProfileUpdateRequest)
    {
        var result = await _userServices.UpdateProfileAsync(id, userProfileUpdateRequest);
        return ProcessResult(result);
    }
}
