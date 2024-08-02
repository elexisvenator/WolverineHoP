using OneOf;
using System.Net;
using System.Text.Json;
using OneOf.Types;

namespace WolverineHoP.Web.Api;

using ModelErrors = Dictionary<string, List<string>>;
public static class ApiExtensions
{
    public static async Task<OneOf<TResponse, ModelErrors>> ParseResponseOrError<TResponse>(
        this HttpResponseMessage response,
        CancellationToken token = default)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.GetModelErrors(token);
        }

        response.EnsureSuccessStatusCode();
        return (await response.ParseResponse<TResponse>(token))!;
    }

    public static async Task<OneOf<Yes, ModelErrors>> ParseSuccessOrError(
        this HttpResponseMessage response,
        CancellationToken token = default)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.GetModelErrors(token);
        }

        response.EnsureSuccessStatusCode();
        return new Yes();
    }

    private class ErrorModel
    {
        public string? Title { get; init; }
        public ModelErrors Errors { get; init; } = new();
    }

    private static async Task<ModelErrors> GetModelErrors(
        this HttpResponseMessage response,
        CancellationToken token = default)
    {

        var content = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
        try
        {
            var errorModel = JsonSerializer.Deserialize<ErrorModel>(content);
            if (errorModel is null)
            {
                return new ModelErrors { { "", [content] } };
            }

            if (!errorModel.Errors.Any())
            {
                return new ModelErrors { { "", [errorModel.Title ?? content] } };
            }

            return errorModel.Errors;
        }
        catch (JsonException)
        {
            try
            {
                var errors = JsonSerializer.Deserialize<ModelErrors>(content);
                if (errors is null || !errors.Any())
                {
                    return new ModelErrors { { "", [content] } };
                }

                return errors;

            }
            catch (JsonException)
            {
                return new ModelErrors { { "", [content] } };
            }
        }
    }

    private static async Task<TResponse?> ParseResponse<TResponse>(
        this HttpResponseMessage message,
        CancellationToken token = default)
    {
        var stream = await message.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync<TResponse>(stream, cancellationToken: token);
    }
}