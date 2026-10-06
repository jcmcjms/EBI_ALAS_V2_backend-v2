using Alas.Api.Features.Branches;
using Alas.Api.Features.Branches.Domain;
using Alas.Api.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Tests;

public class BranchServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly BranchRepository _repository;
    private readonly BranchService _service;

    public BranchServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new BranchRepository(_context);
        _service = new BranchService(_repository, TimeProvider.System);
    }

    public void Dispose() => _context.Dispose();

    private async Task SeedBranchesAsync()
    {
        _context.Branches.AddRange(
            new Branch { Code = "HO", Name = "Head Office", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
            new Branch { Code = "BR001", Name = "Branch 001", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
            new Branch { Code = "BR002", Name = "Branch 002", IsActive = false, CreatedAt = DateTimeOffset.UtcNow }
        );
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetBranchesAsync_ShouldReturnPagedResults()
    {
        await SeedBranchesAsync();
        var parameters = new BranchQueryParameters(IsActive: null, PageNumber: 1, PageSize: 10);

        var result = await _service.GetBranchesAsync(parameters);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetBranchesAsync_WithIsActiveFilter_ShouldFilter()
    {
        await SeedBranchesAsync();
        var parameters = new BranchQueryParameters(IsActive: true, PageNumber: 1, PageSize: 10);

        var result = await _service.GetBranchesAsync(parameters);

        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(b => b.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task GetAllBranchesAsync_ShouldReturnAll()
    {
        await SeedBranchesAsync();

        var result = await _service.GetAllBranchesAsync();

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllBranchesAsync_WithIsActiveFilter_ShouldFilter()
    {
        await SeedBranchesAsync();

        var result = await _service.GetAllBranchesAsync(isActive: true);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturn()
    {
        await SeedBranchesAsync();

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Code.Should().Be("HO");
        result.Name.Should().Be("Head Office");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        var result = await _service.GetByIdAsync(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeAsync_WhenExists_ShouldReturn()
    {
        await SeedBranchesAsync();

        var result = await _service.GetByCodeAsync("BR001");

        result.Should().NotBeNull();
        result!.Name.Should().Be("Branch 001");
    }

    [Fact]
    public async Task GetByCodeAsync_WhenNotExists_ShouldReturnNull()
    {
        var result = await _service.GetByCodeAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBranch()
    {
        var request = new CreateBranchRequest("BR003", "Branch 003", "AREA3");

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Code.Should().Be("BR003");
        result.Name.Should().Be("Branch 003");
        result.AreaCode.Should().Be("AREA3");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ShouldThrow()
    {
        await SeedBranchesAsync();
        var request = new CreateBranchRequest("HO", "Duplicate Branch");

        var act = () => _service.CreateAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Branch code already exists");
    }

    [Fact]
    public async Task CreateAsync_ShouldNormalizeCodeToUpperCase()
    {
        var request = new CreateBranchRequest("br004", "Branch 004");

        var result = await _service.CreateAsync(request);

        result.Code.Should().Be("BR004");
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_ShouldUpdate()
    {
        await SeedBranchesAsync();
        var request = new UpdateBranchRequest("Updated Branch Name", false, "NEWAREA");

        var result = await _service.UpdateAsync(1, request);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Branch Name");
        result.IsActive.Should().BeFalse();
        result.AreaCode.Should().Be("NEWAREA");
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
    {
        var request = new UpdateBranchRequest("Updated", true);

        var result = await _service.UpdateAsync(999, request);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldReturnTrue()
    {
        await SeedBranchesAsync();

        var result = await _service.DeleteAsync(1);

        result.Should().BeTrue();
        var deleted = await _service.GetByIdAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ShouldReturnFalse()
    {
        var result = await _service.DeleteAsync(999);
        result.Should().BeFalse();
    }
}