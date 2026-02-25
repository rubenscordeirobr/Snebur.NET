using Snebur.Presentation.Common;
using Snebur.Presentation.Common.Attributes;
using Snebur.Presentation.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Reflection;

namespace Snebur.Application.UnitTests.Presentation.Common;

public class HttpGetDescriptorSelectorTests
{
    private HttpMethodDescriptor CreateDescriptor(MethodInfo method)
    {
        var endpointDescriptor = new HttpEndpointDescriptor(method!);
        return endpointDescriptor.GetRequiredMethodDescriptor(method);
    }

    [Fact]
    public void Select_WhenSingleDescriptorProvided_ShouldReturnThatDescriptor()
    {
        // Arrange: Only one descriptor is provided.
        var method = typeof(SelectorTestEndpoint).GetMethod(nameof(SelectorTestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var descriptors = new[] { descriptor };
        var context = new DefaultHttpContext();
        context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "filter", "abc" }
            });

        // Act
        var selected = HttpGetDescriptorSelector.Select(context, descriptors);

        // Assert
        selected.Should().BeSameAs(descriptor);
    }

    [Fact]
    public void Select_WhenExactQueryTemplateMatchExists_ShouldReturnExactMatchDescriptor()
    {
        // Arrange:
        // Simulate a request with query parameters "filter" and "sort".
        var queryDict = new Dictionary<string, StringValues>
            {
                { "filter", "abc" },
                { "sort", "desc" }
            };
        var context = new DefaultHttpContext();
        context.Request.Query = new QueryCollection(queryDict);
        // Create two descriptors:
        // One with QueryTemplate exactly "filter={filter}&sort={sort}".
        var methodExact = typeof(SelectorTestEndpoint).GetMethod(nameof(SelectorTestEndpoint.GetByFilterAndSort));
        var descriptorExact = CreateDescriptor(methodExact!);
        // One with a different template.
        var methodOther = typeof(SelectorTestEndpoint).GetMethod(nameof(SelectorTestEndpoint.GetBySort));
        var descriptorOther = CreateDescriptor(methodOther!);
        var descriptors = new[] { descriptorOther, descriptorExact };

        // The CreateQueryTemplate returns "filter={filter}&sort={sort}".
        var requestQueryTemplate = HttpGetDescriptorSelector.CreateQueryTemplate(context);
        requestQueryTemplate.Should().Be("filter={filter}&sort={sort}");

        // Act
        var selected = HttpGetDescriptorSelector.Select(context, descriptors);

        // Assert
        selected.Should().BeSameAs(descriptorExact);
    }

    [Fact]
    public void Select_WhenNoExactMatchExists_ShouldReturnBestMatchingDescriptor()
    {
        // Arrange:
        // Simulate a request with query parameters "filter" and "sort".
        var queryDict = new Dictionary<string, StringValues>
            {
                { "filter", "abc" },
                { "sort", "desc" }
            };
        var context = new DefaultHttpContext();
        context.Request.Query = new QueryCollection(queryDict);
        // Create descriptors that do not exactly match:
        // One with QueryTemplate "filter={filter}"
        var methodFilter = typeof(SelectorTestEndpoint).GetMethod(nameof(SelectorTestEndpoint.GetItem));
        var descriptorFilter = CreateDescriptor(methodFilter!);
        // One with QueryTemplate "sort={sort}"
        var methodSort = typeof(SelectorTestEndpoint).GetMethod(nameof(SelectorTestEndpoint.GetBySort));
        var descriptorSort = CreateDescriptor(methodSort!);
        var descriptors = new[] { descriptorFilter, descriptorSort };

        // Act
        var selected = HttpGetDescriptorSelector.Select(context, descriptors);

        // In this scenario, both descriptors have 1 common key and key count diff 1.
        // According to the algorithm, the first encountered is chosen.
        selected.Should().BeSameAs(descriptorFilter);
    }

    [Fact]
    public void CreateQueryTemplate_WhenNoQueryParameters_ShouldReturnEmptyString()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Query = new QueryCollection();

        // Act
        var queryTemplate = HttpGetDescriptorSelector.CreateQueryTemplate(context);

        // Assert
        queryTemplate.Should().BeEmpty();
    }
}

public class SelectorTestEndpoint
{
    // QueryTemplate: "filter={filter}"
    [HttpGet("items/{id}", "filter={filter}")]
    public void GetItem(int id, string filter) { }

    // QueryTemplate: "filter={filter}&sort={sort}"
    [HttpGet("items/{id}", "filter={filter}&sort={sort}")]
    public void GetByFilterAndSort(int id, string filter, string sort) { }

    // QueryTemplate: "sort={sort}"
    [HttpGet("items/{id}", "sort={sort}")]
    public void GetBySort(int id, string sort) { }
}
