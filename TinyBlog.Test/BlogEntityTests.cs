﻿using TinyBlog.Core.Entities;
using Xunit;

namespace TinyBlog.Test
{
    public class BlogEntityTests
    {
        private static Blog CreateBlogItem()
        {
            return new Blog()
            {
                Title = "Tiny Blog Engine",
                Name = "Tiny Blog"
            };
        }


        [Fact]
        public void NewBlog_HasId()
        {
            var blog = CreateBlogItem();
            Assert.NotEqual(string.Empty, blog.RowKey);
        }

        [Fact]
        public void NewBlog_HasRequiredValues_Set()
        {
            var blog = CreateBlogItem();
            Assert.NotEqual(string.Empty, blog.Title);
            Assert.NotEqual(string.Empty, blog.Name);
        }
    }
}
