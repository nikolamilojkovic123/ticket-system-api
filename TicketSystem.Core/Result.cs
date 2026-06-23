using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TicketSystem.Core;

public class Result
{
    private static readonly Result _successfulResult = new(true);
    private readonly ICollection<Error> _errors = [];


    private Result(bool success)
    {
        IsSuccess = success;
    }

    private Result([NotNull] Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        IsSuccess = false;
        _errors.Add(error);
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

    public bool IsSuccess { get; protected set; }
    public ImmutableList<Error> Errors => [.. _errors];

    public static Result Success() => _successfulResult;
    public static Result Fail(Error error) => new(error);
    public static Result Fail(IReadOnlyCollection<Error> errors) => new(errors);

    public static Result operator +(Result left, Result right)
    {
        if (left.IsSuccess && right.IsSuccess)
        {
            return Success();
        }

        var errors = left.Errors.Concat(right.Errors).ToList();
        return Fail(errors);
    }

    public static implicit operator Result(Error error) => Fail(error);

    public static bool operator !=(Result left, Result right) => !(left == right);

    public static bool operator ==(Result left, Result right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        if (left.GetHashCode() == right.GetHashCode()) return true;
        return false;
    }

    public override int GetHashCode() => IsSuccess.GetHashCode() * 17 + _errors.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is not Result other) return false;
        return this == other;
    }
}
