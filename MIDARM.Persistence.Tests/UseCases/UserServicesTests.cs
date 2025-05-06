using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.Crypto;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using MockQueryable;
using Moq;

namespace MIDASM.Persistence.Services.Tests;

public class UserServicesTests
{
    private readonly Mock<IAuditLogger> _auditLogger;
    private readonly Mock<IBookRepository> _bookRepo;
    private readonly Mock<IBookBorrowingRequestDetailRepository> _borrowDetailRepo;
    private readonly Mock<IBookBorrowingRequestRepository> _borrowReqRepo;
    private readonly Mock<ICryptoServiceFactory> _cryptoFactory;
    private readonly Mock<IExecutionContext> _execContext;
    private readonly Mock<IRoleRepository> _roleRepo;
    private readonly UserServices _service;
    private readonly Mock<ITransactionManager> _transMgr;
    private readonly Mock<IUserRepository> _userRepo;

    public UserServicesTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _borrowReqRepo = new Mock<IBookBorrowingRequestRepository>();
        _execContext = new Mock<IExecutionContext>();
        _bookRepo = new Mock<IBookRepository>();
        _transMgr = new Mock<ITransactionManager>();
        _borrowDetailRepo = new Mock<IBookBorrowingRequestDetailRepository>();
        _roleRepo = new Mock<IRoleRepository>();
        _cryptoFactory = new Mock<ICryptoServiceFactory>();
        _auditLogger = new Mock<IAuditLogger>();

        _service = new UserServices(
            _userRepo.Object,
            _borrowReqRepo.Object,
            _execContext.Object,
            _bookRepo.Object,
            _transMgr.Object,
            _borrowDetailRepo.Object,
            _roleRepo.Object,
            _cryptoFactory.Object,
            _auditLogger.Object
        );
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_UserMismatch_ReturnsFailure()
    {
        // Arrange
        _execContext.Setup(e => e.GetUserId()).Returns(Guid.NewGuid());
        _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), "BookBorrowingRequests")).ReturnsAsync((User)null);
        BookBorrowingRequestCreate req = new() { RequesterId = Guid.NewGuid() };

        // Action
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.UserCannotBeInCurrentSession);
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_LimitZero_ReturnsFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(userId);
        User user = new()
        {
            Id = userId, BookBorrowingLimit = 0, BookBorrowingRequests = new List<BookBorrowingRequest>()
        };
        _userRepo.Setup(r => r.GetByIdAsync(userId, "BookBorrowingRequests")).ReturnsAsync(user);
        BookBorrowingRequestCreate req = new() { RequesterId = userId };

        // Action
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);


        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.UserReachBorrowingRequestLimit);
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_InvalidBooksCount_ReturnsFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(userId);
        User user = new()
        {
            Id = userId, BookBorrowingLimit = 1, BookBorrowingRequests = new List<BookBorrowingRequest>()
        };
        _userRepo.Setup(r => r.GetByIdAsync(userId, "BookBorrowingRequests")).ReturnsAsync(user);
        Guid bookId = Guid.NewGuid();
        BookBorrowingRequestCreate req = new()
        {
            RequesterId = userId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetailCreate> { new() { BookId = bookId } }
        };
        _bookRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Book>());


        // Action
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);


        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.UserBorrowingRequestBooksInvalid);
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_BookUnavailable_ReturnsFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(userId);
        User user = new()
        {
            Id = userId, BookBorrowingLimit = 1, BookBorrowingRequests = new List<BookBorrowingRequest>()
        };
        _userRepo.Setup(r => r.GetByIdAsync(userId, "BookBorrowingRequests")).ReturnsAsync(user);
        Guid bookId = Guid.NewGuid();
        BookBorrowingRequestCreate req = new()
        {
            RequesterId = userId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetailCreate> { new() { BookId = bookId } }
        };
        _bookRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new[] { new Book { Id = bookId, Available = 0 } }.ToList());


        // Action
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);


        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.SomeBooksInBooksBorrowingRequestUnavailable);
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_Success_CommitsAndLogs()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(userId);
        User user = new()
        {
            Id = userId, BookBorrowingLimit = 1, BookBorrowingRequests = new List<BookBorrowingRequest>()
        };
        _userRepo.Setup(r => r.GetByIdAsync(userId, "BookBorrowingRequests")).ReturnsAsync(user);
        Guid bookId = Guid.NewGuid();
        BookBorrowingRequestCreate req = new()
        {
            RequesterId = userId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetailCreate> { new() { BookId = bookId } }
        };
        _bookRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new[] { new Book { Id = bookId, Available = 1 } }.ToList());
        _transMgr.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _bookRepo.Setup(r => r.UpdateRange(It.IsAny<IEnumerable<Book>>()));
        _bookRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _userRepo.Setup(r => r.Update(It.IsAny<User>()));
        _userRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _transMgr.Setup(t => t.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _auditLogger.Setup(a => a.LogAsync(It.IsAny<string>(), nameof(BookBorrowingRequest), It.IsAny<string>(),
                It.IsAny<Dictionary<string, (string?, string?)>>()))
            .Returns(Task.CompletedTask);


        // Action
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);


        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(UserCommandMessages.BooksBorrowingRequestCreateSuccess);
    }


    [Fact]
    public async Task GetBookBorrowingRequestByIdAsync_UserNotFound_ReturnsFailure()
    {
        _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);
        Result<PaginationResult<BookBorrowingRequestResponse>> result =
            await _service.GetBookBorrowingRequestByIdAsync(Guid.NewGuid(),
                new UserBookBorrowingRequestQueryParameters());
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task GetBookBorrowingRequestByIdAsync_Success_ReturnsPaged()
    {
        User user = new() { Id = Guid.NewGuid() };
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        List<BookBorrowingRequest> list = new()
        {
            new BookBorrowingRequest { Id = Guid.NewGuid(), RequesterId = user.Id }
        };
        IQueryable<BookBorrowingRequest>? mockQ = list.AsQueryable().BuildMock();
        _borrowReqRepo.Setup(r => r.GetQueryable()).Returns(mockQ);

        Result<PaginationResult<BookBorrowingRequestResponse>> result =
            await _service.GetBookBorrowingRequestByIdAsync(user.Id,
                new UserBookBorrowingRequestQueryParameters { PageIndex = 1, PageSize = 5 });
        result.IsSuccess.Should().BeTrue();
        result.Data.Items.Should().HaveCount(1);
    }


    [Fact]
    public async Task GetByIdAsync_UserExists_ReturnsDetail()
    {
        Guid id = Guid.NewGuid();
        User usr = new() { Id = id, Username = "u" };
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(usr);
        Result<UserDetailResponse> result = await _service.GetByIdAsync(id);
        result.IsSuccess.Should().BeTrue();
        result.Data.Username.Should().Be("u");
    }


    [Fact]
    public async Task GetBookBorrowedRequestDetailByIdAsync_SessionMismatch_ReturnsFailure()
    {
        _execContext.Setup(e => e.GetUserId()).Returns(Guid.NewGuid());
        Result<PaginationResult<BookBorrowedRequestDetailResponse>> result =
            await _service.GetBookBorrowedRequestDetailByIdAsync(Guid.NewGuid(), new QueryParameters());
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.UserCannotBeInCurrentSession);
    }

    [Fact]
    public async Task GetBookBorrowedRequestDetailByIdAsync_Success_ReturnsPaged()
    {
        Guid id = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(id);
        List<BookBorrowingRequestDetail> list = new()
        {
            new BookBorrowingRequestDetail
            {
                Id = Guid.NewGuid(),
                BookBorrowingRequest =
                    new BookBorrowingRequest
                    {
                        RequesterId = id,
                        Status = (int)BookBorrowingStatus.Approved,
                        Requester = new User(),
                        Approver = new User()
                    },
                Book = new Book { Category = new Category() }
            }
        };
        IQueryable<BookBorrowingRequestDetail>? mockQ = list.AsQueryable().BuildMock();
        _borrowDetailRepo.Setup(r => r.GetQueryable()).Returns(mockQ);
        Result<PaginationResult<BookBorrowedRequestDetailResponse>> result =
            await _service.GetBookBorrowedRequestDetailByIdAsync(id,
                new QueryParameters { PageIndex = 1, PageSize = 5 });
        result.IsSuccess.Should().BeTrue();
        result.Data.Items.Should().HaveCount(1);
    }


    [Fact]
    public async Task ExtendDueDateBookBorrowed_NotFound_ReturnsFailure()
    {
        _borrowDetailRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), "BookBorrowingRequest", "Book"))
            .ReturnsAsync((BookBorrowingRequestDetail)null);
        Result<string> res = await _service.ExtendDueDateBookBorrowed(new DueDatedExtendRequest());
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.BookBorrowedNotExistsCanNotExtendDueDate);
    }

    [Fact]
    public async Task ExtendDueDateBookBorrowed_InvalidStatus_ReturnsFailure()
    {
        BookBorrowingRequestDetail detail = new()
        {
            Id = Guid.NewGuid(),
            BookBorrowingRequest = new BookBorrowingRequest { Status = (int)BookBorrowingStatus.Rejected }
        };
        _borrowDetailRepo.Setup(r => r.GetByIdAsync(detail.Id, "BookBorrowingRequest", "Book")).ReturnsAsync(detail);
        Result<string> res =
            await _service.ExtendDueDateBookBorrowed(new DueDatedExtendRequest { BookBorrowedDetailId = detail.Id });
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.BookBorrowRejectCanNotExtendDueDate);
    }

    [Fact]
    public async Task ExtendDueDateBookBorrowed_Valid_Succeeds()
    {
        DateOnly newDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));
        BookBorrowingRequestDetail detail = new()
        {
            Id = Guid.NewGuid(),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            ExtendDueDateTimes = 0,
            BookBorrowingRequest = new BookBorrowingRequest { Status = (int)BookBorrowingStatus.Approved }
        };
        _borrowDetailRepo.Setup(r => r.GetByIdAsync(detail.Id, "BookBorrowingRequest", "Book")).ReturnsAsync(detail);
        _borrowDetailRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        DueDatedExtendRequest req = new() { BookBorrowedDetailId = detail.Id, ExtendDueDate = newDate };
        Result<string> res = await _service.ExtendDueDateBookBorrowed(req);
        res.IsSuccess.Should().BeTrue();
    }

    // GetAsync (users)
    [Fact]
    public async Task GetAsync_ReturnsPagedUsers()
    {
        List<User> list = new() { new User { Id = Guid.NewGuid() } };
        IQueryable<User>? mockQ = list.AsQueryable().BuildMock();
        _userRepo.Setup(r => r.GetQueryable()).Returns(mockQ);
        Result<PaginationResult<UserDetailResponse>> res =
            await _service.GetAsync(new UserQueryParameters { PageIndex = 1, PageSize = 5 });
        res.IsSuccess.Should().BeTrue();
        res.Data.Items.Should().HaveCount(1);
    }

    // UpdateProfileAsync
    [Fact]
    public async Task UpdateProfileAsync_SessionMismatch_ReturnsFailure()
    {
        _execContext.Setup(e => e.GetUserId()).Returns(Guid.NewGuid());
        Result<string> res = await _service.UpdateProfileAsync(Guid.NewGuid(), new UserProfileUpdateRequest());
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.UserCannotBeInCurrentSession);
    }

    [Fact]
    public async Task UpdateProfileAsync_UserNotFound_ReturnsFailure()
    {
        Guid id = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(id);
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User)null);
        Result<string> res = await _service.UpdateProfileAsync(id, new UserProfileUpdateRequest());
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task UpdateProfileAsync_Success_ReturnsSuccess()
    {
        Guid id = Guid.NewGuid();
        User user = new() { Id = id };
        _execContext.Setup(e => e.GetUserId()).Returns(id);
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
        _userRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        Result<string> res = await _service.UpdateProfileAsync(id,
            new UserProfileUpdateRequest { FirstName = "F", LastName = "L", PhoneNumber = "P" });
        res.IsSuccess.Should().BeTrue();
        res.Data.Should().Be(UserCommandMessages.UserUpdateSuccessfully);
    }

    // UpdateAsync(User)
    [Fact]
    public async Task UpdateAsync_UserNotFound_ReturnsFailure()
    {
        UserUpdateRequest req = new() { Id = Guid.NewGuid() };
        _userRepo.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync((User)null);
        Result<string> res = await _service.UpdateAsync(req);
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task UpdateAsync_RoleNotFound_ReturnsFailure()
    {
        User user = new() { Id = Guid.NewGuid() };
        UserUpdateRequest req = new() { Id = user.Id, RoleId = Guid.NewGuid() };
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _roleRepo.Setup(r => r.GetByIdAsync(req.RoleId)).ReturnsAsync((Role)null);
        Result<string> res = await _service.UpdateAsync(req);
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.UserRoleNotFound);
    }

    [Fact]
    public async Task UpdateAsync_WithPassword_EncryptsAndUpdates()
    {
        User user = new() { Id = Guid.NewGuid() };
        UserUpdateRequest req = new() { Id = user.Id, RoleId = Guid.NewGuid(), Password = "p" };
        Mock<ICryptoService> crypto = new();
        crypto.Setup(c => c.Encrypt("p")).Returns("enc");
        _cryptoFactory.Setup(f => f.SetCryptoAlgorithm("RSA")).Returns(crypto.Object);
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _roleRepo.Setup(r => r.GetByIdAsync(req.RoleId)).ReturnsAsync(new Role { Id = req.RoleId });
        _userRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _auditLogger.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Dictionary<string, (string?, string?)>>())).Returns(Task.CompletedTask);

        Result<string> res = await _service.UpdateAsync(req);
        res.IsSuccess.Should().BeTrue();
        user.Password.Should().Be("enc");
    }

    // CreateAsync(User)
    [Fact]
    public async Task CreateAsync_UsernameExists_ReturnsFailure()
    {
        UserCreateRequest req = new() { Username = "u", Email = "e@e.com", RoleId = Guid.NewGuid() };
        _userRepo.Setup(r => r.GetByUsernameAsync("u")).ReturnsAsync(new User());
        Result<string> res = await _service.CreateAsync(req);
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.UsernameAlreadyExists);
    }

    [Fact]
    public async Task CreateAsync_EmailExists_ReturnsFailure()
    {
        UserCreateRequest req = new() { Username = "u", Email = "e@e.com", RoleId = Guid.NewGuid() };
        _userRepo.Setup(r => r.GetByUsernameAsync("u")).ReturnsAsync((User)null);
        _userRepo.Setup(r => r.GetByEmailAsync("e@e.com")).ReturnsAsync(new User());
        Result<string> res = await _service.CreateAsync(req);
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.EmailAlreadyExists);
    }

    [Fact]
    public async Task CreateAsync_RoleNotFound_ReturnsFailure()
    {
        UserCreateRequest req = new() { Username = "u", Email = "e@e.com", RoleId = Guid.NewGuid() };
        _userRepo.Setup(r => r.GetByUsernameAsync("u")).ReturnsAsync((User)null);
        _userRepo.Setup(r => r.GetByEmailAsync("e@e.com")).ReturnsAsync((User)null);
        _roleRepo.Setup(r => r.GetByIdAsync(req.RoleId)).ReturnsAsync((Role)null);
        Result<string> res = await _service.CreateAsync(req);
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.UserRoleNotFound);
    }

    [Fact]
    public async Task CreateAsync_Success_AddsUserAndLogs()
    {
        UserCreateRequest req = new() { Username = "u", Email = "e@e.com", RoleId = Guid.NewGuid(), Password = "p" };
        Mock<ICryptoService> crypto = new();
        crypto.Setup(c => c.Encrypt("p")).Returns("enc");
        _cryptoFactory.Setup(f => f.SetCryptoAlgorithm("RSA")).Returns(crypto.Object);
        _userRepo.Setup(r => r.GetByUsernameAsync("u")).ReturnsAsync((User)null);
        _userRepo.Setup(r => r.GetByEmailAsync("e@e.com")).ReturnsAsync((User)null);
        _roleRepo.Setup(r => r.GetByIdAsync(req.RoleId)).ReturnsAsync(new Role { Id = req.RoleId });
        _userRepo.Setup(r => r.Add(It.IsAny<User>()));
        _userRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _auditLogger.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Dictionary<string, (string?, string?)>>())).Returns(Task.CompletedTask);

        Result<string> res = await _service.CreateAsync(req);
        res.IsSuccess.Should().BeTrue();
        res.Data.Should().Be(UserCommandMessages.UserCreateSuccessfully);
    }

    // DeleteAsync(User)
    [Fact]
    public async Task DeleteAsync_UserNotFound_ReturnsFailure()
    {
        Guid id = Guid.NewGuid();
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User)null);
        Result<string> res = await _service.DeleteAsync(id);
        res.IsSuccess.Should().BeFalse();
        res.Errors.Should().Contain(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task DeleteAsync_Success_DeletesAndLogs()
    {
        User user = new() { Id = Guid.NewGuid() };
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _userRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _auditLogger.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Dictionary<string, (string?, string?)>>())).Returns(Task.CompletedTask);

        Result<string> res = await _service.DeleteAsync(user.Id);
        res.IsSuccess.Should().BeTrue();
        res.Data.Should().Be(UserCommandMessages.UserDeleteSuccessfully);
        user.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_DbConflict_RetriesAndSucceeds()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(userId);
        User user = new()
        {
            Id = userId, BookBorrowingLimit = 1, BookBorrowingRequests = new List<BookBorrowingRequest>()
        };
        _userRepo.Setup(r => r.GetByIdAsync(userId, "BookBorrowingRequests")).ReturnsAsync(user);
        Guid bookId = Guid.NewGuid();
        BookBorrowingRequestCreate req = new()
        {
            RequesterId = userId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetailCreate> { new() { BookId = bookId } }
        };
        Book book = new() { Id = bookId, Available = 2 };
        Book bookInDb = new() { Id = bookId, Available = 1 };
        List<Book> books = new() { book };
        _bookRepo.SetupSequence(r => r.GetByIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(books)
            .ReturnsAsync(new List<Book> { bookInDb });

        _bookRepo.SetupSequence(r => r.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateConcurrencyException("conflict"))
            .Returns(Task.CompletedTask);
        _transMgr.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _bookRepo.Setup(r => r.UpdateRange(It.IsAny<IEnumerable<Book>>()));
        _userRepo.Setup(r => r.Update(It.IsAny<User>()));
        _userRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _transMgr.Setup(t => t.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _auditLogger.Setup(a => a.LogAsync(It.IsAny<string>(), nameof(BookBorrowingRequest), It.IsAny<string>(),
                It.IsAny<Dictionary<string, (string?, string?)>>()))
            .Returns(Task.CompletedTask);

        // Act
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_DbConflictRollback_ReturnsErrorBookInAvailable()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(userId);
        User user = new()
        {
            Id = userId, BookBorrowingLimit = 1, BookBorrowingRequests = new List<BookBorrowingRequest>()
        };
        _userRepo.Setup(r => r.GetByIdAsync(userId, "BookBorrowingRequests")).ReturnsAsync(user);
        Guid bookId = Guid.NewGuid();
        BookBorrowingRequestCreate req = new()
        {
            RequesterId = userId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetailCreate> { new() { BookId = bookId } }
        };
        Book book = new() { Id = bookId, Available = 1 };
        Book bookInDb = new() { Id = bookId, Available = 0 };
        List<Book> books = new() { book };
        _bookRepo.SetupSequence(r => r.GetByIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(books)
            .ReturnsAsync(new List<Book> { bookInDb });

        _bookRepo.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateConcurrencyException("conflict"));
        _transMgr.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _transMgr.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);
        _transMgr.Setup(t => t.DisposeTransaction());

        // Act
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.SomeBooksInBooksBorrowingRequestUnavailable);
    }

    [Fact]
    public async Task CreateBookBorrowingRequestAsync_DbConflictRollbackAndRetryFailure_ReturnsError()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _execContext.Setup(e => e.GetUserId()).Returns(userId);
        User user = new()
        {
            Id = userId, BookBorrowingLimit = 1, BookBorrowingRequests = new List<BookBorrowingRequest>()
        };
        _userRepo.Setup(r => r.GetByIdAsync(userId, "BookBorrowingRequests")).ReturnsAsync(user);
        Guid bookId = Guid.NewGuid();
        BookBorrowingRequestCreate req = new()
        {
            RequesterId = userId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetailCreate> { new() { BookId = bookId } }
        };
        Book book = new() { Id = bookId, Available = 2 };
        List<Book> books = new() { book };
        _bookRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(books);
        _bookRepo.Setup(r => r.SaveChangesAsync()).Throws(new DbUpdateConcurrencyException("conflict"));
        _transMgr.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _transMgr.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);
        _transMgr.Setup(t => t.DisposeTransaction());

        // Act
        Result<string> result = await _service.CreateBookBorrowingRequestAsync(req);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.ErrorOccurWhenCreateBookBorrowingRequest);
    }
}