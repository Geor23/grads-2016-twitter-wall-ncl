﻿using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterWall.Context;
using TwitterWall.Models;
using TwitterWall.Repository;
using TwitterWall.Twitter;
using Xunit;

namespace TwitterWall.Test
{
    public class TwitterStreamTest : IDisposable
    {
        TwitterStream stream;

        public TwitterStreamTest()
        {
            stream = TwitterStream.Instance();

            // Inject DB
            var subscriptions = new List<Subscription>()
            {
            };
            var data = subscriptions.AsQueryable();
            var urlMockSet = setUpAsQueriable(data);
            urlMockSet.Setup(d => d.Add(It.IsAny<Subscription>())).Callback<Subscription>((r) => subscriptions.Add(r));

            var mockContext = new Mock<TweetContext>();
            mockContext.Setup(c => c.Subscriptions).Returns(urlMockSet.Object);

            // Arrange
            SubscriptionDBRepository repo = new SubscriptionDBRepository(mockContext.Object);

            stream._subRepo = repo;
        }

        private Mock<DbSet<Subscription>> setUpAsQueriable(IQueryable<Subscription> data)
        {
            var queriable = new Mock<DbSet<Subscription>>();
            queriable.As<IQueryable<Subscription>>().Setup(m => m.Provider).Returns(() => data.Provider);
            queriable.As<IQueryable<Subscription>>().Setup(m => m.Expression).Returns(() => data.Expression);
            queriable.As<IQueryable<Subscription>>().Setup(m => m.ElementType).Returns(() => data.ElementType);
            queriable.As<IQueryable<Subscription>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            return queriable;
        }

        public void Dispose()
        {
            stream = null;
        }

        [Fact]
        public void StreamStart()
        {
            stream.ConfigureStream();
            stream.Start();
            System.Threading.Thread.Sleep(2000);
            Assert.True(stream.StreamStatus() == Tweetinvi.Models.StreamState.Running);
        }

        [Fact]
        public void StreamStop()
        {
            stream.Stop();
            Assert.True(stream.StreamStatus() == Tweetinvi.Models.StreamState.Stop);
        }

        [Fact]
        public void StreamFollowsPerson()
        {
            Assert.True(stream.GetStream().FollowingUserIds.Count > 0);
        }
    }
}
