using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Utilities.Results
{
    public class Result
    {
        List<Error> _errors = new();
        public bool IsSuccess => !_errors.Any();
        public bool IsFailure => !IsSuccess;
        public string? Message { get; init; }
        public IReadOnlyCollection<Error> Errors => _errors;

        protected Result()
        {
        }

        protected Result(string? message)
        {
            Message = message;
        }

        protected Result(IEnumerable<Error> errors, string? message)
        {
            _errors.AddRange(errors);
            Message = message;
        }

        public static Result Success(string? message = "Tarea realizada con éxito") => new(message);
        public static Result Failure(Error error, string? message = "Ha ocurrido un error") => new([error], message);
        public static Result Failure(IEnumerable<Error> errors, string? message = "Ha ocurrido un error") => new(errors, message);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value, string? message = null) : base(message)
        {
            Value = value;
        }

        private Result(IEnumerable<Error> errors, string? message) : base(errors, message) {}

        public static Result<T> Success(T value, string? message = null) => new(value, message);
        public static Result<T> Failure(Error error, string? message = null) => new([error], message);
        public static Result<T> Failure(IEnumerable<Error> errors, string? message = null) => new(errors, message);
    }
}
