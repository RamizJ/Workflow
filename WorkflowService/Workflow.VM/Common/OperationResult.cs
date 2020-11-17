using System.Collections.Generic;

namespace Workflow.VM.Common
{
    
    public class OperationResult
    {
        /// <summary>
        /// Успешность выполнения операции
        /// </summary>
        public bool Succeeded { get; set; } = true;

        /// <summary>
        /// Ошибки
        /// </summary>
        public IReadOnlyCollection<string> Errors { get; private set; } = new List<string>();


        /// <summary>
        /// Конструктор
        /// </summary>
        public OperationResult()
        { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="errorMessages"></param>
        /// <param name="succeeded"></param>
        public OperationResult(IEnumerable<string> errorMessages, bool succeeded)
        {
            Errors = new List<string>(errorMessages);
            Succeeded = succeeded;
        }

        /// <summary>
        /// Добавить ошибку
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        /// <param name="succeeded">Успешность операции</param>
        public void AddError(string message, bool succeeded = false)
        {
            Succeeded = succeeded;
            Errors = new List<string>(Errors) { message }.AsReadOnly();
        }

        /// <summary>
        /// Добавить ошибки
        /// </summary>
        /// <param name="messages">Тексты ошибок</param>
        /// <param name="succeeded">Успешность операции</param>
        public void AddErrors(IEnumerable<string> messages, bool succeeded = false)
        {
            Succeeded = succeeded;
            var errors = new List<string>(Errors);
            errors.AddRange(messages);
            Errors = errors.AsReadOnly();
        }
    }


    /// <inheritdoc />
    public class OperationResult<TVm> : OperationResult
    {
        /// <summary>
        /// Данные
        /// </summary>
        public TVm Data { get; set; }

        public OperationResult()
        { }

        public OperationResult(IEnumerable<string> errorMessages, bool succeeded)
        : base(errorMessages, succeeded)
        { }
    }
}