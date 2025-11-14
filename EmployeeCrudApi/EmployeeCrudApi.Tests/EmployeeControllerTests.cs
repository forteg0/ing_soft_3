using EmployeeCrudApi.Controllers;
using EmployeeCrudApi.Data;
using EmployeeCrudApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeCrudApi.Tests
{
    public class EmployeeControllerUnitTests
    {
        private ApplicationDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // ---------------------------------------------------------
        // GET ALL
        // ---------------------------------------------------------
        [Fact]
        public async Task GetAll_ReturnsAllEmployees()
        {
            var db = GetInMemoryDb();
            db.Employees.AddRange(
                new Employee { Id = 1, Name = "John" },
                new Employee { Id = 2, Name = "Jane" }
            );
            db.SaveChanges();

            var controller = new EmployeeController(db);

            var result = await controller.GetAll();
            var list = result;  // GetAll returns List<Employee>

            Assert.Equal(2, list.Count);
            Assert.Contains(list, e => e.Name == "John");
            Assert.Contains(list, e => e.Name == "Jane");
        }

        // ---------------------------------------------------------
        // GET BY ID
        // ---------------------------------------------------------
        [Fact]
        public async Task GetById_ReturnsEmployee_WhenExists()
        {
            var db = GetInMemoryDb();
            db.Employees.Add(new Employee { Id = 1, Name = "John" });
            db.SaveChanges();

            var controller = new EmployeeController(db);

            var result = await controller.GetById(1);
            var employee = result.Value;

            Assert.NotNull(employee);
            Assert.Equal(1, employee.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            var db = GetInMemoryDb();
            var controller = new EmployeeController(db);

            var result = await controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        [Fact]
        public async Task Create_AddsEmployee()
        {
            var db = GetInMemoryDb();
            var controller = new EmployeeController(db);

            var emp = new Employee { Id = 10, Name = "New" };

            var response = await controller.Create(emp);

            Assert.IsType<OkResult>(response);

            var saved = db.Employees.Find(10);
            Assert.NotNull(saved);
        }

        // ---------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------
        [Fact]
        public async Task Update_UpdatesEmployee_WhenExists()
        {
            var db = GetInMemoryDb();
            db.Employees.Add(new Employee { Id = 1, Name = "Old" });
            db.SaveChanges();

            var controller = new EmployeeController(db);

            var updated = new Employee { Id = 1, Name = "Updated" };

            var result = await controller.Update(updated);

            Assert.IsType<OkResult>(result);
            Assert.Equal("Updated", db.Employees.Find(1).Name);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            var db = GetInMemoryDb();
            var controller = new EmployeeController(db);

            var updated = new Employee { Id = 99, Name = "Invalid" };

            var result = await controller.Update(updated);

            Assert.IsType<NotFoundResult>(result);
        }

        // ---------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------
        [Fact]
        public async Task Delete_RemovesEmployee_WhenExists()
        {
            var db = GetInMemoryDb();
            db.Employees.Add(new Employee { Id = 1, Name = "John" });
            db.SaveChanges();

            var controller = new EmployeeController(db);

            var result = await controller.Delete(1);

            Assert.IsType<OkResult>(result);
            Assert.Null(db.Employees.Find(1));
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenDoesNotExist()
        {
            var db = GetInMemoryDb();
            var controller = new EmployeeController(db);

            var result = await controller.Delete(123);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
