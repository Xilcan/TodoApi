﻿//-----------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------

/// <summary>
/// Contains the data context for todo items.
/// </summary>
namespace TodoApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using TodoApi.Interfaces;
    using TodoApi.Models;

    /// <summary>
    /// Represents the database context for todo items.
    /// </summary>
    public class TodoContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TodoContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the context.</param>
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the todo items DbSet.
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}