﻿//-----------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------

namespace TodoApi.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TodoApi.Models;

    /// <summary>
    /// Interface for interacting with the todo repository.
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Gets all todo items.
        /// </summary>
        /// <returns>A collection of todo items.</returns>
        Task<IEnumerable<TodoItem>> GetTodoItems();

        /// <summary>
        /// Gets a specific todo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the todo item.</param>
        /// <returns>The todo item with the specified ID.</returns>
        Task<TodoItem> GetTodoItem(int id);

        /// <summary>
        /// Adds a new todo item.
        /// </summary>
        /// <param name="todoItem">The todo item to add.</param>
        Task AddTodoItem(TodoItem todoItem);

        /// <summary>
        /// Updates an existing todo item.
        /// </summary>
        /// <param name="todoItem">The updated todo item.</param>
        Task UpdateTodoItem(TodoItem todoItem);

        /// <summary>
        /// Deletes a todo item.
        /// </summary>
        /// <param name="id">The ID of the todo item to delete.</param>
        Task DeleteTodoItem(int id);

        /// <summary>
        /// Checks if a todo item with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the todo item.</param>
        /// <returns>True if the todo item exists, otherwise false.</returns>
        Task<bool> TodoItemExists(int id);
    }
}
