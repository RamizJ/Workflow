﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class GoalAttachmentsServiceTest
    {
        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var serviceProvider = ContextHelper.Initialize(_dbConnection, false);
            var dataContext = serviceProvider.GetRequiredService<DataContext>();
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            dataContext.Database.EnsureCreated();
            _testData = new TestData();
            _testData.Initialize(dataContext, userManager);

            _serviceProvider = ContextHelper.Initialize(_dbConnection, true);
            _dataContext = _serviceProvider.GetRequiredService<DataContext>();
            _service = _serviceProvider.GetRequiredService<IGoalAttachmentsService>();
            _currentUser = _testData.Users.First();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
            _dbConnection.Close();
        }


        [Test]
        public void GetAttachmentsWithoutUserTest()
        {
            //Arrange
            var goal = _dataContext.Goals.First();

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.GetAll(null, goal.Id));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        public async Task GetNotExistedGoalAttachments(int goalId)
        {
            //Arrange
            var attachments = await _service.GetAll(_currentUser, goalId);

            //Assert
            Assert.IsEmpty(attachments);
        }

        [Test]
        public async Task GetAttachmentsTest()
        {
            //Arrange
            var dataContext = ContextHelper.CreateContext(_dbConnection, false);

            var creationDate = DateTime.Now.ToUniversalTime();
            var fileData = Builder<FileData>.CreateNew()
                .With(f => f.Id = 0)
                .With(f => f.Data = new byte[] {1,2,3})
                .Build();
            var attachments = Builder<Attachment>.CreateListOfSize(5)
                .All()
                .With(a => a.Id = 0)
                .With(a => a.CreationDate = creationDate)
                .With(a => a.FileName = $"file{a.Id}")
                .With(a => a.FileType = $"ext{a.Id}")
                .With(a => a.FileSize = a.Id)
                .With(a => a.FileData = fileData)
                .Build();

            //await dataContext.FileData.AddAsync(fileData);

            var goal = dataContext.Goals.First();
            goal.Attachments.AddRange(attachments);

            await dataContext.SaveChangesAsync();

            //Act
            var resultAttachments = (await _service.GetAll(_currentUser, goal.Id))
                .OrderBy(a => a.Id)
                .ToArray();

            Assert.AreEqual(attachments.Count, resultAttachments.Length);
            for (var i = 0; i < resultAttachments.Length; i++)
            {
                var attachment = resultAttachments[i];
                Assert.AreEqual(attachments[i].Id, attachment.Id);
                Assert.AreEqual(attachments[i].CreationDate, attachment.CreationDate);
                Assert.AreEqual(attachments[i].FileName, attachment.FileName);
                Assert.AreEqual(attachments[i].FileSize, attachment.FileSize);
                Assert.AreEqual(attachments[i].FileDataId, attachment.FileDataId);
            }
        }

        [Test]
        public void AddFoNullCurrentUser()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await _service.Add(null, 1, new List<Attachment>()));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        public void AddFoNotExistedGoalTest(int goalId)
        {

            Assert.ThrowsAsync<HttpResponseException>(async () =>
                await _service.Add(_currentUser, goalId, new List<Attachment>()));
        }

        [Test]
        public async Task AddTest()
        {
            //Arrange
            //var goal = _dataContext.Goals.First();
            var creationDate = DateTime.Now.ToUniversalTime();
            var fileData = Builder<FileData>.CreateNew()
                .With(f => f.Id = 0)
                .With(f => f.Data = new byte[] { 1, 2, 3 })
                .Build();
            var attachments = Builder<Attachment>.CreateListOfSize(5)
                .All()
                .With(a => a.Id = 0)
                .With(a => a.CreationDate = creationDate)
                .With(a => a.FileName = $"file{a.Id}")
                .With(a => a.FileType = $"ext{a.Id}")
                .With(a => a.FileSize = a.Id)
                .With(a => a.FileDataId = fileData.Id)
                .With(a => a.FileData = fileData)
                .Build();

            //Act
            await _service.Add(_currentUser, 1, attachments);

            //Assert
            var resultAttachments = await _dataContext.Attachments.ToArrayAsync();

            Assert.AreEqual(attachments.Count, resultAttachments.Length);
            for (var i = 0; i < resultAttachments.Length; i++)
            {
                var attachment = resultAttachments[i];
                Assert.AreEqual(attachments[i].Id, attachment.Id);
                Assert.LessOrEqual(attachments[i].CreationDate, attachment.CreationDate);
                Assert.AreEqual(attachments[i].FileName, attachment.FileName);
                Assert.AreEqual(attachments[i].FileSize, attachment.FileSize);
                Assert.AreEqual(attachments[i].FileDataId, attachment.FileDataId);
            }
        }

        [Test]
        public void RemoveFoNullCurrentUser()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.Remove(null,new List<int>()));
        }

        [Test]
        public async Task RemoveTest()
        {
            //Arrange
            var dataContext = ContextHelper.CreateContext(_dbConnection, false);

            var creationDate = DateTime.Now.ToUniversalTime();
            var fileData = Builder<FileData>.CreateNew()
                .With(f => f.Id = 0)
                .With(f => f.Data = new byte[] { 1, 2, 3 })
                .Build();
            var attachments = Builder<Attachment>.CreateListOfSize(5)
                .All()
                .With(a => a.Id = 0)
                .With(a => a.CreationDate = creationDate)
                .With(a => a.FileName = $"file{a.Id}")
                .With(a => a.FileType = $"ext{a.Id}")
                .With(a => a.FileSize = a.Id)
                .With(a => a.FileData = fileData)
                .Build();

            var goal = dataContext.Goals.First();
            goal.Attachments.AddRange(attachments);

            await dataContext.SaveChangesAsync();

            //Act
            await _service.Remove(_currentUser, new []{1,2,3});
            var resultAttachments = await _dataContext.Attachments
                .OrderBy(a => a.Id)
                .ToArrayAsync();

            //Assert
            Assert.AreEqual(2, resultAttachments.Length);
            Assert.AreEqual(4, resultAttachments[0].Id);
            Assert.AreEqual(5, resultAttachments[1].Id);
        }



        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private IGoalAttachmentsService _service;
        private ApplicationUser _currentUser;
    }
}