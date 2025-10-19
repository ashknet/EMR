using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using PatientService.API.Controllers;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Services;

namespace PatientService.Tests.Controllers;

public class PatientsControllerTests
{
    private readonly Mock<IEncryptionService> _encryptionServiceMock;
    private readonly Mock<ILogger<PatientsController>> _loggerMock;
    private readonly PatientDbContext _context;
    private readonly PatientsController _controller;

    public PatientsControllerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<PatientDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientDbContext(options);
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _loggerMock = new Mock<ILogger<PatientsController>>();
        var mapperMock = new Mock<AutoMapper.IMapper>();

        _controller = new PatientsController(_context, _encryptionServiceMock.Object, _loggerMock.Object, mapperMock.Object);
    }

    [Fact]
    public async Task GetPatient_ExistingPatient_ReturnsPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            PatientId = patientId,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1980, 5, 15),
            Gender = "Male",
            Email = "john.doe@test.com",
            PhoneNumber = "555-0101",
            AddressLine1 = "123 Test St",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            IsActive = true,
            IsDeleted = false
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        okResult.Should().NotBeNull();
        
        var patientDto = okResult!.Value as PatientDto;
        patientDto.Should().NotBeNull();
        patientDto!.PatientId.Should().Be(patientId);
        patientDto.FirstName.Should().Be("John");
        patientDto.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetPatient_NonExistentPatient_ReturnsNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreatePatient_ValidData_ReturnsCreatedPatient()
    {
        // Arrange
        var request = new CreatePatientRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 3, 20),
            Gender = "Female",
            Email = "jane.smith@test.com",
            PhoneNumber = "555-0202",
            AddressLine1 = "456 Test Ave",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345"
        };

        _encryptionServiceMock.Setup(x => x.EncryptAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("encrypted_ssn");
        _encryptionServiceMock.Setup(x => x.ComputeHash(It.IsAny<string>()))
            .Returns("hashed_ssn");

        // Act
        var result = await _controller.CreatePatient(request);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Result as Microsoft.AspNetCore.Mvc.CreatedAtActionResult;
        createdResult.Should().NotBeNull();

        var patientDto = createdResult!.Value as PatientDto;
        patientDto.Should().NotBeNull();
        patientDto!.FirstName.Should().Be("Jane");
        patientDto.LastName.Should().Be("Smith");

        // Verify patient was added to database
        var dbPatient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == "jane.smith@test.com");
        dbPatient.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePatient_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var existingPatient = new Patient
        {
            PatientId = Guid.NewGuid(),
            FirstName = "Existing",
            LastName = "Patient",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Email = "duplicate@test.com",
            PhoneNumber = "555-0303",
            AddressLine1 = "789 Test Blvd",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            IsActive = true
        };

        _context.Patients.Add(existingPatient);
        await _context.SaveChangesAsync();

        var request = new CreatePatientRequest
        {
            FirstName = "New",
            LastName = "Patient",
            DateOfBirth = new DateTime(1995, 6, 15),
            Gender = "Female",
            Email = "duplicate@test.com",
            PhoneNumber = "555-0404",
            AddressLine1 = "101 Test Ln",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345"
        };

        // Act
        var result = await _controller.CreatePatient(request);

        // Assert
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdatePatient_ValidData_UpdatesPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            PatientId = patientId,
            FirstName = "Original",
            LastName = "Name",
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = "Male",
            Email = "original@test.com",
            PhoneNumber = "555-0505",
            AddressLine1 = "Original Address",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            IsActive = true
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdatePatientRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "original@test.com",
            PhoneNumber = "555-0606",
            AddressLine1 = "Updated Address",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345"
        };

        // Act
        var result = await _controller.UpdatePatient(patientId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();

        var updatedPatient = await _context.Patients.FindAsync(patientId);
        updatedPatient.Should().NotBeNull();
        updatedPatient!.FirstName.Should().Be("Updated");
        updatedPatient.PhoneNumber.Should().Be("555-0606");
    }

    [Fact]
    public async Task DeletePatient_ExistingPatient_SoftDeletesPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            PatientId = patientId,
            FirstName = "To",
            LastName = "Delete",
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = "Male",
            Email = "delete@test.com",
            PhoneNumber = "555-0707",
            AddressLine1 = "Delete Address",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            IsActive = true,
            IsDeleted = false
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeletePatient(patientId);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Mvc.NoContentResult>();

        var deletedPatient = await _context.Patients.FindAsync(patientId);
        deletedPatient.Should().NotBeNull();
        deletedPatient!.IsDeleted.Should().BeTrue();
        deletedPatient.IsActive.Should().BeFalse();
    }
}

