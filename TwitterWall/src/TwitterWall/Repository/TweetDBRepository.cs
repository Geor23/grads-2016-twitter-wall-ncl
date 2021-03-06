﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitterWall.Context;
using TwitterWall.Models;

namespace TwitterWall.Repository
{
    public class TweetDBRepository : Repository<Tweet>
    {
        public TweetDBRepository() : base()
        {
        }

        public TweetDBRepository(TweetContext ctx) : base(ctx)
        {
        }

        public override Tweet Get(long id)
        {
            using (TweetContext context = GetContext())
            {
                return context.Tweets.Where(t => t.Id == id).SingleOrDefault();
            }
        }

        public override IEnumerable<Tweet> GetAll()
        {
            using (TweetContext context = GetContext())
            {
                return context.Tweets.Include(t => t.MediaList).ToList();
            }
        }

        public IEnumerable<Tweet> GetLatest(int limit, string eventName)
        {
            using (TweetContext context = GetContext())
            {
                Event ev = context.Events.Where(e => e.Name == eventName).SingleOrDefault();
                if (ev != null)
                {
                    List<Tweet> result = new List<Tweet>();
                    // Prioritise stickies
                    result.AddRange(context.Tweets.Where(t => t.Sticky && t.Event.Name == ev.Name).Include(t => t.MediaList));
                    int fill = limit - result.Count;
                    if (fill > 0)
                    {
                        List<Tweet> latestNonStickyTweets = new List<Tweet>();
                        latestNonStickyTweets.AddRange(context.Tweets.OrderByDescending(t => t.Date).Include(t => t.MediaList).Where(t => !t.Sticky && t.Event.Name == ev.Name).Take(fill));
                        latestNonStickyTweets.Reverse();
                        result.AddRange(latestNonStickyTweets);
                    }
                    return result;
                }
                return null;
            }
        }

        public override IEnumerable<Tweet> Find(Func<Tweet, bool> exp)
        {
            using (TweetContext context = GetContext())
            {
                return context.Tweets.Include(t => t.MediaList).Include(t => t.Event).Where<Tweet>(exp).ToList();
            }
        }

        public override void Add(Tweet entity)
        {
            using (TweetContext context = GetContext())
            {
                context.Attach(entity.Event);
                context.Tweets.Add(entity);
                context.SaveChanges();
            }
        }

        public override void Remove(long id)
        {
            using (TweetContext context = GetContext())
            {
                Tweet tweet = Get(id);
                if (tweet != null)
                {
                    context.Attach(tweet);
                    context.Tweets.Remove(tweet);
                    context.SaveChanges();
                }
            }
        }

        public Tweet GetLastTweet()
        {
            using (TweetContext context = GetContext())
            {
                return context.Tweets
                    .Include(t => t.MediaList)
                    .Include(t => t.Event)
                    .FirstOrDefault(t => t.Date == context.Tweets.Max(x => x.Date));
            }
        }

        public void ToggleSticky(long id)
        {
            using (TweetContext context = GetContext())
            {
                Tweet tweet = context.Tweets.Where(t => t.Id == id).SingleOrDefault();
                if (tweet != null)
                {
                    tweet.Sticky = !tweet.Sticky;
                    context.Tweets.Update(tweet);
                    context.SaveChanges();
                }
            }
        }
    }
}