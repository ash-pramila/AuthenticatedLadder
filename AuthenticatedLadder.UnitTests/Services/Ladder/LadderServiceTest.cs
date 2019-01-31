﻿using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Services.Ladder;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Services.Ladder
{
    public class LadderServiceTest
    {
        private Mock<ILadderRepository> _repository;

        public static IEnumerable<object[]> GetEntryForUserTestData => new List<object[]>
        {
            new object[]{ "", "ok", "ok"},
            new object[]{ "ok", "", "ok"},
            new object[]{ "ok", "ok", ""},
            new object[]{ null, "ok", "ok"},
            new object[]{ "ok", null, "ok"},
            new object[]{ "ok", "ok", null},
        };

        public LadderServiceTest()
        {
            _repository = new Mock<ILadderRepository>();
        }

        [Theory]
        [MemberData(nameof(GetEntryForUserTestData))]
        public void GetEntryForUser_ReturnsNullWhenInputIsNotValid(string ladderId, string platform, string username)
        {

            var service = new LadderService(_repository.Object);

            service
                .GetEntryForUser(ladderId, platform, username)
                .Should()
                .BeNull();
        }

        [Fact]
        public void GetEntryForUser_ReturnsNullWhenInputIsOkButThereAreNoEntriesCorrespondingToThatKey()
        {
            var myLadderId = "My Ladder Id";
            var myPlatform = "PC";
            var myUsername = "My Username";
            _repository
                .Setup(r => r.GetEntryForUser(myLadderId, myPlatform, myUsername))
                .Returns<LadderEntry>(null);

            var service = new LadderService(_repository.Object);

            service
                .GetEntryForUser(myLadderId, myPlatform, myUsername)
                .Should()
                .BeNull();

        }

        [Fact]
        public void GetEntryForUser_ReturnsMyEntryWhenMyKeyMatchesOneEntry()
        {
            var myLadderId = "My Ladder Id";
            var myPlatform = "PC";
            var myUsername = "My Username";
            var myEntry = new LadderEntry
            {
                LadderId = myLadderId,
                Platform = myPlatform,
                Username = myUsername
            };
            _repository
                .Setup(r => r.GetEntryForUser(myLadderId, myPlatform, myUsername))
                .Returns(() => myEntry);

            var service = new LadderService(_repository.Object);
            var result = service.GetEntryForUser(myLadderId, myPlatform, myUsername);

            result
                .Should()
                .BeEquivalentTo(myEntry);
        }

        [Fact]
        public void GetEntryForUser_LogsErrorAndThrowsWhenRepositoryThrows()
        {
            var myLadderId = "My Ladder Id";
            var myPlatform = "PC";
            var myUsername = "My Username";

            var exceptionMessage = "An error occurred because blablabalabla";
            _repository
                .Setup(r => r.GetEntryForUser(myLadderId, myPlatform, myUsername))
                .Throws(new Exception(exceptionMessage));

            var service = new LadderService(_repository.Object);

            service
                .Invoking(s => s.GetEntryForUser(myLadderId, myPlatform, myUsername))
                .Should().Throw<Exception>()
                .WithMessage(exceptionMessage);

            //TODO Check if it logs

        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetTopEntries_ReturnsEmptyListIfLadderIdIsNullOrEmpty(string ladderId)
        {

            var service = new LadderService(_repository.Object);

            service
                .GetTopEntries(ladderId)
                .Should()
                .NotBeNull()
                .And.BeEmpty();
        }

        [Fact]
        public void GetTopEntries_ReturnsEmptyListIfLadderIdIsOkButServerHasNoEntryForThatLadder()
        {
            var myLadderId = "My Ladder Id";

            _repository
                .Setup(r => r.GetTopEntries(myLadderId))
                .Returns(new List<LadderEntry>());

            var service = new LadderService(_repository.Object);

            var result = service.GetTopEntries(myLadderId);

            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GetTopEntries_ReturnTopEntriesOfGivenLadder()
        {
            var myLadderId = "My Ladder Id";
            var myEntry = new LadderEntry
            {
                LadderId = myLadderId,
                Platform = "PC",
                Score = 123,
                Username = "My Username"
            };
            var myLadderList = new List<LadderEntry>
            {
                new LadderEntry(),
                new LadderEntry(),
                new LadderEntry(),
                new LadderEntry(),
            };
            myLadderList.Add(myEntry);

            _repository
                .Setup(r => r.GetTopEntries(myLadderId))
                .Returns(myLadderList);

            var service = new LadderService(_repository.Object);

            var result = service.GetTopEntries(myLadderId);

            result
                .Should()
                .NotBeEmpty()
                .And.HaveCount(5)
                .And.ContainEquivalentOf(myEntry);
        }

        [Fact]
        public void GetTopEntries_LogsErrorAndThrowsWhenRepositoryThrows()
        {
            var myLadderId = "My Ladder Id";

            var exceptionMessage = "An error occurred because blablabalabla";
            _repository
                .Setup(r => r.GetTopEntries(myLadderId))
                .Throws(new Exception(exceptionMessage));

            var service = new LadderService(_repository.Object);

            service
                .Invoking(s => s.GetTopEntries(myLadderId))
                .Should().Throw<Exception>()
                .WithMessage(exceptionMessage);

            //TODO Check if it logs
        }

        [Fact]
        public void Upsert_ReturnsNullWhenNullIsPassed()
        {
            var service = new LadderService(_repository.Object);

            service
                .Upsert(null)
                .Should()
                .BeNull();
        }

        [Fact]
        public void Upsert_ReturnsTheUpdatedOrInsertedEntryIfEverythingIsOK()
        {
            var entry = new LadderEntry
            {
                LadderId = "My Ladder",
                Platform = "PC",
                Username = "My Username"
            };

            _repository
                .Setup(r => r.Upsert(entry))
                .Returns(() => entry);

            var service = new LadderService(_repository.Object);

            service
                .Upsert(entry)
                .Should()
                .BeEquivalentTo(entry);
        }

        [Fact]
        public void Upsert_LogsErrorAndThrowsWhenRepositoryThrows()
        {
            var entry = new LadderEntry
            {
                LadderId = "My Ladder",
                Platform = "PC",
                Username = "My Username"
            };


            var exceptionMessage = "An error occurred because blablabalabla";
            _repository
                .Setup(r => r.Upsert(entry))
                .Throws(new Exception(exceptionMessage));

            var service = new LadderService(_repository.Object);

            service
                .Invoking(s => s.Upsert(entry))
                .Should().Throw<Exception>()
                .WithMessage(exceptionMessage);

            //TODO Check if it logs
        }
    }
}
