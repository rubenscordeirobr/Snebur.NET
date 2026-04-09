namespace Snebur.FunctionalTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static void MediaTypeShouldBeApplicationJson(this HttpResponseMessage response)
    {
        var mediaType = response.Content?.Headers?.ContentType?.MediaType;
        mediaType.Should().Be("application/json");

    }
}

