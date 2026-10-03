namespace WorkoutLogger.API.Infrastructure;

public static class Validate
{
    public static IResult? Required(string? value, string field) =>
        string.IsNullOrWhiteSpace(value) ? Results.BadRequest($"{field} je obavezno.") : null;

    public static IResult? MaxLength(string? value, int max, string field) =>
        (value?.Length ?? 0) > max ? Results.BadRequest($"{field} ne smije biti duže od {max} znakova.") : null;

    public static IResult? MinLength(string? value, int min, string field) =>
        (value?.Length ?? 0) < min ? Results.BadRequest($"{field} mora imati najmanje {min} znakova.") : null;

    public static IResult? Min(decimal value, decimal min, string field) =>
        value < min ? Results.BadRequest($"{field} mora biti najmanje {min}.") : null;

    public static IResult? Range(int value, int min, int max, string field) =>
        value < min || value > max ? Results.BadRequest($"{field} mora biti između {min} i {max}.") : null;
}
