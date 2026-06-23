using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Core;

public sealed class Result<T>
{
    private readonly ICollection<Error> _errors = [];

    private Result(bool success, T data)
    {
        IsSuccess = success;
        Data = data;
    }

    private Result([NotNull] IReadOnlyCollection<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        IsSuccess = false;
        foreach (var error in errors)
        {
            ArgumentNullException.ThrowIfNull(error);
            _errors.Add(error);
        }
    }

    private Result([NotNull] Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        IsSuccess = false;
        _errors.Add(error);
    }

    public bool IsSuccess { get; private set; }
    public ImmutableList<Error> Errors => [.. _errors];
    public T? Data { get; private set; }

    public static Result<T> Success([NotNull] T data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return new Result<T>(true, data);
    }

    public static Result<T> Fail(Error error) => new(error);
    public static Result<T> Fail(IReadOnlyCollection<Error> errors) => new(errors);

    public static Result<T> operator +(Result<T> left, Result<T> right)
    {
        if (left.IsSuccess && right.IsSuccess)
        {
            return Result<T>.Success(left.Data!);
        }

        var errors = left.Errors.Concat(right.Errors).ToList();
        return Result<T>.Fail(errors);
    }

    public static Result<T> operator +(Result<T> left, Result right)
    {
        if (left.IsSuccess && right.IsSuccess)
        {
            return Result<T>.Success(left.Data!);
        }
        var errors = left.Errors.Concat(right.Errors).ToList();
        return Result<T>.Fail(errors);
    }

    public static implicit operator Result<T>(T data) => Success(data);

    public static implicit operator Result<T>(Error error) => Fail(error);

    public static bool operator !=(Result<T> left, Result<T> right) => !(left == right);

    public static bool operator ==(Result<T> left, Result<T> right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        if (left.GetHashCode() == right.GetHashCode()) return true;
        return false;
    }

    public override int GetHashCode() => IsSuccess.GetHashCode() * 17 + _errors.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is not Result<T> other) return false;
        return this == other;
    }
}
