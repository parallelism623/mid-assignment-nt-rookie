using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

using MIDASM.Persistence.Services;
using MIDASM.Domain.Entities;
using MIDASM.Contract.SharedKernel;
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;

namespace MIDASM.Persistence.Services.Tests
{
    public class AuditLoggerTests : IDisposable
    {
        private readonly AuditLogDbContext _context;
        private readonly AuditLogger _logger;
        private readonly DefaultHttpContext _httpContext;
        private readonly Mock<IExecutionContext> _execContext;

        public AuditLoggerTests()
        {
            var options = new DbContextOptionsBuilder<AuditLogDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AuditLogDbContext(options);

            // Setup HttpContext with route data
            _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["User-Agent"] = "TestAgent";
            _httpContext.Request.Method = "POST";
            _httpContext.Request.Path = "/api/test/action";
            var routeData = new RouteData();
            routeData.Values["controller"] = "Test";
            routeData.Values["action"] = "Action";
            _httpContext.Features.Set<IRoutingFeature>(new RoutingFeature { RouteData = routeData });

            var httpAccessor = new Mock<IHttpContextAccessor>();
            httpAccessor.Setup(a => a.HttpContext).Returns(_httpContext);

            _execContext = new Mock<IExecutionContext>();
            _execContext.Setup(e => e.GetUserId()).Returns(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
            _execContext.Setup(e => e.GetUserName()).Returns("testuser");

            _logger = new AuditLogger(_context, _execContext.Object, httpAccessor.Object);
        }

        [Fact]
        public async Task LogAsync_ShouldInsertAuditLogAndData()
        {
            // Arrange
            var changedProps = new Dictionary<string, (string?, string?)>
            {
                ["PropA"] = ("old", "new"),
                ["PropB"] = (null, "val")
            };

            // Act
            await _logger.LogAsync(
                entityId: "E1",
                entityName: "Entity",
                desciption: "Desc",
                changedProperties: changedProps
            );

            // Assert
            var logs = _context.AuditLogs.ToList();
            logs.Should().HaveCount(1);
            var log = logs.First();
            log.EntityId.Should().Be("E1");
            log.EntityName.Should().Be("Entity");
            log.Description.Should().Be("Desc");
            log.Username.Should().Be("testuser");
            log.BrowserInfo.Should().Be("TestAgent");
            log.HttpMethod.Should().Be("POST");
            log.ServiceName.Should().Be("Test");
            log.MethodName.Should().Be("Action");

            var datas = _context.AuditLogDatas.Where(a => a.AuditLogId == log.Id).ToList();
            datas.Should().HaveCount(2);
            datas.Select(d => d.PropertyName).Should().BeEquivalentTo("PropA", "PropB");
        }

        [Fact]
        public async Task GetActivitiesAsync_ShouldReturnPagedResults()
        {
            // Arrange: seed 3 logs
            for (int i = 1; i <= 3; i++)
            {
                var log = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityId = $"E{i}",
                    EntityName = "Ent",
                    TimeStamp = DateTime.UtcNow.AddDays(-i),
                    Description = "D",
                    UserId = Guid.NewGuid(),
                    Username = "u"
                };
                _context.Add(log);
                _context.Add(new AuditLogData { Id = Guid.NewGuid(), AuditLogId = log.Id, PropertyName = "P", OriginalValue = "o", NewValue = "n" });
            }
            await _context.SaveChangesAsync();

            var queryParams = new AuditLogQueryParameters { Skip = 1, Take = 1 };

            // Act
            var result = await _logger.GetActivitiesAsync(queryParams);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Items.Should().HaveCount(1);
            result.Data.TotalCount.Should().Be(3);
            var item = result.Data.Items.First();
            item.AuditLogDatas.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetUserActivitiesAsync_ShouldFilterByUserIdAndPage()
        {
            // Arrange: two users
            var u1 = Guid.NewGuid();
            var u2 = Guid.NewGuid();
            var today = DateTime.UtcNow;
            var logs = new List<AuditLog>
            {
                new AuditLog { Id = Guid.NewGuid(), UserId = u1, TimeStamp = today },
                new AuditLog { Id = Guid.NewGuid(), UserId = u2, TimeStamp = today }
            };
            _context.AddRange(logs);
            await _context.SaveChangesAsync();

            var qp = new UserAuditLogQueryParameters { PageIndex = 1, PageSize = 5 };

            // Act
            var result = await _logger.GetUserActivitiesAsync(u1, qp);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Items.Should().OnlyContain(x => x.Id == logs[0].Id);
        }

        //[Fact]
        //public async Task GetUserActivitiesReportAsync_ShouldCountDistinctDays()
        //{
        //    // Arrange: two logs on same day and one on another day for u1
        //    var u1 = Guid.NewGuid();
        //    var logs = new List<AuditLog>
        //    {
        //        new AuditLog { Id = Guid.NewGuid(), UserId = u1, TimeStamp = DateTime.UtcNow },
        //        new AuditLog { Id = Guid.NewGuid(), UserId = u1, TimeStamp = DateTime.UtcNow },
        //        new AuditLog { Id = Guid.NewGuid(), UserId = u1, TimeStamp = DateTime.UtcNow.AddDays(-1) }
        //    };
        //    _context.AddRange(logs);
        //    await _context.SaveChangesAsync();
        //    var qp = new UserActivitiesQueryParameters
        //    {
        //        FromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
        //        ToDate = DateOnly.FromDateTime(DateTime.UtcNow)
        //    };

        //    // Act
        //    var result = await _logger.GetUserActivitiesReportAsync(qp);

        //    // Assert
        //    result.Should().HaveCount(1);
        //    result[0].UserId.Should().Be(u1);
        //    result[0].ActiveDays.Should().Be(2);
        //}

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
