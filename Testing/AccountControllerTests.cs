using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using SecureAuthApp.Controllers;
using SecureAuthApp.Services;
using System;
using System.Linq;
using Moq;

namespace SecureAuthApp.Tests
{
    [TestFixture]
    public class AccountControllerTests
    {
        private AccountController _controller;
        private Mock<IJwtService> _jwtServiceMock;
        
        [SetUp]
        public void Setup()
        {
            _jwtServiceMock = new Mock<IJwtService>();

            _jwtServiceMock
            .Setup(s => s.GenerateToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("mocked-jwt-token");

            _controller = new AccountController(_jwtServiceMock.Object);
        }

        [Test]
        public void Register_ValidNewUser_ReturnsOkWithToken()
        {
            var model = new UserModel
            {
                Username = "newuser",
                Password = "SafePass123"
            };

            var result = _controller.Register(model) as OkObjectResult;

            Assert.IsNotNull(result);
            StringAssert.StartsWith("Register Success:Token:", result.Value.ToString());
        }

        [Test]
        public void Register_ExistingUser_ReturnsUnauthorized()
        {
            var existing = new UserModel
            {
                Username = "kirima",
                Password = "anything"
            };

            var result = _controller.Register(existing) as UnauthorizedObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Username already exists.", result.Value);
        }

        [Test]
        public void Login_ValidCredentials_ReturnsOkWithToken()
        {
            var model = new UserModel
            {
                Username = "kirima",
                Password = "securepassword123"
            };

            var result = _controller.Login(model) as OkObjectResult;

            Assert.IsNotNull(result);
            StringAssert.StartsWith("Login Success:Token:", result.Value.ToString());
        }

        [Test]
        public void Login_InvalidPassword_ReturnsUnauthorized()
        {
            var model = new UserModel
            {
                Username = "kirima",
                Password = "wrongpass"
            };

            var result = _controller.Login(model) as UnauthorizedObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid login attempt.", result.Value);
        }

        [Test]
        public void Login_UnsafeInput_ReturnsUnauthorized()
        {
            var model = new UserModel
            {
                Username = "<script>",
                Password = "123"
            };

            var result = _controller.Login(model) as UnauthorizedObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid input.", result.Value);
        }
    }
}
